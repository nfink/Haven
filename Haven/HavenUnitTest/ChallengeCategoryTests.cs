using Haven;
using NUnit.Framework;

namespace HavenUnitTest
{
    [TestFixture]
    public class ChallengeCategoryTests
    {
        [Test]
        public void DeleteChallengeCategory()
        {
            var repository = new TestRepository();

            // create a category with challenges
            var category = new ChallengeCategory();
            repository.Add(category);
            repository.Add(new Challenge() { ChallengeCategoryId = category.Id, Question = "Test Question 1" });
            repository.Add(new Challenge() { ChallengeCategoryId = category.Id, Question = "Test Question 2" });
            repository.Add(new SpaceChallengeCategory() { ChallengeCategoryId = category.Id });
            repository.Add(new SpaceChallengeCategory() { ChallengeCategoryId = category.Id });
            repository.Add(new BoardChallengeCategory() { ChallengeCategoryId = category.Id });
            repository.Add(new BoardChallengeCategory() { ChallengeCategoryId = category.Id });

            // delete the category
            category.Delete();

            // verify category is deleted
            Assert.IsEmpty(repository.Find<ChallengeCategory>(x => x.Id == category.Id));

            // verify challenges are deleted
            Assert.IsEmpty(repository.Find<Challenge>(x => x.ChallengeCategoryId == category.Id));

            // verify that space links are removed
            Assert.IsEmpty(repository.Find<SpaceChallengeCategory>(x => x.ChallengeCategoryId == category.Id));

            // verify board links are removed
            Assert.IsEmpty(repository.Find<BoardChallengeCategory>(x => x.ChallengeCategoryId == category.Id));
        }

        [Test]
        public void CloneChallengeCategory()
        {
            var repository = new TestRepository();

            // create a category
            var category = new ChallengeCategory() { Name = "test1", OwnerId = 2 };
            repository.Add(category);

            // clone the category
            var clonedCategory = category.Clone();

            // verify that category was cloned
            Assert.AreNotEqual(category.Id, clonedCategory.Id);
            Assert.AreEqual(category.Name, clonedCategory.Name);
            Assert.AreEqual(category.OwnerId, clonedCategory.OwnerId);
        }
    }
}
