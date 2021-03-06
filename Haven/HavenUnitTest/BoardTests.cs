﻿using Haven;
using NUnit.Framework;
using System;
using System.Linq;

namespace HavenUnitTest
{
    [TestFixture]
    public class BoardTests
    {
        [Test]
        public void DeleteBoard()
        {
            var repository = new TestRepository();

            // create some boards and associated data
            var board1 = CreateBoardData(repository);
            var board2 = CreateBoardData(repository);

            // delete the board
            board1.Delete();
            
            // verify that the board is deleted
            Assert.IsEmpty(repository.Find<Board>(x => x.Id == board1.Id));

            // verify that associated data is deleted
            Assert.IsEmpty(repository.Find<Image>(x => x.Id == board1.ImageId));
            Assert.IsEmpty(board1.Challenges);
            Assert.IsEmpty(board1.Spaces);

            // verify that other board data is not deleted
            Assert.IsNotEmpty(repository.Find<Board>(x => x.Id == board2.Id));
            Assert.IsNotEmpty(repository.Find<Image>(x => x.Id == board2.ImageId));
            Assert.IsNotEmpty(board2.Challenges);
            Assert.IsNotEmpty(board2.Spaces);
        }

        [Test]
        public void DeleteEmptyBoard()
        {
            var repository = new TestRepository();

            // create a board without any associated data
            var board = new Board();
            repository.Add(board);

            // verify that deletion works
            board.Delete();
            Assert.IsEmpty(repository.Find<Board>(x => x.Id == board.Id));
        }

        [Test]
        public void GetNewSpace()
        {
            var repository = new TestRepository();

            // create a board with some spaces
            var board = new Board();
            repository.Add(board);
            var space1 = new Space() { BoardId = board.Id, Order = 10 };
            var space2 = new Space() { BoardId = board.Id, Order = 20 };
            var space3 = new Space() { BoardId = board.Id, Order = 30 };
            var space4 = new Space() { BoardId = board.Id, Order = 40 };
            var space5 = new Space() { BoardId = board.Id, Order = 50 };
            repository.AddAll(new Space[] { space1, space2, space3, space4, space5 });

            // verify that the correct space is returned moving forward
            Assert.AreEqual(space2.Id, board.GetNewSpace(space1.Id, 1, true).Id);
            Assert.AreEqual(space3.Id, board.GetNewSpace(space1.Id, 2, true).Id);
            Assert.AreEqual(space5.Id, board.GetNewSpace(space1.Id, 4, true).Id);

            // verify that the correct space is returned moving forward and wrapping around
            Assert.AreEqual(space1.Id, board.GetNewSpace(space5.Id, 1, true).Id);
            Assert.AreEqual(space3.Id, board.GetNewSpace(space4.Id, 4, true).Id);

            // verify that the correct space is returned moving forward and moving more than the number of spaces
            Assert.AreEqual(space1.Id, board.GetNewSpace(space1.Id, 5, true).Id);
            Assert.AreEqual(space2.Id, board.GetNewSpace(space1.Id, 11, true).Id);

            // verify that the correct space is returned moving backward
            Assert.AreEqual(space4.Id, board.GetNewSpace(space5.Id, 1, false).Id);
            Assert.AreEqual(space3.Id, board.GetNewSpace(space5.Id, 2, false).Id);
            Assert.AreEqual(space1.Id, board.GetNewSpace(space5.Id, 4, false).Id);

            // verify that the correct space is returned moving backward and wrapping around
            Assert.AreEqual(space5.Id, board.GetNewSpace(space1.Id, 1, false).Id);
            Assert.AreEqual(space3.Id, board.GetNewSpace(space2.Id, 4, false).Id);

            // verify that the correct space is returned moving backward and moving more than the number of spaces
            Assert.AreEqual(space1.Id, board.GetNewSpace(space1.Id, 5, false).Id);
            Assert.AreEqual(space5.Id, board.GetNewSpace(space1.Id, 11, false).Id);
        }

        [Test]
        public void GetNewSpaceSingleSpace()
        {
            var repository = new TestRepository();

            // create a board with a single space
            var board = new Board();
            repository.Add(board);
            var space = new Space() { BoardId = board.Id, Order = 10 };
            repository.Add(space);

            // verify that the same space is returned when moving forward
            Assert.AreEqual(space.Id, board.GetNewSpace(space.Id, 1, true).Id);
            Assert.AreEqual(space.Id, board.GetNewSpace(space.Id, 5, true).Id);

            // verify that the same space is returned when moving backward
            Assert.AreEqual(space.Id, board.GetNewSpace(space.Id, 1, false).Id);
            Assert.AreEqual(space.Id, board.GetNewSpace(space.Id, 5, false).Id);
        }

        [Test]
        public void GetNewSpaceNoSpace()
        {
            var repository = new TestRepository();

            // create a board without spaces
            var board = new Board();
            repository.Add(board);

            // verify that an exception is raised when trying to move
            Assert.Catch<Exception>(() => board.GetNewSpace(0, 1, true));
        }

        [Test]
        public void CloneBoard()
        {
            var repository = new TestRepository();

            // create a board and associated data
            var board = CreateBoardData(repository);

            // clone the board
            var clonedBoard = board.Clone();

            // verify that the board and associated objects are cloned
            Assert.AreNotEqual(board.Id, clonedBoard.Id);
            Assert.AreNotEqual(board.ImageId, clonedBoard.ImageId);
            Assert.AreEqual(board.Active, clonedBoard.Active);
            Assert.AreEqual(board.Description, clonedBoard.Description);
            Assert.AreEqual(board.Name, clonedBoard.Name);
            Assert.AreEqual(board.OwnerId, clonedBoard.OwnerId);
            Assert.AreEqual(board.TurnsToEnd, clonedBoard.TurnsToEnd);
            Assert.AreEqual(board.NameCardsToEnd, clonedBoard.NameCardsToEnd);
            Assert.AreEqual(board.SafeHavenCardsToEnd, clonedBoard.SafeHavenCardsToEnd);

            // spaces
            Assert.AreNotEqual(board.Spaces.Select(x => x.Id), clonedBoard.Spaces.Select(x => x.Id));
            Assert.AreEqual(board.Spaces.Select(x => clonedBoard.Id), clonedBoard.Spaces.Select(x => x.BoardId));
            Assert.AreEqual(board.Spaces.Select(x => x.Order), clonedBoard.Spaces.Select(x => x.Order));
            Assert.AreNotEqual(board.Spaces.Select(x => x.ChallengeCategories.Select(y => y.Id)), clonedBoard.Spaces.Select(x => x.ChallengeCategories.Select(y => y.Id)));
            Assert.AreEqual(board.Spaces.Select(x => x.ChallengeCategories.Select(y => repository.Get<ChallengeCategory>(y.ChallengeCategoryId).Name)), clonedBoard.Spaces.Select(x => x.ChallengeCategories.Select(y => repository.Get<ChallengeCategory>(y.ChallengeCategoryId).Name)));
            Assert.AreNotEqual(board.Spaces.Select(x => x.ChallengeCategories.Select(y => repository.Get<ChallengeCategory>(y.ChallengeCategoryId).Id)), clonedBoard.Spaces.Select(x => x.ChallengeCategories.Select(y => repository.Get<ChallengeCategory>(y.ChallengeCategoryId).Id)));

            // challenges
            Assert.AreNotEqual(board.Challenges.Select(x => x.Id), clonedBoard.Challenges.Select(x => x.Id));
            Assert.AreEqual(board.Challenges.Select(x => x.Question), clonedBoard.Challenges.Select(x => x.Question));
            Assert.AreEqual(board.Challenges.Select(x => 0), clonedBoard.Challenges.Select(x => x.OwnerId));
           
            // categories
            var categories = board.Challenges.Select(x => repository.Get<ChallengeCategory>(x.ChallengeCategoryId));
            var clonedCategories = clonedBoard.Challenges.Select(x => repository.Get<ChallengeCategory>(x.ChallengeCategoryId));
            Assert.AreNotEqual(categories.Select(x => x.Id), clonedCategories.Select(x => x.Id));
            Assert.AreEqual(categories.Select(x => x.Name), clonedCategories.Select(x => x.Name));
            Assert.AreEqual(categories.Select(x => 0), clonedCategories.Select(x => x.OwnerId));
        }

        [Test]
        public void CloneEmptyBoard()
        {
            var repository = new TestRepository();

            // create a board without any associated data
            var board = new Board();
            board.Name = "test1";
            repository.Add(board);

            // clone the board
            var clonedBoard = board.Clone();

            // verify that the board is cloned
            Assert.AreNotEqual(board.Id, clonedBoard.Id);
            Assert.AreEqual(board.Name, clonedBoard.Name);
        }

        [Test]
        public void CopyBoard()
        {
            var repository = new TestRepository();

            // create a board and associated data
            var board = CreateBoardData(repository);

            // copy the board
            var copiedBoard = board.Copy();

            // verify that the board and spaces are copied, and challenge links are copied but challenges are the same
            Assert.AreNotEqual(board.Id, copiedBoard.Id);
            Assert.AreEqual(board.ImageId, copiedBoard.ImageId);
            Assert.AreEqual(board.Active, copiedBoard.Active);
            Assert.AreEqual(board.Description, copiedBoard.Description);
            Assert.AreEqual(board.Name, copiedBoard.Name);
            Assert.AreEqual(board.OwnerId, copiedBoard.OwnerId);
            Assert.AreEqual(board.TurnsToEnd, copiedBoard.TurnsToEnd);
            Assert.AreEqual(board.NameCardsToEnd, copiedBoard.NameCardsToEnd);
            Assert.AreEqual(board.SafeHavenCardsToEnd, copiedBoard.SafeHavenCardsToEnd);
            Assert.AreNotEqual(board.Spaces.Select(x => x.Id), copiedBoard.Spaces.Select(x => x.Id));
            Assert.AreEqual(board.Spaces.Select(x => copiedBoard.Id), copiedBoard.Spaces.Select(x => x.BoardId));
            Assert.AreEqual(board.Spaces.Select(x => x.Order), copiedBoard.Spaces.Select(x => x.Order));
            Assert.AreNotEqual(board.Spaces.Select(x => x.ChallengeCategories.Select(y => y.Id)), copiedBoard.Spaces.Select(x => x.ChallengeCategories.Select(y => y.Id)));
            Assert.AreEqual(board.Spaces.Select(x => x.ChallengeCategories.Select(y => repository.Get<ChallengeCategory>(y.ChallengeCategoryId).Id)), copiedBoard.Spaces.Select(x => x.ChallengeCategories.Select(y => repository.Get<ChallengeCategory>(y.ChallengeCategoryId).Id)));
            Assert.AreEqual(board.Challenges.Select(x => x.Id), copiedBoard.Challenges.Select(x => x.Id));
        }

        [Test]
        public void CopyEmptyBoard()
        {
            var repository = new TestRepository();

            // create a board without any associated data
            var board = new Board();
            board.Name = "test1";
            repository.Add(board);

            // clone the board
            var copiedBoard = board.Copy();

            // verify that the board is copied
            Assert.AreNotEqual(board.Id, copiedBoard.Id);
            Assert.AreEqual(board.Name, copiedBoard.Name);
        }

        private Board CreateBoardData(TestRepository repository)
        {
            var image = new Image() { Filename = "test.jpg" };
            repository.Add(image);
            var board = new Board() { ImageId = image.Id, Active = true, Description = "test1", Name = "test3", OwnerId = 4, TurnsToEnd = 50, NameCardsToEnd = 10, SafeHavenCardsToEnd = 5 };
            repository.Add(board);
            var space1 = new Space() { BoardId = board.Id, Order = 10 };
            var space2 = new Space() { BoardId = board.Id, Order = 20 };
            var space3 = new Space() { BoardId = board.Id, Order = 30 };
            repository.AddAll(new Space[] { space1, space2, space3 });
            var category1 = new ChallengeCategory() { Name = "test1", OwnerId = 2 };
            var category2 = new ChallengeCategory() { Name = "test3", OwnerId = 4 };
            var category3 = new ChallengeCategory() { Name = "test4", OwnerId = 4 };
            var categories = new ChallengeCategory[] { category1, category2, category3 };
            repository.AddAll(categories);
            repository.Add(new SpaceChallengeCategory() { SpaceId = space1.Id, ChallengeCategoryId = category1.Id });
            repository.Add(new SpaceChallengeCategory() { SpaceId = space2.Id, ChallengeCategoryId = category3.Id });
            var challenge1 = new Challenge() { Question = "test1", ChallengeCategoryId = category1.Id, OwnerId = 3 };
            var challenge2 = new Challenge() { Question = "test2", ChallengeCategoryId = category2.Id, OwnerId = 4 };
            repository.AddAll(new Challenge[] { challenge1, challenge2 });
            repository.Add(new BoardChallengeCategory() { BoardId = board.Id, ChallengeCategoryId = category1.Id });
            repository.Add(new BoardChallengeCategory() { BoardId = board.Id, ChallengeCategoryId = category2.Id });
            return board;
        }
    }
}
