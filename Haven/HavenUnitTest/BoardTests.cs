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
        [SetUp]
        public void Setup()
        {
            // create necessary tables
            Persistence.Connection.CreateTable<Challenge>();
            Persistence.Connection.CreateTable<ChallengeAnswer>();
            Persistence.Connection.CreateTable<Space>();
            Persistence.Connection.CreateTable<Image>();
            Persistence.Connection.CreateTable<Board>();
        }

        [Test]
        public void Delete()
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
            Assert.IsEmpty(Persistence.Connection.Table<Challenge>().Where(x => x.BoardId == board1.Id));
            Assert.IsEmpty(Persistence.Connection.Table<Space>().Where(x => x.BoardId == board1.Id));

            // verify that other board data is not deleted
            Assert.IsNotEmpty(Persistence.Connection.Table<Board>().Where(x => x.Id == board2.Id));
            Assert.IsNotEmpty(Persistence.Connection.Table<Image>().Where(x => x.Id == board2.ImageId));
            Assert.IsNotEmpty(Persistence.Connection.Table<Challenge>().Where(x => x.BoardId == board2.Id));
            Assert.IsNotEmpty(Persistence.Connection.Table<Space>().Where(x => x.BoardId == board2.Id));
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

        private Board CreateBoardData()
        {
            var image = new Image();
            Persistence.Connection.Insert(image);
            var board = new Board() { ImageId = image.Id };
            Persistence.Connection.Insert(board);
            Persistence.Connection.Insert(new Space() { BoardId = board.Id });
            Persistence.Connection.Insert(new Space() { BoardId = board.Id });
            Persistence.Connection.Insert(new Challenge() { BoardId = board.Id });
            Persistence.Connection.Insert(new Challenge() { BoardId = board.Id });
            return board;
        }
    }
}
