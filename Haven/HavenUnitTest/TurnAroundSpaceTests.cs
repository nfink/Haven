using Haven;
using NUnit.Framework;
using System.Linq;

namespace HavenUnitTest
{
    [TestFixture]
    public class TurnAroundSpaceTests
    {
        [Test]
        public void TurnAround()
        {
            // set up data
            var repository = new TestRepository();

            var board = new Board();
            repository.Add(board);
            var space = new Space() { Type = SpaceType.TurnAround, BoardId = board.Id };
            repository.Add(space);
            var game = new Game();
            game.Repository = repository;
            game.Create(board.Id, 2);

            // trigger onland
            var player = game.Players.First();
            space.OnLand(player);

            // verify that player was turned around and received the correct message
            player = repository.Get<Player>(player.Id);
            Assert.AreEqual(false, player.MovementDirection);
            var message = player.Messages.Single();
            Assert.AreEqual("Turned around.", message.Text);
        }
    }
}
