using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Haven;
using Haven.Data;

namespace HavenUnitTest
{
    [TestFixture]
    public class ChallengeTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            // create tables
            Persistence.Connection.CreateTable<Challenge>();
            Persistence.Connection.CreateTable<ChallengeAnswer>();
        }

        [Test]
        public void Delete()
        {
            // add test challenges
            var challenge1 = new Challenge() { Question = "Test Question 1" };
            var challenge2 = new Challenge() { Question = "Test Question 2" };
            Persistence.Connection.Insert(challenge1);
            Persistence.Connection.Insert(challenge2);

            // add answers
            var answer1a = new ChallengeAnswer() { Answer = "Test Answer 1a", Correct = true, ChallengeId = challenge1.Id };
            var answer1b = new ChallengeAnswer() { Answer = "Test Answer 1b", Correct = false, ChallengeId = challenge1.Id };
            var answer2a = new ChallengeAnswer() { Answer = "Test Answer 2a", Correct = true, ChallengeId = challenge2.Id };
            var answer2b = new ChallengeAnswer() { Answer = "Test Answer 2b", Correct = false, ChallengeId = challenge2.Id };
            Persistence.Connection.InsertAll(new ChallengeAnswer[] { answer1a, answer1b, answer2a, answer2b });

            // delete one challenge
            challenge1.Delete();

            // verify challenge and answers are deleted
            Assert.IsEmpty(Persistence.Connection.Table<Challenge>().Where(x => x.Id == challenge1.Id));
            Assert.IsEmpty(Persistence.Connection.Table<ChallengeAnswer>().Where(x => x.Id == answer1a.Id));
            Assert.IsEmpty(Persistence.Connection.Table<ChallengeAnswer>().Where(x => x.Id == answer1b.Id));

            // verify other challenge is not deleted
            Assert.IsNotEmpty(Persistence.Connection.Table<Challenge>().Where(x => x.Id == challenge2.Id));
            Assert.IsNotEmpty(Persistence.Connection.Table<ChallengeAnswer>().Where(x => x.Id == answer2a.Id));
            Assert.IsNotEmpty(Persistence.Connection.Table<ChallengeAnswer>().Where(x => x.Id == answer2b.Id));
        }
    }
}
