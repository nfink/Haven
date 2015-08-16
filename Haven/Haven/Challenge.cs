using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Haven
{
    public class Challenge : IEntity, IDeletable, ICloneable<Challenge>
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Ignore]
        public IRepository Repository { private get; set; }

        public int OwnerId { get; set; }

        public int ChallengeCategoryId { get; set; }

        public string Question { get; set; }

        public bool OpenEnded { get; set; }

        public IEnumerable<ChallengeAnswer> Answers
        {
            get
            {
                return this.Repository.Find<ChallengeAnswer>(x => x.ChallengeId == this.Id);
            }
        }

        public bool CorrectAnswer(int answerId)
        {
            return this.Repository.Get<ChallengeAnswer>(answerId).Correct;
        }

        public bool CorrectAnswer(string answer)
        {
            return this.Answers.Where(x => x.Correct && string.Equals(answer, x.Answer, StringComparison.CurrentCultureIgnoreCase)).Count() > 0;
        }

        public bool CorrectAnswer(object answer)
        {
            if (this.OpenEnded)
            {
                return CorrectAnswer((string)answer);
            }
            else
            {
                return CorrectAnswer((int)answer);
            }
        }

        public void Delete()
        {
            foreach (ChallengeAnswer answer in this.Answers.ToList())
            {
                this.Repository.Remove(answer);
            }

            // delete challenge
            this.Repository.Remove(this);
        }

        public Challenge Clone()
        {
            var challenge = new Challenge() { OwnerId = this.OwnerId, ChallengeCategoryId = this.ChallengeCategoryId, Question = this.Question, OpenEnded = this.OpenEnded };
            this.Repository.Add(challenge);

            // clone answers
            foreach (ChallengeAnswer answer in this.Answers.ToList())
            {
                this.Repository.Add(new ChallengeAnswer() { ChallengeId = challenge.Id, Answer = answer.Answer, Correct = answer.Correct });
            }

            return challenge;
        }
    }
}
