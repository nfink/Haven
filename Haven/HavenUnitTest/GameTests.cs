using Haven;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace HavenUnitTest
{
    [TestFixture]
    public class GameTests
    {
        [Test]
        public void DeleteGame()
        {
            var repository = new TestRepository();

            // create games with players and used challenges
            var game1 = CreateGameData(repository);
            var game2 = CreateGameData(repository);

            // delete a game
            game1.Delete();

            // verify that the game is deleted
            Assert.IsEmpty(repository.Find<Game>(x => x.Id == game1.Id));

            // verify that players are deleted
            Assert.IsEmpty(repository.Find<Player>(x => x.GameId == game1.Id));

            // verify that challenges are not marked as used
            Assert.IsEmpty(repository.Find<UsedChallenge>(x => x.GameId == game1.Id));

            // verify that data from other game is not deleted
            Assert.IsNotEmpty(repository.Find<Game>(x => x.Id == game2.Id));
            Assert.IsNotEmpty(repository.Find<Player>(x => x.GameId == game2.Id));
            Assert.IsNotEmpty(repository.Find<UsedChallenge>(x => x.GameId == game2.Id));
        }

        [Test]
        public void DeleteEmptyGame()
        {
            var repository = new TestRepository();

            // create a game without players or used challenges
            var game = new Game();
            repository.Add(game);

            // delete the game
            game.Delete();

            // verify that deletion works
            Assert.IsEmpty(repository.Find<Game>(x => x.Id == game.Id));
        }

        [Test]
        public void GetNextChallengeBoardCategory()
        {
            var repository = new TestRepository();

            // create a game with some challenges
            var board = new Board();
            repository.Add(board);
            var game = new Game() { BoardId = board.Id };
            repository.Add(game);
            var category = new ChallengeCategory();
            repository.Add(category);
            var challenge1 = new Challenge() { ChallengeCategoryId = category.Id };
            var challenge2 = new Challenge() { ChallengeCategoryId = category.Id };
            var challenge3 = new Challenge() { ChallengeCategoryId = category.Id };
            var challenges = new Challenge[] { challenge1, challenge2, challenge3 };
            repository.AddAll(challenges);
            repository.Add(new BoardChallengeCategory() { BoardId = board.Id, ChallengeCategoryId = category.Id });

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
            var repository = new TestRepository();

            // create a game with some challenges
            var board = new Board();
            repository.Add(board);
            var game = new Game() { BoardId = board.Id };
            repository.Add(game);
            var category1 = new ChallengeCategory();
            var category2 = new ChallengeCategory();
            repository.Add(category1);
            repository.Add(category2);
            var challenge1 = new Challenge() { ChallengeCategoryId = category1.Id };
            var challenge2 = new Challenge() { ChallengeCategoryId = category1.Id };
            var challenge3 = new Challenge() { ChallengeCategoryId = category2.Id };
            var challenge4 = new Challenge() { ChallengeCategoryId = category2.Id };
            var challenge5 = new Challenge() { ChallengeCategoryId = category2.Id };
            var challenges = new Challenge[] { challenge1, challenge2, challenge3, challenge4, challenge5 };
            repository.AddAll(challenges);
            repository.Add(new BoardChallengeCategory() { BoardId = board.Id, ChallengeCategoryId = category1.Id });
            repository.Add(new BoardChallengeCategory() { BoardId = board.Id, ChallengeCategoryId = category2.Id });

            // get some challenges (which ones will be used is indeterminate)
            for (int i = 0; i < 10; i++)
            {
                game.GetNextChallenge(0);
            }
        }

        [Test]
        public void GetNextChallengeSpaceCategory()
        {
            var repository = new TestRepository();

            // create a game with some challenges
            var board = new Board();
            repository.Add(board);
            var game = new Game() { BoardId = board.Id };
            repository.Add(game);
            var category = new ChallengeCategory();
            repository.Add(category);
            var challenge1 = new Challenge() { ChallengeCategoryId = category.Id };
            var challenge2 = new Challenge() { ChallengeCategoryId = category.Id };
            var challenge3 = new Challenge() { ChallengeCategoryId = category.Id };
            var challenges = new Challenge[] { challenge1, challenge2, challenge3 };
            repository.AddAll(challenges);
            var space = new Space() { BoardId = board.Id };
            repository.Add(space);
            repository.Add(new SpaceChallengeCategory() { SpaceId=space.Id, ChallengeCategoryId = category.Id });

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
            var repository = new TestRepository();

            // create a game with some challenges
            var board = new Board();
            repository.Add(board);
            var game = new Game() { BoardId = board.Id };
            repository.Add(game);
            var category1 = new ChallengeCategory();
            var category2 = new ChallengeCategory();
            repository.Add(category1);
            repository.Add(category2);
            var challenge1 = new Challenge() { ChallengeCategoryId = category1.Id };
            var challenge2 = new Challenge() { ChallengeCategoryId = category1.Id };
            var challenge3 = new Challenge() { ChallengeCategoryId = category2.Id };
            var challenge4 = new Challenge() { ChallengeCategoryId = category2.Id };
            var challenge5 = new Challenge() { ChallengeCategoryId = category2.Id };
            var challenges = new Challenge[] { challenge1, challenge2, challenge3, challenge4, challenge5 };
            repository.AddAll(challenges);
            var space = new Space() { BoardId = board.Id };
            repository.Add(space);
            repository.Add(new SpaceChallengeCategory() { SpaceId = space.Id, ChallengeCategoryId = category1.Id });
            repository.Add(new SpaceChallengeCategory() { SpaceId = space.Id, ChallengeCategoryId = category2.Id });

            // get some challenges (which ones will be used is indeterminate)
            for (int i = 0; i < 10; i++)
            {
                game.GetNextChallenge(space.Id);
            }
        }


        private Game CreateGameData(TestRepository repository)
        {
            // create game
            var board = new Board();
            repository.Add(board);
            repository.Add(new Space() { BoardId = board.Id });
            var game = new Game() { BoardId = board.Id };
            repository.Add(game);

            // add challenges
            var challenge1 = new Challenge();
            var challenge2 = new Challenge();
            var challenge3 = new Challenge();
            var challenge4 = new Challenge();
            repository.AddAll(new Challenge[] { challenge1, challenge2, challenge3, challenge4 });
            
            // mark some challenges as used
            repository.Add(new UsedChallenge() { GameId = game.Id, ChallengeId = challenge1.Id });
            repository.Add(new UsedChallenge() { GameId = game.Id, ChallengeId = challenge2.Id });

            // add players
            game.AddPlayer();
            game.AddPlayer();
            game.AddPlayer();

            return game;
        }
    }
}
