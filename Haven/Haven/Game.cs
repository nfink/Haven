using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Haven
{
    public class Game : IDeletable, IEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Ignore]
        public IRepository Repository { private get; set; }

        public int OwnerId { get; set; }

        public string Name { get; set; }

        public int Turn { get; set; }

        public bool Ended { get; set; }

        public IEnumerable<Player> Players
        {
            get
            {
                return this.Repository.Find<Player>(x => x.GameId == this.Id);
            }
        }

        public Board Board
        {
            get
            {
                return this.BoardId == 0 ? null : this.Repository.Get<Board>(this.BoardId);
            }
        }

        public int BoardId { get; set; }

        public IEnumerable<GameWinner> Winners
        {
            get
            {
                return this.Ended ? this.Repository.Find<GameWinner>(x => x.GameId == this.Id) : null;
            }
        }

        private static Random Rand = new Random();

        public void Create(int boardId, int numberOfPlayers)
        {
            // clone the board so that edits won't affect existing games
            var board = this.Repository.Get<Board>(boardId);
            this.OwnerId = board.OwnerId;
            board.OwnerId = 0;
            this.BoardId = board.Clone().Id;
            this.Repository.Add(this);

            // create players
            for (int i = 0; i < numberOfPlayers; i++)
            {
                this.AddPlayer();
            }
        }

        public Player AddPlayer()
        {
            // create a player
            var player = new Player();
            player.GameId = this.Id;
            player.Guid = Guid.NewGuid().ToString();
            var players = this.Players;
            if (players.Count() > 0)
            {
                player.TurnOrder = players.Select(x => x.TurnOrder).Max() + 1;
            }

            // start player on the first space
            player.SpaceId = this.Board.StartingSpace.Id;
            this.Repository.Add(player);

            // add actions for setting up the player
            this.Repository.Add(new Action() { Type = ActionType.SelectPiece, OwnerId = player.Id });
            this.Repository.Add(new Action() { Type = ActionType.EnterName, OwnerId = player.Id });
            this.Repository.Add(new Action() { Type = ActionType.EnterPassword, OwnerId = player.Id });

            return player;
        }

        public void EndTurn(int currentPlayerId)
        {
            // a player wins if they have more than the required number of name and safe haven cards
            // -1 for # of cards to end means that the player must collect all cards
            var board = this.Board;
            var players = this.Players;
            var winners = new List<Player>();
            var totalNameCards = board.NameCards.Count();
            var totalSafeHavenCards = board.SafeHavenCards.Count();
            if (totalNameCards > 0)
            {
                foreach (Player player in players)
                {
                    var playerNameCards = player.NameCards.Count();
                    if (((board.NameCardsToEnd >= 0) && (playerNameCards >= board.NameCardsToEnd)) ||
                        (playerNameCards >= totalNameCards))
                    {
                        var playerSafeHavenCards = player.SafeHavenCards.Count();
                        if (((board.SafeHavenCardsToEnd >= 0) && (playerSafeHavenCards >= board.SafeHavenCardsToEnd)) ||
                            (playerSafeHavenCards >= totalSafeHavenCards))
                        {
                            winners.Add(player);
                        }
                    }
                }
            }

            // if no player has enough cards to win and the game is over by turns, determine winner
            // winner has the most safe haven cards
            // if there is a tie, winner has the most name cards
            // if there is still a tie, multiple players win
            if (((board.TurnsToEnd > 0) && (this.Turn >= board.TurnsToEnd)) && winners.Count < 1)
            {
                int maxSafeHavenCards = players.Max(x => x.SafeHavenCards.Count());
                var safeHavenWinners = players.Where(x => x.SafeHavenCards.Count() == maxSafeHavenCards);
                int maxNameCards = safeHavenWinners.Max(x => x.NameCards.Count());
                var nameCardWinners = players.Where(x => x.NameCards.Count() == maxNameCards);
                winners.AddRange(nameCardWinners);
            }

            // end the game if conditions have been met
            if (winners.Count > 0)
            {
                this.EndGame(winners);
            }
            else
            {
                // otherwise start the next player's turn
                this.NextPlayer(currentPlayerId);
            }
        }

        public void NextPlayer(int currentPlayerId)
        {
            var currentPlayer = this.Repository.Get<Player>(currentPlayerId);
            var players = this.Players.OrderBy(x => x.TurnOrder).ToList();
            var index = players.IndexOf(currentPlayer) + 1;
            if (index >= players.Count)
            {
                index = 0;
            }

            var nextPlayer = players[index];
            this.Turn++;
            this.Repository.Update(this);
            this.Repository.Add(new Action() { Type = ActionType.Roll, OwnerId = nextPlayer.Id });
        }

        public void StartGame()
        {
            // if all players have selected a piece and entered a name and password,  start game
            if (this.Players.Where(x => x.PieceId == 0 || x.Name == null || x.Password == null).Count() < 1)
            {
                var firstPlayer = this.Players.OrderBy(x => x.TurnOrder).First();
                this.Repository.Add(new Action() { Type = ActionType.Roll, OwnerId = firstPlayer.Id });
            }
        }

        public void EndGame(IEnumerable<Player> winners)
        {
            // mark game as ended
            this.Ended = true;

            // record winners
            foreach (Player player in winners)
            {
                this.Repository.Add(new GameWinner() { GameId = this.Id, Player = player.Name, Turn = this.Turn });
            }

            // delete players, used challenges, board
            foreach (Player player in this.Players)
            {
                player.Delete();
            }
            foreach (UsedChallenge usedChallenge in this.Repository.Find<UsedChallenge>(x => x.GameId == this.Id))
            {
                this.Repository.Remove(usedChallenge);
            }
            this.Board.Delete();
        }

        public Challenge GetNextChallenge(int spaceId)
        {
            // get challenge categories that the space can use (from space, or board if the space has no specified categories)
            IEnumerable<int> categories;
            var spaceCategories = this.Repository.Find<SpaceChallengeCategory>(x => x.SpaceId == spaceId);
            if (spaceCategories.Count() > 0)
            {
                categories = spaceCategories.Select(x => x.ChallengeCategoryId);
            }
            else
            {
                categories = this.Board.ChallengeCategories.Select(x => x.ChallengeCategoryId);
            }

            // get unused challenges from each category, and if a category has no unused challenges, mark all as unused
            var unusedChallenges = new List<int>();
            foreach (int categoryId in categories)
            {
                var categoryChallenges = this.Repository.Find<Challenge>(x => x.ChallengeCategoryId == categoryId);
                var usedCategoryChallenges = this.Repository.Find<UsedChallenge>(x => x.GameId == this.Id);
                var unusedCategoryChallenges = categoryChallenges.Select(x => x.Id).Except(usedCategoryChallenges.Select(x => x.ChallengeId));

                if (unusedCategoryChallenges.Count() < 1)
                {
                    foreach (UsedChallenge usedChallenge in categoryChallenges.SelectMany(x => this.Repository.Find<UsedChallenge>(y => x.Id == y.ChallengeId)).ToList())
                    {
                        this.Repository.Remove(usedChallenge);
                    }

                    unusedChallenges.AddRange(categoryChallenges.Select(x => x.Id));
                }
                else
                {
                    unusedChallenges.AddRange(unusedCategoryChallenges);
                }
            }

            // select a random challenge to use and mark as used
            var nextChallenge = unusedChallenges.OrderBy(x => Rand.Next()).First();
            this.Repository.Add(new UsedChallenge() { ChallengeId = nextChallenge, GameId = this.Id });
            return this.Repository.Get<Challenge>(nextChallenge);
        }

        public void Delete()
        {
            // delete players
            foreach (Player player in this.Players.ToList())
            {
                player.Delete();
            }

            // delete record of used challenges
            foreach (UsedChallenge usedChallenge in this.Repository.Find<UsedChallenge>(x => x.GameId == this.Id).ToList())
            {
                this.Repository.Remove(usedChallenge);
            }

            // delete cloned board
            if (this.BoardId != 0)
            {
                this.Board.Delete();
            }

            // delete game
            this.Repository.Remove(this);
        }
    }
}
