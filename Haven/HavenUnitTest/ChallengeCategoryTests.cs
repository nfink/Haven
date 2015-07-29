using Haven;
using NUnit.Framework;
using System.Linq;

namespace HavenUnitTest
{
    [TestFixture]
    public class ChallengeCategoryTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            // create tables
            Persistence.Connection.CreateTable<Challenge>();
            Persistence.Connection.CreateTable<ChallengeCategory>();
        }

        [Test]
        public void DeleteChallengeCategory()
        {
            // create a category with challenges
            var category = new ChallengeCategory();
            Persistence.Connection.Insert(category);
            var challenge1 = new Challenge() { ChallengeCategoryId = category.Id, Question = "Test Question 1" };
            var challenge2 = new Challenge() { ChallengeCategoryId = category.Id, Question = "Test Question 2" };
            Persistence.Connection.Insert(challenge1);
            Persistence.Connection.Insert(challenge2);
         
            // delete the category
            category.Delete();

            // verify category is deleted
            Assert.IsEmpty(Persistence.Connection.Table<ChallengeCategory>().Where(x => x.Id == category.Id));

            // verify challenges have their category set to default
            Assert.AreEqual(0, Persistence.Connection.Get<Challenge>(challenge1.Id).ChallengeCategoryId);
            Assert.AreEqual(0, Persistence.Connection.Get<Challenge>(challenge2.Id).ChallengeCategoryId);
        }

        [Test]
        public void CloneChallengeCategory()
        {
            // create a category
            var category = new ChallengeCategory() { Name = "test1", OwnerId = 2 };
            Persistence.Connection.Insert(category);

            // clone the category
            var clonedCategory = category.Clone();

            // verify that category was cloned
            Assert.AreNotEqual(category.Id, clonedCategory.Id);
            Assert.AreEqual(category.Name, clonedCategory.Name);
            Assert.AreEqual(category.OwnerId, clonedCategory.OwnerId);
        }
    }
}
