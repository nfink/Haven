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
    public class PlayerTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            // add needed tables
            Persistence.Connection.CreateTable<Haven.Action>();
            Persistence.Connection.CreateTable<PlayerNameCard>();
            Persistence.Connection.CreateTable<PlayerSafeHavenCard>();
            Persistence.Connection.CreateTable<Message>();
            Persistence.Connection.CreateTable<Player>();
        }

        [Test]
        public void Password()
        {
            // create a player with a password
            var player = new Player();
            Persistence.Connection.Insert(player);
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
        public void Delete()
        {
            // create players with actions, cards, and messages
            var player1 = CreatePlayerData();
            var player2 = CreatePlayerData();
            var player3 = CreatePlayerData();

            // set up turns links between players
            player1.NextPlayerId = player2.Id;
            player2.NextPlayerId = player3.Id;
            player3.NextPlayerId = player1.Id;
            Persistence.Connection.UpdateAll(new Player[] { player1, player2, player3 });


            // delete a player
            player1.Delete();

            // verify all related data is deleted
            VerifyPlayerDataDeleted(player1);

            // verify that the linked list of players is correct
            player2 = Persistence.Connection.Get<Player>(player2.Id);
            player3 = Persistence.Connection.Get<Player>(player3.Id);
            Assert.AreEqual(player3.Id, player2.NextPlayerId);
            Assert.AreEqual(player2.Id, player3.NextPlayerId);

            // verify that other player data was not deleted
            Assert.IsNotEmpty(Persistence.Connection.Table<Haven.Action>().Where(x => x.OwnerId == player2.Id));
            Assert.IsNotEmpty(Persistence.Connection.Table<PlayerNameCard>().Where(x => x.PlayerId == player2.Id));
            Assert.IsNotEmpty(Persistence.Connection.Table<PlayerSafeHavenCard>().Where(x => x.PlayerId == player2.Id));
            Assert.IsNotEmpty(Persistence.Connection.Table<Message>().Where(x => x.PlayerId == player2.Id));
        }

        [Test]
        public void DeleteOnlyPlayer()
        {
            // create a single player
            var player = CreatePlayerData();

            // delete the player
            player.Delete();

            // verify that delete works when there are no other players (especially concerned about updating NextPlayerId)
            VerifyPlayerDataDeleted(player);
        }

        private void VerifyPlayerDataDeleted(Player player)
        {
            // verify that player is deleted
            Assert.IsEmpty(Persistence.Connection.Table<Player>().Where(x => x.Id == player.Id));

            // verify that actions, cards, and messages are deleted
            Assert.IsEmpty(Persistence.Connection.Table<Haven.Action>().Where(x => x.OwnerId == player.Id));
            Assert.IsEmpty(Persistence.Connection.Table<PlayerNameCard>().Where(x => x.PlayerId == player.Id));
            Assert.IsEmpty(Persistence.Connection.Table<PlayerSafeHavenCard>().Where(x => x.PlayerId == player.Id));
            Assert.IsEmpty(Persistence.Connection.Table<Message>().Where(x => x.PlayerId == player.Id));
        }

        [Test]
        public void RecentMessages()
        {
            // create a player with some messages
            var player = new Player();
            Persistence.Connection.Insert(player);
            var message1 = new Message() { PlayerId = player.Id };
            var message2 = new Message() { PlayerId = player.Id };
            var message3 = new Message() { PlayerId = player.Id };
            var message4 = new Message() { PlayerId = player.Id };
            var message5 = new Message() { PlayerId = player.Id };
            Persistence.Connection.Insert(message1);
            Persistence.Connection.Insert(message2);
            Persistence.Connection.Insert(message3);
            Persistence.Connection.Insert(message4);
            Persistence.Connection.Insert(message5);

            // verify that the correct messages are returned and in the correct order
            var expectedMessages = new Message[] { message5, message4, message3 }.Select(x => x.Id);
            var recentMessages = player.RecentMessages(3).Select(x => x.Id);
            Assert.AreEqual(expectedMessages, recentMessages);

            // add more messages
            var message6 = new Message() { PlayerId = player.Id };
            var message7 = new Message() { PlayerId = player.Id };
            Persistence.Connection.Insert(message6);
            Persistence.Connection.Insert(message7);

            // verify that the correct messages are returned and in the correct order
            expectedMessages = new Message[] { message7, message6, message5 }.Select(x => x.Id);
            recentMessages = player.RecentMessages(3).Select(x => x.Id);
            Assert.AreEqual(expectedMessages, recentMessages);
        }

        private Player CreatePlayerData()
        {
            var player = new Player();
            Persistence.Connection.Insert(player);
            Persistence.Connection.Insert(new Haven.Action() { OwnerId = player.Id });
            Persistence.Connection.Insert(new Haven.Action() { OwnerId = player.Id });
            Persistence.Connection.Insert(new PlayerNameCard() { PlayerId = player.Id });
            Persistence.Connection.Insert(new PlayerNameCard() { PlayerId = player.Id });
            Persistence.Connection.Insert(new PlayerSafeHavenCard() { PlayerId = player.Id });
            Persistence.Connection.Insert(new PlayerSafeHavenCard() { PlayerId = player.Id });
            Persistence.Connection.Insert(new Message() { PlayerId = player.Id });
            Persistence.Connection.Insert(new Message() { PlayerId = player.Id });
            return player;
        }

    }
}
