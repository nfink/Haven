using Haven;
using NUnit.Framework;
using System.Linq;

namespace HavenUnitTest
{
    [TestFixture]
    public class ChallengeTests
    {
        [Test]
        public void DeleteChallenge()
        {
            var repository = new TestRepository();

            // add test challenges
            var challenge1 = new Challenge() { Question = "Test Question 1" };
            var challenge2 = new Challenge() { Question = "Test Question 2" };
            repository.Add(challenge1);
            repository.Add(challenge2);

            // add answers
            var answer1a = new ChallengeAnswer() { Answer = "Test Answer 1a", Correct = true, ChallengeId = challenge1.Id };
            var answer1b = new ChallengeAnswer() { Answer = "Test Answer 1b", Correct = false, ChallengeId = challenge1.Id };
            var answer2a = new ChallengeAnswer() { Answer = "Test Answer 2a", Correct = true, ChallengeId = challenge2.Id };
            var answer2b = new ChallengeAnswer() { Answer = "Test Answer 2b", Correct = false, ChallengeId = challenge2.Id };
            repository.AddAll(new ChallengeAnswer[] { answer1a, answer1b, answer2a, answer2b });

            // delete one challenge
            challenge1.Delete();

            // verify challenge and answers are deleted
            Assert.IsEmpty(repository.Find<Challenge>(x => x.Id == challenge1.Id));
            Assert.IsEmpty(repository.Find<ChallengeAnswer>(x => x.Id == answer1a.Id));
            Assert.IsEmpty(repository.Find<ChallengeAnswer>(x => x.Id == answer1b.Id));

            // verify other challenge is not deleted
            Assert.IsNotEmpty(repository.Find<Challenge>(x => x.Id == challenge2.Id));
            Assert.IsNotEmpty(repository.Find<ChallengeAnswer>(x => x.Id == answer2a.Id));
            Assert.IsNotEmpty(repository.Find<ChallengeAnswer>(x => x.Id == answer2b.Id));
        }

        [Test]
        public void CloneChallenge()
        {
            var repository = new TestRepository();

            // create a challenge
            var challenge = new Challenge() { Question = "Test Question 1", ChallengeCategoryId = 2, OwnerId = 3, OpenEnded = true };
            repository.Add(challenge);
            var answer1 = new ChallengeAnswer() { Answer = "Test Answer 1", Correct = true, ChallengeId = challenge.Id };
            var answer2 = new ChallengeAnswer() { Answer = "Test Answer 2", Correct = false, ChallengeId = challenge.Id };
            repository.AddAll(new ChallengeAnswer[] { answer1, answer2 });

            // clone the challenge
            var clonedChallenge = challenge.Clone();

            // verify that challenge and answers were cloned
            Assert.AreNotEqual(challenge.Id, clonedChallenge.Id);
            Assert.AreEqual(challenge.Question, clonedChallenge.Question);
            Assert.AreEqual(challenge.ChallengeCategoryId, clonedChallenge.ChallengeCategoryId);
            Assert.AreEqual(challenge.OwnerId, clonedChallenge.OwnerId);
            Assert.AreEqual(challenge.OpenEnded, clonedChallenge.OpenEnded);
            Assert.AreNotEqual(challenge.Answers.Select(x => x.Id), clonedChallenge.Answers.Select(x => x.Id));
            Assert.AreEqual(challenge.Answers.Select(x => x.Answer), clonedChallenge.Answers.Select(x => x.Answer));
            Assert.AreEqual(challenge.Answers.Select(x => x.Correct), clonedChallenge.Answers.Select(x => x.Correct));
        }

        [Test]
        public void MultipleChoiceAnswer()
        {
            var repository = new TestRepository();

            // create a multiple choice challenge
            var challenge = new Challenge() { Question = "Test Question 1", ChallengeCategoryId = 2, OwnerId = 3 };
            repository.Add(challenge);
            var answer1 = new ChallengeAnswer() { Answer = "Test Answer 1", Correct = true, ChallengeId = challenge.Id };
            var answer2 = new ChallengeAnswer() { Answer = "Test Answer 2", Correct = false, ChallengeId = challenge.Id };
            repository.AddAll(new ChallengeAnswer[] { answer1, answer2 });

            // verify that correct answer is acknowledged as correct
            Assert.True(challenge.CorrectAnswer(answer1.Id));
            Assert.False(challenge.CorrectAnswer(answer2.Id));
        }

        [Test]
        public void OpenEndedAnswer()
        {
            var repository = new TestRepository();

            // create an open ended challenge
            var challenge = new Challenge() { Question = "Test Question 1", ChallengeCategoryId = 2, OwnerId = 3, OpenEnded = true };
            repository.Add(challenge);
            var answer1 = new ChallengeAnswer() { Answer = "Test Answer 1", Correct = true, ChallengeId = challenge.Id };
            var answer2 = new ChallengeAnswer() { Answer = "Test Answer 2", Correct = false, ChallengeId = challenge.Id };
            repository.AddAll(new ChallengeAnswer[] { answer1, answer2 });

            // verify that correct answer is acknowledged as correct
            Assert.True(challenge.CorrectAnswer(answer1.Answer));
            Assert.False(challenge.CorrectAnswer(answer2.Answer));
            Assert.False(challenge.CorrectAnswer(answer1.Answer + "a"));
        }
    }
}
