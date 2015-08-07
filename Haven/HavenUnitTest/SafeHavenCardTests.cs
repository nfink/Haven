using Haven;
using NUnit.Framework;

namespace HavenUnitTest
{
    [TestFixture]
    public class SafeHavenCardTests
    {
        [Test]
        public void CloneSafeHavenCard()
        {
            var repository = new TestRepository();

            // create a safe haven card
            var image = new Image() { Filename = "test1" };
            repository.Add(image);
            var safeHavenCard = new SafeHavenCard() { Name = "test1", Details = "test2", ImageId = image.Id };
            repository.Add(safeHavenCard);

            // clone the safe haven card
            var clonedCard = safeHavenCard.Clone();

            // verify that the safe haven card was cloned
            Assert.AreNotEqual(safeHavenCard.Id, clonedCard.Id);
            Assert.AreEqual(safeHavenCard.Name, clonedCard.Name);
            Assert.AreEqual(safeHavenCard.Details, clonedCard.Details);
            Assert.AreNotEqual(safeHavenCard.ImageId, clonedCard.ImageId);
        }
    }
}