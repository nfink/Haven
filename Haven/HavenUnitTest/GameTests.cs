using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Haven;

namespace HavenUnitTest
{
    [TestFixture]
    public class GameTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            // create necessary tables
            Persistence.Connection.CreateTable<Challenge>();
            Persistence.Connection.CreateTable<ChallengeAnswer>();
            Persistence.Connection.CreateTable<UsedChallenge>();
            Persistence.Connection.CreateTable<Board>();
            Persistence.Connection.CreateTable<Haven.Action>();
            Persistence.Connection.CreateTable<PlayerNameCard>();
            Persistence.Connection.CreateTable<PlayerSafeHavenCard>();
            Persistence.Connection.CreateTable<Message>();
            Persistence.Connection.CreateTable<Player>();
            Persistence.Connection.CreateTable<Game>();
        }

        [Test]
        public void Delete()
        {
            // create games with players and used challenges
            var game1 = CreateGameData();
            var game2 = CreateGameData();

            // delete a game
            game1.Delete();

            // verify that the game is deleted
            Assert.IsEmpty(Persistence.Connection.Table<Game>().Where(x => x.Id == game1.Id));

            // verify that players are deleted
            Assert.IsEmpty(Persistence.Connection.Table<Player>().Where(x => x.GameId == game1.Id));

            // verify that challenges are not marked as used
            Assert.IsEmpty(Persistence.Connection.Table<UsedChallenge>().Where(x => x.GameId == game1.Id));

            // verify that data from other game is not deleted
            Assert.IsNotEmpty(Persistence.Connection.Table<Game>().Where(x => x.Id == game2.Id));
            Assert.IsNotEmpty(Persistence.Connection.Table<Player>().Where(x => x.GameId == game2.Id));
            Assert.IsNotEmpty(Persistence.Connection.Table<UsedChallenge>().Where(x => x.GameId == game2.Id));
        }

        [Test]
        public void DeleteEmptyGame()
        {
            // create a game without players or used challenges
            var game = new Game();
            Persistence.Connection.Insert(game);

            // delete the game
            game.Delete();

            // verify that deletion works
            Assert.IsEmpty(Persistence.Connection.Table<Game>().Where(x => x.Id == game.Id));
        }

        [Test]
        public void GetNextChallenge()
        {
            // create a game with some challenges
            var board = new Board();
            Persistence.Connection.Insert(board);
            var game = new Game() { BoardId = board.Id };
            Persistence.Connection.Insert(game);
            var challenge1 = new Challenge() { BoardId = board.Id };
            var challenge2 = new Challenge() { BoardId = board.Id };
            var challenge3 = new Challenge() { BoardId = board.Id };
            var challenges = new Challenge[] { challenge1, challenge2, challenge3 };
            Persistence.Connection.InsertAll(challenges);

            // keep getting the next challenge until all challenges should have been used
            var usedChallenges = new List<Challenge>();
            foreach (Challenge challenge in challenges)
            {
                usedChallenges.Add(game.GetNextChallenge());
            }

            // verify that all challenges were used
            Assert.AreEqual(challenges.Select(x => x.Id).OrderBy(x => x), usedChallenges.Select(x => x.Id).OrderBy(x => x));

            // verify that more challenges can be retrieved
            Assert.IsNotNull(game.GetNextChallenge());
        }

        private Game CreateGameData()
        {
            // create game
            var game = new Game();
            Persistence.Connection.Insert(game);

            // add challenges
            var challenge1 = new Challenge();
            var challenge2 = new Challenge();
            var challenge3 = new Challenge();
            var challenge4 = new Challenge();
            Persistence.Connection.InsertAll(new Challenge[] { challenge1, challenge2, challenge3, challenge4 });
            
            // mark some challenges as used
            Persistence.Connection.Insert(new UsedChallenge() { GameId = game.Id, ChallengeId = challenge1.Id });
            Persistence.Connection.Insert(new UsedChallenge() { GameId = game.Id, ChallengeId = challenge2.Id });

            // add players
            var player1 = new Player() { GameId = game.Id };
            var player2 = new Player() { GameId = game.Id };
            var player3 = new Player() { GameId = game.Id };
            Persistence.Connection.InsertAll(new Player[] { player1, player2, player3 });

            // set up player turn order
            player1.NextPlayerId = player2.Id;
            player2.NextPlayerId = player3.Id;
            player3.NextPlayerId = player1.Id;
            Persistence.Connection.UpdateAll(new Player[] { player1, player2, player3 });

            return game;
        }
    }
}
