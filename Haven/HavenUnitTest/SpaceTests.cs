using Haven;
using NUnit.Framework;
using System.Linq;

namespace HavenUnitTest
{
    [TestFixture]
    public class SpaceTests
    {
        [Test]
        public void DeleteSpace()
        {
            var repository = new TestRepository();

            // create spaces
            var space1 = new Space();
            var space2 = new Space();
            repository.AddAll(new Space[] { space1, space2 });

            // delete a space
            space1.Delete();

            // verify that space is deleted
            Assert.IsEmpty(repository.Find<Space>(x => x.Id == space1.Id));

            // verify that other space is not deleted
            Assert.IsNotEmpty(repository.Find<Space>(x => x.Id == space2.Id));
        }

        [Test]
        public void DeletedChallengeSpace()
        {
            var repository = new TestRepository();

            // create challenge spaces
            var nameCard1 = new NameCard();
            var nameCard2 = new NameCard();
            repository.AddAll(new NameCard[] { nameCard1, nameCard2 });
            var space1 = new Space() { NameCardId = nameCard1.Id };
            var space2 = new Space() { NameCardId = nameCard2.Id };
            repository.AddAll(new Space[] { space1, space2 });

            // delete a space
            space1.Delete();
            
            // verify that space is deleted
            Assert.IsEmpty(repository.Find<Space>(x => x.Id == space1.Id));
            
            // verify that name card is deleted
            Assert.IsEmpty(repository.Find<NameCard>(x => x.Id == space1.NameCardId));

            // verify that other space data is not deleted
            Assert.IsNotEmpty(repository.Find<Space>(x => x.Id == space2.Id));
            Assert.IsNotEmpty(repository.Find<NameCard>(x => x.Id == space2.NameCardId));
        }

        [Test]
        public void DeletedSafeHavenSpace()
        {
            var repository = new TestRepository();

            // create safe haven spaces
            var safeHavenCard1 = new SafeHavenCard();
            var safeHavenCard2 = new SafeHavenCard();
            repository.AddAll(new SafeHavenCard[] { safeHavenCard1, safeHavenCard2 });
            var space1 = new Space() { SafeHavenCardId = safeHavenCard1.Id };
            var space2 = new Space() { SafeHavenCardId = safeHavenCard2.Id };
            repository.AddAll(new Space[] { space1, space2 });

            // delete a space
            space1.Delete();

            // verify that space is deleted
            Assert.IsEmpty(repository.Find<Space>(x => x.Id == space1.Id));

            // verify that safe haven card is deleted
            Assert.IsEmpty(repository.Find<SafeHavenCard>(x => x.Id == space1.SafeHavenCardId));

            // verify that other space data is not deleted
            Assert.IsNotEmpty(repository.Find<Space>(x => x.Id == space2.Id));
            Assert.IsNotEmpty(repository.Find<SafeHavenCard>(x => x.Id == space2.SafeHavenCardId));
        }

        [Test]
        public void DeletedSpaceWithCategories()
        {
            var repository = new TestRepository();

            // create a space with category links
            var space = new Space();
            repository.Add(space);
            repository.Add(new SpaceChallengeCategory() { SpaceId = space.Id, ChallengeCategoryId = 1 });
            repository.Add(new SpaceChallengeCategory() { SpaceId = space.Id, ChallengeCategoryId = 2 });

            // delete a space
            space.Delete();

            // verify that space is deleted
            Assert.IsEmpty(repository.Find<Space>(x => x.Id == space.Id));

            // verify that category links are deleted
            Assert.IsEmpty(repository.Find<SpaceChallengeCategory>(x => x.SpaceId == space.Id));
        }

        [Test]
        public void CloneSpace()
        {
            var repository = new TestRepository();

            // create a space
            var space = new Space() { BackgroundColorId = 1, BoardId = 2, Height = 3, ImageId = 4, Order = 5, TextColorId = 6, Type = SpaceType.TurnAround, Width = 7, X = 8, Y = 9, IconId = 11 };
            repository.Add(space);

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
            Assert.AreEqual(clonedSpace.IconId, space.IconId);
        }

        [Test]
        public void CloneChallengeSpace()
        {
            var repository = new TestRepository();

            // create a space with a name card
            var nameCard = new NameCard() { Name = "test1" };
            repository.Add(nameCard);
            var space = new Space() { NameCardId = nameCard.Id, Order = 10 };
            repository.Add(space);

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
            var repository = new TestRepository();

            // create a space with a safe haven card
            var safeHavenCard = new SafeHavenCard() { Name = "test1" };
            repository.Add(safeHavenCard);
            var space = new Space() { SafeHavenCardId = safeHavenCard.Id, Order = 10 };
            repository.Add(space);

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
            var repository = new TestRepository();

            // create a space with category links
            var space = new Space() { Order = 10 };
            repository.Add(space);
            repository.Add(new SpaceChallengeCategory() { SpaceId = space.Id, ChallengeCategoryId = 1 });
            repository.Add(new SpaceChallengeCategory() { SpaceId = space.Id, ChallengeCategoryId = 2 });

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
