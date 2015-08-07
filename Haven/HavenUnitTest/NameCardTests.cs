using Haven;
using NUnit.Framework;

namespace HavenUnitTest
{
    [TestFixture]
    public class NameCardTests
    {
        [Test]
        public void CloneNameCard()
        {
            var repository = new TestRepository();

            // create a name card
            var image = new Image() { Filename = "test1" };
            repository.Add<Image>(image);
            var nameCard = new NameCard() { Name = "test1", Details = "test2", ImageId = image.Id };
            repository.Add<NameCard>(nameCard);

            // clone the name card
            var clonedCard = nameCard.Clone();

            // verify that the name card was cloned
            Assert.AreNotEqual(nameCard.Id, clonedCard.Id);
            Assert.AreEqual(nameCard.Name, clonedCard.Name);
            Assert.AreEqual(nameCard.Details, clonedCard.Details);
            Assert.AreNotEqual(nameCard.ImageId, clonedCard.ImageId);
        }
    }
}