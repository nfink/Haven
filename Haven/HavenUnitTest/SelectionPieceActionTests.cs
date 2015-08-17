using Haven;
using NUnit.Framework;
using System.Linq;

namespace HavenUnitTest
{
    [TestFixture]
    public class SelectionPieceActionTests
    {
        [Test]
        public void SelectPiece()
        {
            var repository = new TestRepository();

            var game = SetupGame(repository);

            // select a piece
            var piece = repository.FindAll<Piece>().First();
            var color = repository.FindAll<Color>().First();
            var player = game.Players.First();
            var action = player.Actions.Where(x => x.Type == ActionType.SelectPiece).Single();
            action.PerformAction(string.Format("{{'PieceId':'{0}','ColorId':'{1}'}}", piece.Id, color.Id));

            // verify that piece was selected
            player = repository.Get<Player>(player.Id);
            Assert.AreEqual(piece.Id, player.ColorId);
            Assert.AreEqual(color.Id, player.PieceId);
            Assert.IsEmpty(player.Actions.Where(x => x.Type == ActionType.SelectPiece));
            var message = player.Messages.Single();
            Assert.AreEqual(string.Format("{0} {1} piece selected.", piece.Name, color.Name), message.Text);
        }

        [Test]
        public void SelectSamePiece()
        {
            var repository = new TestRepository();

            var game = SetupGame(repository);

            // select a piece
            var piece = repository.FindAll<Piece>().First();
            var color = repository.FindAll<Color>().First();
            var player1 = game.Players.First();
            var action1 = player1.Actions.Where(x => x.Type == ActionType.SelectPiece).Single();
            action1.PerformAction(string.Format("{{'PieceId':'{0}','ColorId':'{1}'}}", piece.Id, color.Id));

            // select the same combo of piece/color with another player
            var player2 = game.Players.Where(x => x.Actions.Where(y => y.Type == ActionType.SelectPiece).Count() > 0).First();
            var action2 = player2.Actions.Where(x => x.Type == ActionType.SelectPiece).Single();
            action2.PerformAction(string.Format("{{'PieceId':'{0}','ColorId':'{1}'}}", piece.Id, color.Id));

            // verify that no piece was selected
            player2 = repository.Get<Player>(player2.Id);
            Assert.AreEqual(0, player2.ColorId);
            Assert.AreEqual(0, player2.PieceId);
            Assert.IsNotEmpty(player2.Actions.Where(x => x.Type == ActionType.SelectPiece));
            var message = player2.Messages.Single();
            Assert.AreEqual("Another player has the same piece. Please choose another picture and/or color.", message.Text);
        }

        private Game SetupGame(IRepository repository)
        {
            var board = new Board();
            repository.Add(board);
            repository.Add(new Space() { BoardId = board.Id });
            var game = new Game();
            game.Repository = repository;
            game.Create(board.Id, 2);
            repository.Add(new Piece() { Name = "testpiece1" });
            repository.Add(new Piece() { Name = "testpiece2" });
            repository.Add(new Piece() { Name = "testpiece3" });
            repository.Add(new Color() { Name = "testcolor1" });
            repository.Add(new Color() { Name = "testcolor2" });
            repository.Add(new Color() { Name = "testcolor3" });
            return game;
        }
    }
}
