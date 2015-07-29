using Haven;
using NUnit.Framework;

namespace HavenUnitTest
{
    [TestFixture]
    public class NameCardTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            Persistence.Connection.CreateTable<NameCard>();
        }

        [Test]
        public void CloneNameCard()
        {
            // create a name card
            var nameCard = new NameCard() { Name = "test1", Details = "test2", ImageId = 3 };
            Persistence.Connection.Insert(nameCard);

            // clone the name card
            var clonedCard = nameCard.Clone();

            // verify that the name card was cloned
            Assert.AreNotEqual(nameCard.Id, clonedCard.Id);
            Assert.AreEqual(nameCard.Name, clonedCard.Name);
            Assert.AreEqual(nameCard.Details, clonedCard.Details);
            Assert.AreEqual(nameCard.ImageId, clonedCard.ImageId);
        }
    }
}