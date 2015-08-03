using SQLite;
using System.Collections.Generic;

namespace Haven
{
    public class Challenge : IDeletable, ICloneable<Challenge>
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int OwnerId { get; set; }

        public int ChallengeCategoryId { get; set; }

        public string Question { get; set; }

        public bool OpenEnded { get; set; }

        public IEnumerable<ChallengeAnswer> Answers
        {
            get
            {
                return Persistence.Connection.Table<ChallengeAnswer>().Where(x => x.ChallengeId == this.Id);
            }
        }

        public bool CorrectAnswer(int answerId)
        {
            return Persistence.Connection.Get<ChallengeAnswer>(answerId).Correct;
        }

        public bool CorrectAnswer(string answer)
        {
            return Persistence.Connection.Query<ChallengeAnswer>("select ChallengeAnswer.* from ChallengeAnswer where ChallengeAnswer.ChallengeId=? and ChallengeAnswer.Correct<>0 and ChallengeAnswer.Answer like ?", this.Id, answer).Count > 0;
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
            // delete challenge
            Persistence.Connection.Delete<Challenge>(this.Id);

            // delete any answers
            Persistence.Connection.Execute("delete from ChallengeAnswer where ChallengeId=?", this.Id);
        }

        public Challenge Clone()
        {
            var challenge = new Challenge() { OwnerId = this.OwnerId, ChallengeCategoryId = this.ChallengeCategoryId, Question = this.Question, OpenEnded = this.OpenEnded };
            Persistence.Connection.Insert(challenge);

            // clone answers
            foreach (ChallengeAnswer answer in this.Answers)
            {
                Persistence.Connection.Insert(new ChallengeAnswer() { ChallengeId = challenge.Id, Answer = answer.Answer, Correct = answer.Correct });
            }

            return challenge;
        }
    }
}
