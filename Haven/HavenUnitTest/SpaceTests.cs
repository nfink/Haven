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
    public class SpaceTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            Persistence.Connection.CreateTable<BibleVerse>();
            Persistence.Connection.CreateTable<NameCard>();
            Persistence.Connection.CreateTable<SafeHavenCard>();
            Persistence.Connection.CreateTable<Space>();
        }

        [Test]
        public void Delete()
        {
            // create spaces
            var space1 = new Space();
            var space2 = new Space();
            Persistence.Connection.InsertAll(new Space[] { space1, space2 });

            // delete a space
            space1.Delete();

            // verify that space is deleted
            Assert.IsEmpty(Persistence.Connection.Table<Space>().Where(x => x.Id == space1.Id));

            // verify that other space is not deleted
            Assert.IsNotEmpty(Persistence.Connection.Table<Space>().Where(x => x.Id == space2.Id));
        }

        [Test]
        public void DeleteRecallSpace()
        {
            // create recall spaces
            var recall1 = new BibleVerse();
            var recall2 = new BibleVerse();
            Persistence.Connection.InsertAll(new BibleVerse[] { recall1, recall2 });

            var space1 = new Space() { BibleVerseId = recall1.Id };
            var space2 = new Space() { BibleVerseId = recall2.Id };
            Persistence.Connection.InsertAll(new Space[] { space1, space2 });

            // delete a space
            space1.Delete();

            // verify that space is deleted
            Assert.IsEmpty(Persistence.Connection.Table<Space>().Where(x => x.Id == space1.Id));

            // verify that recall is deleted
            Assert.IsEmpty(Persistence.Connection.Table<BibleVerse>().Where(x => x.Id == space1.BibleVerseId));

            // verify that other space data is not deleted
            Assert.IsNotEmpty(Persistence.Connection.Table<Space>().Where(x => x.Id == space2.Id));
            Assert.IsNotEmpty(Persistence.Connection.Table<BibleVerse>().Where(x => x.Id == space2.BibleVerseId));
        }

        [Test]
        public void DeletedChallengeSpace()
        {
            // create challenge spaces
            var nameCard1 = new NameCard();
            var nameCard2 = new NameCard();
            Persistence.Connection.InsertAll(new NameCard[] { nameCard1, nameCard2 });
            var space1 = new Space() { NameCardId = nameCard1.Id };
            var space2 = new Space() { NameCardId = nameCard2.Id };
            Persistence.Connection.InsertAll(new Space[] { space1, space2 });

            // delete a space
            space1.Delete();
            
            // verify that space is deleted
            Assert.IsEmpty(Persistence.Connection.Table<Space>().Where(x => x.Id == space1.Id));
            
            // verify that name card is deleted
            Assert.IsEmpty(Persistence.Connection.Table<NameCard>().Where(x => x.Id == space1.NameCardId));

            // verify that other space data is not deleted
            Assert.IsNotEmpty(Persistence.Connection.Table<Space>().Where(x => x.Id == space2.Id));
            Assert.IsNotEmpty(Persistence.Connection.Table<NameCard>().Where(x => x.Id == space2.NameCardId));
        }

        [Test]
        public void DeletedSafeHavenSpace()
        {
            // create safe haven spaces
            var safeHavenCard1 = new SafeHavenCard();
            var safeHavenCard2 = new SafeHavenCard();
            Persistence.Connection.InsertAll(new SafeHavenCard[] { safeHavenCard1, safeHavenCard2 });
            var space1 = new Space() { SafeHavenCardId = safeHavenCard1.Id };
            var space2 = new Space() { SafeHavenCardId = safeHavenCard2.Id };
            Persistence.Connection.InsertAll(new Space[] { space1, space2 });

            // delete a space
            space1.Delete();

            // verify that space is deleted
            Assert.IsEmpty(Persistence.Connection.Table<Space>().Where(x => x.Id == space1.Id));

            // verify that safe haven card is deleted
            Assert.IsEmpty(Persistence.Connection.Table<SafeHavenCard>().Where(x => x.Id == space1.SafeHavenCardId));

            // verify that other space data is not deleted
            Assert.IsNotEmpty(Persistence.Connection.Table<Space>().Where(x => x.Id == space2.Id));
            Assert.IsNotEmpty(Persistence.Connection.Table<SafeHavenCard>().Where(x => x.Id == space2.SafeHavenCardId));
        }
    }
}
