using Haven;
using NUnit.Framework;
using System.Linq;

namespace HavenUnitTest
{
    [TestFixture]
    public class PlayerTests
    {
        [Test]
        public void Password()
        {
            var repository = new TestRepository();

            // create a player with a password
            var player = new Player();
            repository.Add(player);
            player.SetPassword("testpass123");

            // verify that a correct password is verified
            Assert.True(player.VerifyPassword("testpass123"));

            // verify that an incorrect password is not verified
            Assert.False(player.VerifyPassword("wrongpassword"));

            // verify that an empty password is not verified
            Assert.False(player.VerifyPassword(""));

            // verify that a null password is not verified
            Assert.False(player.VerifyPassword(null));
        }

        [Test]
        public void DeletePlayer()
        {
            var repository = new TestRepository();

            // create players with actions, cards, and messages
            var player1 = CreatePlayerData(repository);
            var player2 = CreatePlayerData(repository);
            var player3 = CreatePlayerData(repository);

            // delete a player
            player1.Delete();

            // verify all related data is deleted
            VerifyPlayerDataDeleted(player1, repository);

            // verify that other player data was not deleted
            Assert.IsNotEmpty(repository.Find<Haven.Action>(x => x.OwnerId == player2.Id));
            Assert.IsNotEmpty(repository.Find<PlayerNameCard>(x => x.PlayerId == player2.Id));
            Assert.IsNotEmpty(repository.Find<PlayerSafeHavenCard>(x => x.PlayerId == player2.Id));
            Assert.IsNotEmpty(repository.Find<Message>(x => x.PlayerId == player2.Id));
        }

        [Test]
        public void DeleteOnlyPlayer()
        {
            var repository = new TestRepository();

            // create a single player
            var player = CreatePlayerData(repository);

            // delete the player
            player.Delete();

            // verify that delete works when there are no other players (especially concerned about updating NextPlayerId)
            VerifyPlayerDataDeleted(player, repository);
        }

        private void VerifyPlayerDataDeleted(Player player, IRepository repository)
        {
            // verify that player is deleted
            Assert.IsEmpty(repository.Find<Player>(x => x.Id == player.Id));

            // verify that actions, cards, and messages are deleted
            Assert.IsEmpty(repository.Find<Haven.Action>(x => x.OwnerId == player.Id));
            Assert.IsEmpty(repository.Find<PlayerNameCard>(x => x.PlayerId == player.Id));
            Assert.IsEmpty(repository.Find<PlayerSafeHavenCard>(x => x.PlayerId == player.Id));
            Assert.IsEmpty(repository.Find<Message>(x => x.PlayerId == player.Id));
        }

        [Test]
        public void RecentMessages()
        {
            var repository = new TestRepository();

            // create a player with some messages
            var player = new Player();
            repository.Add(player);
            var message1 = new Message() { PlayerId = player.Id };
            var message2 = new Message() { PlayerId = player.Id };
            var message3 = new Message() { PlayerId = player.Id };
            var message4 = new Message() { PlayerId = player.Id };
            var message5 = new Message() { PlayerId = player.Id };
            repository.Add(message1);
            repository.Add(message2);
            repository.Add(message3);
            repository.Add(message4);
            repository.Add(message5);

            // verify that the correct messages are returned and in the correct order
            var expectedMessages = new Message[] { message5, message4, message3 }.Select(x => x.Id);
            var recentMessages = player.RecentMessages(3).Select(x => x.Id);
            Assert.AreEqual(expectedMessages, recentMessages);

            // add more messages
            var message6 = new Message() { PlayerId = player.Id };
            var message7 = new Message() { PlayerId = player.Id };
            repository.Add(message6);
            repository.Add(message7);

            // verify that the correct messages are returned and in the correct order
            expectedMessages = new Message[] { message7, message6, message5 }.Select(x => x.Id);
            recentMessages = player.RecentMessages(3).Select(x => x.Id);
            Assert.AreEqual(expectedMessages, recentMessages);
        }

        private Player CreatePlayerData(IRepository repository)
        {
            var player = new Player();
            repository.Add(player);
            repository.Add(new Haven.Action() { OwnerId = player.Id });
            repository.Add(new Haven.Action() { OwnerId = player.Id });
            repository.Add(new NameCard() { Id = 1 });
            repository.Add(new NameCard() { Id = 2 });
            repository.Add(new PlayerNameCard() { PlayerId = player.Id, NameCardId = 1 });
            repository.Add(new PlayerNameCard() { PlayerId = player.Id, NameCardId = 2 });
            repository.Add(new SafeHavenCard() { Id = 1 });
            repository.Add(new SafeHavenCard() { Id = 2 });
            repository.Add(new PlayerSafeHavenCard() { PlayerId = player.Id, SafeHavenCardId = 1 });
            repository.Add(new PlayerSafeHavenCard() { PlayerId = player.Id, SafeHavenCardId = 2 });
            repository.Add(new Message() { PlayerId = player.Id });
            repository.Add(new Message() { PlayerId = player.Id });
            return player;
        }

    }
}
