using Haven;
using NUnit.Framework;

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
            Persistence.Connection.CreateTable<SpaceChallengeCategory>();
            Persistence.Connection.CreateTable<BoardChallengeCategory>();
        }

        [Test]
        public void DeleteChallengeCategory()
        {
            // create a category with challenges
            var category = new ChallengeCategory();
            Persistence.Connection.Insert(category);
            Persistence.Connection.Insert(new Challenge() { ChallengeCategoryId = category.Id, Question = "Test Question 1" });
            Persistence.Connection.Insert(new Challenge() { ChallengeCategoryId = category.Id, Question = "Test Question 2" });
            Persistence.Connection.Insert(new SpaceChallengeCategory() { ChallengeCategoryId = category.Id });
            Persistence.Connection.Insert(new SpaceChallengeCategory() { ChallengeCategoryId = category.Id });
            Persistence.Connection.Insert(new BoardChallengeCategory() { ChallengeCategoryId = category.Id });
            Persistence.Connection.Insert(new BoardChallengeCategory() { ChallengeCategoryId = category.Id });

            // delete the category
            category.Delete();

            // verify category is deleted
            Assert.IsEmpty(Persistence.Connection.Table<ChallengeCategory>().Where(x => x.Id == category.Id));

            // verify challenges are deleted
            Assert.IsEmpty(Persistence.Connection.Table<Challenge>().Where(x => x.ChallengeCategoryId == category.Id));

            // verify that space links are removed
            Assert.IsEmpty(Persistence.Connection.Table<SpaceChallengeCategory>().Where(x => x.ChallengeCategoryId == category.Id));

            // verify board links are removed
            Assert.IsEmpty(Persistence.Connection.Table<BoardChallengeCategory>().Where(x => x.ChallengeCategoryId == category.Id));
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
