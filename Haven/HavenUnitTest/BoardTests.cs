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
    public class BoardTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            // create necessary tables
            Persistence.Connection.CreateTable<Challenge>();
            Persistence.Connection.CreateTable<ChallengeAnswer>();
            Persistence.Connection.CreateTable<BoardChallenge>();
            Persistence.Connection.CreateTable<Space>();
            Persistence.Connection.CreateTable<Image>();
            Persistence.Connection.CreateTable<Board>();
        }

        [Test]
        public void DeleteBoard()
        {
            // create some boards and associated data
            var board1 = CreateBoardData();
            var board2 = CreateBoardData();

            // delete the board
            board1.Delete();
            
            // verify that the board is deleted
            Assert.IsEmpty(Persistence.Connection.Table<Board>().Where(x => x.Id == board1.Id));

            // verify that associated data is deleted
            Assert.IsEmpty(Persistence.Connection.Table<Image>().Where(x => x.Id == board1.ImageId));
            Assert.IsEmpty(board1.Challenges);
            Assert.IsEmpty(board1.Spaces);

            // verify that other board data is not deleted
            Assert.IsNotEmpty(Persistence.Connection.Table<Board>().Where(x => x.Id == board2.Id));
            Assert.IsNotEmpty(Persistence.Connection.Table<Image>().Where(x => x.Id == board2.ImageId));
            Assert.IsNotEmpty(board2.Challenges);
            Assert.IsNotEmpty(board2.Spaces);
        }

        [Test]
        public void DeleteEmptyBoard()
        {
            // create a board without any associated data
            var board = new Board();
            Persistence.Connection.Insert(board);

            // verify that deletion works
            board.Delete();
            Assert.IsEmpty(Persistence.Connection.Table<Board>().Where(x => x.Id == board.Id));
        }

        [Test]
        public void GetNewSpace()
        {
            // create a board with some spaces
            var board = new Board();
            Persistence.Connection.Insert(board);
            var space1 = new Space() { BoardId = board.Id, Order = 10 };
            var space2 = new Space() { BoardId = board.Id, Order = 20 };
            var space3 = new Space() { BoardId = board.Id, Order = 30 };
            var space4 = new Space() { BoardId = board.Id, Order = 40 };
            var space5 = new Space() { BoardId = board.Id, Order = 50 };
            Persistence.Connection.InsertAll(new Space[] { space1, space2, space3, space4, space5 });

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
            // create a board with a single space
            var board = new Board();
            Persistence.Connection.Insert(board);
            var space = new Space() { BoardId = board.Id, Order = 10 };
            Persistence.Connection.Insert(space);

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
            // create a board without spaces
            var board = new Board();
            Persistence.Connection.Insert(board);

            // verify that an exception is raised when trying to move
            Assert.Catch<Exception>(() => board.GetNewSpace(0, 1, true));
        }

        private Board CreateBoardData()
        {
            var image = new Image();
            Persistence.Connection.Insert(image);
            var board = new Board() { ImageId = image.Id };
            Persistence.Connection.Insert(board);
            Persistence.Connection.Insert(new Space() { BoardId = board.Id });
            Persistence.Connection.Insert(new Space() { BoardId = board.Id });
            var challenge1 = new Challenge();
            var challenge2 = new Challenge();
            Persistence.Connection.InsertAll(new Challenge[] { challenge1, challenge2 });
            Persistence.Connection.Insert(new BoardChallenge() { BoardId = board.Id, ChallengeId = challenge1.Id });
            Persistence.Connection.Insert(new BoardChallenge() { BoardId = board.Id, ChallengeId = challenge2.Id });
            return board;
        }
    }
}
