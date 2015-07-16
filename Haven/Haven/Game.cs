using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

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
            }
        }

        public Challenge GetNextChallenge()
        {
            var unusedChallenges = this.GetUnusedChallenges();

            if (unusedChallenges.Count() < 1)
            {
                Persistence.Connection.Execute("delete from UsedChallenge where GameId=?", this.Id);
                unusedChallenges = this.GetUnusedChallenges();
            }

            var nextChallenge = unusedChallenges.OrderBy(x => Rand.Next()).First();
            Persistence.Connection.Insert(new UsedChallenge() { ChallengeId = nextChallenge.Id, GameId = this.Id });
            return nextChallenge;
        }

        private IEnumerable<Challenge> GetUnusedChallenges()
        {
            return Persistence.Connection.Query<Challenge>(
                @"select Challenge.* from Challenge
                  left join UsedChallenge on Challenge.Id=UsedChallenge.ChallengeId
                  where (UsedChallenge.ChallengeId is null or UsedChallenge.GameId<>?)
                  and Challenge.BoardId=?",
                  this.Id, this.BoardId);
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
