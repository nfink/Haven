using Haven;
using NUnit.Framework;
using System.Linq;

namespace HavenUnitTest
{
    [TestFixture]
    public class SpaceTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            Persistence.Connection.CreateTable<SpaceChallengeCategory>();
            Persistence.Connection.CreateTable<BibleVerse>();
            Persistence.Connection.CreateTable<NameCard>();
            Persistence.Connection.CreateTable<SafeHavenCard>();
            Persistence.Connection.CreateTable<Space>();
        }

        [Test]
        public void DeleteSpace()
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

        [Test]
        public void DeletedSpaceWithCategories()
        {
            // create a space with category links
            var space = new Space();
            Persistence.Connection.Insert(space);
            Persistence.Connection.Insert(new SpaceChallengeCategory() { SpaceId = space.Id, ChallengeCategoryId = 1 });
            Persistence.Connection.Insert(new SpaceChallengeCategory() { SpaceId = space.Id, ChallengeCategoryId = 2 });

            // delete a space
            space.Delete();

            // verify that space is deleted
            Assert.IsEmpty(Persistence.Connection.Table<Space>().Where(x => x.Id == space.Id));

            // verify that category links are deleted
            Assert.IsEmpty(Persistence.Connection.Table<SpaceChallengeCategory>().Where(x => x.SpaceId == space.Id));
        }

        [Test]
        public void CloneSpace()
        {
            // create a space
            var space = new Space() { BackgroundColorId = 1, BoardId = 2, Height = 3, ImageId = 4, Order = 5, TextColorId = 6, Type = SpaceType.TurnAround, Width = 7, X = 8, Y = 9 };
            Persistence.Connection.Insert(space);

            // clone the space
            var clonedSpace = space.Clone();

            // verify that space was cloned
            Assert.AreNotEqual(space.Id, clonedSpace.Id);
            Assert.AreEqual(clonedSpace.BackgroundColorId, space.BackgroundColorId);
            Assert.AreEqual(clonedSpace.BoardId, space.BoardId);
            Assert.AreEqual(clonedSpace.Height, space.Height);
            Assert.AreEqual(clonedSpace.ImageId, space.ImageId);
            Assert.AreEqual(clonedSpace.Name, space.Name);
            Assert.AreEqual(clonedSpace.Order, space.Order);
            Assert.AreEqual(clonedSpace.TextColorId, space.TextColorId);
            Assert.AreEqual(clonedSpace.Type, space.Type);
            Assert.AreEqual(clonedSpace.Width, space.Width);
            Assert.AreEqual(clonedSpace.X, space.X);
            Assert.AreEqual(clonedSpace.Y, space.Y);
        }

        [Test]
        public void CloneRecallSpace()
        {
            // create a space with a recall
            var recall = new BibleVerse() { Text = "test1" };
            Persistence.Connection.Insert(recall);
            var space = new Space() { BibleVerseId = recall.Id, Order = 10 };
            Persistence.Connection.Insert(space);

            // clone the space
            var clonedSpace = space.Clone();

            // verify that space and subobject were cloned
            Assert.AreNotEqual(space.Id, clonedSpace.Id);
            Assert.AreEqual(clonedSpace.Order, space.Order);
            Assert.AreNotEqual(clonedSpace.BibleVerseId, space.BibleVerseId);
            Assert.AreEqual(clonedSpace.BibleVerse.Text, space.BibleVerse.Text);
        }

        [Test]
        public void CloneChallengeSpace()
        {
            // create a space with a name card
            var nameCard = new NameCard() { Name = "test1" };
            Persistence.Connection.Insert(nameCard);
            var space = new Space() { NameCardId = nameCard.Id, Order = 10 };
            Persistence.Connection.Insert(space);

            // clone the space
            var clonedSpace = space.Clone();

            // verify that space and subobject were cloned
            Assert.AreNotEqual(space.Id, clonedSpace.Id);
            Assert.AreEqual(clonedSpace.Order, space.Order);
            Assert.AreNotEqual(clonedSpace.NameCardId, space.NameCardId);
            Assert.AreEqual(clonedSpace.NameCard.Name, space.NameCard.Name);
        }

        [Test]
        public void CloneSafeHavenSpace()
        {
            // create a space with a safe haven card
            var safeHavenCard = new SafeHavenCard() { Name = "test1" };
            Persistence.Connection.Insert(safeHavenCard);
            var space = new Space() { SafeHavenCardId = safeHavenCard.Id, Order = 10 };
            Persistence.Connection.Insert(space);

            // clone the space
            var clonedSpace = space.Clone();

            // verify that space and subobject were cloned
            Assert.AreNotEqual(space.Id, clonedSpace.Id);
            Assert.AreEqual(clonedSpace.Order, space.Order);
            Assert.AreNotEqual(clonedSpace.SafeHavenCardId, space.SafeHavenCardId);
            Assert.AreEqual(clonedSpace.SafeHavenCard.Name, space.SafeHavenCard.Name);
        }

        [Test]
        public void CloneSpaceWithCategories()
        {
            // create a space with category links
            var space = new Space() { Order = 10 };
            Persistence.Connection.Insert(space);
            Persistence.Connection.Insert(new SpaceChallengeCategory() { SpaceId = space.Id, ChallengeCategoryId = 1 });
            Persistence.Connection.Insert(new SpaceChallengeCategory() { SpaceId = space.Id, ChallengeCategoryId = 2 });

            // clone the space
            var clonedSpace = space.Clone();

            // verify that space and category links were cloned
            Assert.AreNotEqual(space.Id, clonedSpace.Id);
            Assert.AreEqual(clonedSpace.Order, space.Order);
            Assert.AreNotEqual(clonedSpace.ChallengeCategories.Select(x => x.Id), space.ChallengeCategories.Select(x => x.Id));
            Assert.AreEqual(clonedSpace.ChallengeCategories.Select(x => x.ChallengeCategoryId), space.ChallengeCategories.Select(x => x.ChallengeCategoryId));
        }
    }
}
