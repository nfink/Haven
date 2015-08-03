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

        public IEnumerable<Player> Players
        {
            get
            {
                return Persistence.Connection.Table<Player>().Where(x => x.GameId == this.Id);
            }
        }

        public int CurrentPlayerId { get; set; }

        public Board Board
        {
            get
            {
                return Persistence.Connection.Get<Board>(this.BoardId);
            }
        }

        public int BoardId { get; set; }

        private static Random Rand = new Random();

        public static Game NewGame(int boardId, int numberOfPlayers)
        {
            var game = new Game();
            game.BoardId = boardId;
            game.OwnerId = Persistence.Connection.Get<Board>(boardId).OwnerId;
            Persistence.Connection.Insert(game);

            int firstSpaceId = Persistence.Connection.Query<Space>("select [Id] from [Space] where [Order]=(select min([Order]) from [Space] where BoardId=?) and BoardId=?", game.BoardId, game.BoardId).First().Id;

            var players = new List<Player>();

            // create players
            for (int i = 0; i < numberOfPlayers; i++)
            {
                var player = new Player();
                player.GameId = game.Id;
                player.Guid = Guid.NewGuid().ToString();
                Persistence.Connection.Insert(player);

                // add actions for setting up the player
                Persistence.Connection.Insert(new Action() { Type = ActionType.SelectPiece, OwnerId = player.Id });
                Persistence.Connection.Insert(new Action() { Type = ActionType.EnterName, OwnerId = player.Id });
                Persistence.Connection.Insert(new Action() { Type = ActionType.EnterPassword, OwnerId = player.Id });

                // start player on the first space
                player.SpaceId = firstSpaceId;

                players.Add(player);
            }

            // set up turn order
            for (int i = 0; i < players.Count; i++)
            {
                players[i].NextPlayerId = players[(i + 1) % players.Count].Id;
            }

            Persistence.Connection.UpdateAll(players);
            game.CurrentPlayerId = players[0].Id;
            Persistence.Connection.Update(game);
            return game;
        }

        public static int NextPlayer(int currentPlayerId)
        {
            var player = Persistence.Connection.Get<Player>(currentPlayerId);
            Persistence.Connection.Execute("update Game set CurrentPlayerId=?", player.NextPlayerId);
            return player.NextPlayerId;
        }

        public static void EndTurn(int currentPlayerId)
        {
            int nextPlayerId = Game.NextPlayer(currentPlayerId);
            Persistence.Connection.Insert(new Action() { Type = ActionType.Roll, OwnerId = nextPlayerId });
        }

        public void StartGame()
        {
            // if all players have selected a piece and entered a name and password,  start game
            if (Persistence.Connection.Query<Player>("select Player.* from Player where GameId=? and (PieceId=0 or Name is null or Password is null)", this.Id).Count < 1)
            {
                Persistence.Connection.Insert(new Action() { Type = ActionType.Roll, OwnerId = this.CurrentPlayerId });

                // clone the board so that edits won't affect existing games
                var board = this.Board;
                board.OwnerId = 0;
                this.BoardId = board.Clone().Id;
                Persistence.Connection.Update(this);
            }
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
