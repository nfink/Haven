using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Haven
{
    public class Game : IDeletable
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int OwnerId { get; set; }

        public string Name { get; set; }

        public int Turn { get; set; }

        public bool Ended { get; set; }

        public IEnumerable<Player> Players
        {
            get
            {
                return Persistence.Connection.Table<Player>().Where(x => x.GameId == this.Id);
            }
        }

        public Board Board
        {
            get
            {
                return this.BoardId != 0 ? Persistence.Connection.Get<Board>(this.BoardId) : null;
            }
        }

        public int BoardId { get; set; }

        public IEnumerable<GameWinner> Winners
        {
            get
            {
                return this.Ended ? Persistence.Connection.Table<GameWinner>().Where(x => x.GameId == this.Id) : null;
            }
        }

        private static Random Rand = new Random();

        public static Game NewGame(int boardId, int numberOfPlayers)
        {
            var game = new Game();
            game.BoardId = boardId;
            game.OwnerId = Persistence.Connection.Get<Board>(boardId).OwnerId;
            Persistence.Connection.Insert(game);

            // create players
            for (int i = 0; i < numberOfPlayers; i++)
            {
                game.AddPlayer();
            }

            return game;
        }

        public static Game GetGame(int playerId)
        {
            return Persistence.Connection.Query<Game>("select Game.* from Game join Player on Player.GameId=Game.Id where Player.Id=?", playerId).SingleOrDefault();
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
            Persistence.Connection.Insert(player);

            // add actions for setting up the player
            Persistence.Connection.Insert(new Action() { Type = ActionType.SelectPiece, OwnerId = player.Id });
            Persistence.Connection.Insert(new Action() { Type = ActionType.EnterName, OwnerId = player.Id });
            Persistence.Connection.Insert(new Action() { Type = ActionType.EnterPassword, OwnerId = player.Id });

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
            var currentPlayer = Persistence.Connection.Get<Player>(currentPlayerId);
            var players = this.Players.OrderBy(x => x.TurnOrder).ToList();
            var index = players.IndexOf(currentPlayer) + 1;
            if (index >= players.Count)
            {
                index = 0;
            }

            var nextPlayer = players[index];
            Persistence.Connection.Execute("update Game set Turn=(Turn + 1) where Id=?", currentPlayer.GameId);
            Persistence.Connection.Insert(new Action() { Type = ActionType.Roll, OwnerId = nextPlayer.Id });
        }

        public void StartGame()
        {
            // if all players have selected a piece and entered a name and password,  start game
            if (Persistence.Connection.Query<Player>("select Player.* from Player where GameId=? and (PieceId=0 or Name is null or Password is null)", this.Id).Count < 1)
            {
                var firstPlayer = this.Players.OrderBy(x => x.TurnOrder).First();
                Persistence.Connection.Insert(new Action() { Type = ActionType.Roll, OwnerId = firstPlayer.Id });

                // clone the board so that edits won't affect existing games
                var board = this.Board;
                board.OwnerId = 0;
                this.BoardId = board.Clone().Id;
                Persistence.Connection.Update(this);
            }
        }

        public void EndGame(IEnumerable<Player> winners)
        {
            // mark game as ended
            this.Ended = true;

            // record winners
            foreach (Player player in winners)
            {
                Persistence.Connection.Insert(new GameWinner() { GameId = this.Id, Player = player.Name, Turn = this.Turn });
            }

            // delete players, used challenges, board
            foreach (Player player in this.Players)
            {
                player.Delete();
            }
            Persistence.Connection.Execute("delete from UsedChallenge where GameId=?", this.Id);
            this.Board.Delete();
        }

        public Challenge GetNextChallenge(int spaceId)
        {
            // get challenge categories that the space can use (from space, or board if the space has no specified categories)
            IEnumerable<int> categories;
            var spaceCategories = Persistence.Connection.Table<SpaceChallengeCategory>().Where(x => x.SpaceId == spaceId);
            if (spaceCategories.Count() > 0)
            {
                categories = Persistence.Connection.Query<ChallengeCategory>("select ChallengeCategory.* from ChallengeCategory join SpaceChallengeCategory on ChallengeCategory.Id=SpaceChallengeCategory.ChallengeCategoryId where SpaceChallengeCategory.SpaceId=?", spaceId).Select(x => x.Id);
            }
            else
            {
                categories = Persistence.Connection.Query<ChallengeCategory>("select ChallengeCategory.* from ChallengeCategory join BoardChallengeCategory on ChallengeCategory.Id=BoardChallengeCategory.ChallengeCategoryId where BoardChallengeCategory.BoardId=?", this.BoardId).Select(x => x.Id);
            }

            // get unused challenges from each category, and if a category has no unused challenges, mark all as unused
            foreach (int categoryId in categories)
            {
                var unusedCategoryChallenges = Persistence.Connection.Query<Challenge>(
                    @"select Challenge.* from 
                        (select Challenge.* from Challenge
                            join ChallengeCategory on Challenge.ChallengeCategoryId=ChallengeCategory.Id
                            where ChallengeCategory.Id=?) as Challenge
                        left join UsedChallenge on Challenge.Id=UsedChallenge.ChallengeId
                        where (UsedChallenge.ChallengeId is null or UsedChallenge.GameId<>?)",
                      categoryId, this.Id);

                if (unusedCategoryChallenges.Count() < 1)
                {
                    Persistence.Connection.Execute(
                        @"delete from UsedChallenge where Id in (
                            select UsedChallenge.Id from UsedChallenge
                            join Challenge on UsedChallenge.ChallengeId=Challenge.Id
                            join ChallengeCategory on Challenge.ChallengeCategoryId=ChallengeCategory.Id
                            where ChallengeCategory.Id=? and UsedChallenge.GameId=?)",
                        categoryId, this.Id);
                }
            }

            // get unused challenges from all categories
            IEnumerable<Challenge> unusedChallenges;
            if (spaceCategories.Count() > 0)
            {
                unusedChallenges = Persistence.Connection.Query<Challenge>(
                    @"select Challenge.* from 
                        (select Challenge.* from Challenge
                            join ChallengeCategory on Challenge.ChallengeCategoryId=ChallengeCategory.Id
                            where ChallengeCategory.Id in (
                                select ChallengeCategory.Id from ChallengeCategory
                                join SpaceChallengeCategory on ChallengeCategory.Id=SpaceChallengeCategory.ChallengeCategoryId
                                where SpaceChallengeCategory.SpaceId=?)) as Challenge
                        left join UsedChallenge on Challenge.Id=UsedChallenge.ChallengeId
                        where (UsedChallenge.ChallengeId is null or UsedChallenge.GameId<>?)",
                        spaceId, this.Id);

            }
            else
            {
                unusedChallenges = Persistence.Connection.Query<Challenge>(
                    @"select Challenge.* from 
                        (select Challenge.* from Challenge
                            join ChallengeCategory on Challenge.ChallengeCategoryId=ChallengeCategory.Id
                            where ChallengeCategory.Id in (
                                select ChallengeCategory.Id from ChallengeCategory
                                join BoardChallengeCategory on ChallengeCategory.Id=BoardChallengeCategory.ChallengeCategoryId
                                where BoardChallengeCategory.BoardId=?)) as Challenge
                        left join UsedChallenge on Challenge.Id=UsedChallenge.ChallengeId
                        where (UsedChallenge.ChallengeId is null or UsedChallenge.GameId<>?)",
                        this.BoardId, this.Id);
            }

            // select a random challenge to use and mark as used
            var nextChallenge = unusedChallenges.OrderBy(x => Rand.Next()).First();
            Persistence.Connection.Insert(new UsedChallenge() { ChallengeId = nextChallenge.Id, GameId = this.Id });
            return nextChallenge;
        }

        public void Delete()
        {
            // delete players
            foreach (Player player in this.Players)
            {
                player.Delete();
            }

            // delete record of used challenges
            Persistence.Connection.Execute("delete from UsedChallenge where GameId=?", this.Id);

            // delete game
            Persistence.Connection.Delete(this);
        }
    }
}
