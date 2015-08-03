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
            Persistence.Connection.CreateTable<ChallengeCategory>();
            Persistence.Connection.CreateTable<UsedChallenge>();
            Persistence.Connection.CreateTable<Board>();
            Persistence.Connection.CreateTable<BoardChallengeCategory>();
            Persistence.Connection.CreateTable<Space>();
            Persistence.Connection.CreateTable<SpaceChallengeCategory>();
            Persistence.Connection.CreateTable<Haven.Action>();
            Persistence.Connection.CreateTable<PlayerNameCard>();
            Persistence.Connection.CreateTable<PlayerSafeHavenCard>();
            Persistence.Connection.CreateTable<Message>();
            Persistence.Connection.CreateTable<Player>();
            Persistence.Connection.CreateTable<Game>();
        }

        [Test]
        public void DeleteGame()
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
        public void GetNextChallengeBoardCategory()
        {
            // create a game with some challenges
            var board = new Board();
            Persistence.Connection.Insert(board);
            var game = new Game() { BoardId = board.Id };
            Persistence.Connection.Insert(game);
            var category = new ChallengeCategory();
            Persistence.Connection.Insert(category);
            var challenge1 = new Challenge() { ChallengeCategoryId = category.Id };
            var challenge2 = new Challenge() { ChallengeCategoryId = category.Id };
            var challenge3 = new Challenge() { ChallengeCategoryId = category.Id };
            var challenges = new Challenge[] { challenge1, challenge2, challenge3 };
            Persistence.Connection.InsertAll(challenges);
            Persistence.Connection.Insert(new BoardChallengeCategory() { BoardId = board.Id, ChallengeCategoryId = category.Id });

            // keep getting the next challenge until all challenges should have been used
            var usedChallenges = new List<Challenge>();
            foreach (Challenge challenge in challenges)
            {
                usedChallenges.Add(game.GetNextChallenge(0));
            }

            // verify that all challenges were used
            CollectionAssert.AreEqual(challenges.Select(x => x.Id).OrderBy(x => x), usedChallenges.Select(x => x.Id).OrderBy(x => x));

            // verify that more challenges can be retrieved
            Assert.IsNotNull(game.GetNextChallenge(0));
        }

        [Test]
        public void GetNextChallengeMultipleBoardCategories()
        {
            // create a game with some challenges
            var board = new Board();
            Persistence.Connection.Insert(board);
            var game = new Game() { BoardId = board.Id };
            Persistence.Connection.Insert(game);
            var category1 = new ChallengeCategory();
            var category2 = new ChallengeCategory();
            Persistence.Connection.Insert(category1);
            Persistence.Connection.Insert(category2);
            var challenge1 = new Challenge() { ChallengeCategoryId = category1.Id };
            var challenge2 = new Challenge() { ChallengeCategoryId = category1.Id };
            var challenge3 = new Challenge() { ChallengeCategoryId = category2.Id };
            var challenge4 = new Challenge() { ChallengeCategoryId = category2.Id };
            var challenge5 = new Challenge() { ChallengeCategoryId = category2.Id };
            var challenges = new Challenge[] { challenge1, challenge2, challenge3, challenge4, challenge5 };
            Persistence.Connection.InsertAll(challenges);
            Persistence.Connection.Insert(new BoardChallengeCategory() { BoardId = board.Id, ChallengeCategoryId = category1.Id });
            Persistence.Connection.Insert(new BoardChallengeCategory() { BoardId = board.Id, ChallengeCategoryId = category2.Id });

            // get some challenges (which ones will be used is indeterminate)
            for (int i = 0; i < 10; i++)
            {
                game.GetNextChallenge(0);
            }
        }

        [Test]
        public void GetNextChallengeSpaceCategory()
        {
            // create a game with some challenges
            var board = new Board();
            Persistence.Connection.Insert(board);
            var game = new Game() { BoardId = board.Id };
            Persistence.Connection.Insert(game);
            var category = new ChallengeCategory();
            Persistence.Connection.Insert(category);
            var challenge1 = new Challenge() { ChallengeCategoryId = category.Id };
            var challenge2 = new Challenge() { ChallengeCategoryId = category.Id };
            var challenge3 = new Challenge() { ChallengeCategoryId = category.Id };
            var challenges = new Challenge[] { challenge1, challenge2, challenge3 };
            Persistence.Connection.InsertAll(challenges);
            var space = new Space() { BoardId = board.Id };
            Persistence.Connection.Insert(space);
            Persistence.Connection.Insert(new SpaceChallengeCategory() { SpaceId=space.Id, ChallengeCategoryId = category.Id });

            // keep getting the next challenge until all challenges should have been used
            var usedChallenges = new List<Challenge>();
            foreach (Challenge challenge in challenges)
            {
                usedChallenges.Add(game.GetNextChallenge(space.Id));
            }

            // verify that all challenges were used
            CollectionAssert.AreEqual(challenges.Select(x => x.Id).OrderBy(x => x), usedChallenges.Select(x => x.Id).OrderBy(x => x));

            // verify that more challenges can be retrieved
            Assert.IsNotNull(game.GetNextChallenge(space.Id));
        }

        [Test]
        public void GetNextChallengeMultipleSpaceCategories()
        {
            // create a game with some challenges
            var board = new Board();
            Persistence.Connection.Insert(board);
            var game = new Game() { BoardId = board.Id };
            Persistence.Connection.Insert(game);
            var category1 = new ChallengeCategory();
            var category2 = new ChallengeCategory();
            Persistence.Connection.Insert(category1);
            Persistence.Connection.Insert(category2);
            var challenge1 = new Challenge() { ChallengeCategoryId = category1.Id };
            var challenge2 = new Challenge() { ChallengeCategoryId = category1.Id };
            var challenge3 = new Challenge() { ChallengeCategoryId = category2.Id };
            var challenge4 = new Challenge() { ChallengeCategoryId = category2.Id };
            var challenge5 = new Challenge() { ChallengeCategoryId = category2.Id };
            var challenges = new Challenge[] { challenge1, challenge2, challenge3, challenge4, challenge5 };
            Persistence.Connection.InsertAll(challenges);
            var space = new Space() { BoardId = board.Id };
            Persistence.Connection.Insert(space);
            Persistence.Connection.Insert(new SpaceChallengeCategory() { SpaceId = space.Id, ChallengeCategoryId = category1.Id });
            Persistence.Connection.Insert(new SpaceChallengeCategory() { SpaceId = space.Id, ChallengeCategoryId = category2.Id });

            // get some challenges (which ones will be used is indeterminate)
            for (int i = 0; i < 10; i++)
            {
                game.GetNextChallenge(space.Id);
            }
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
