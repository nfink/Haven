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

        public IEnumerable<ChallengeAnswer> Answers
        {
            get
            {
                return Persistence.Connection.Table<ChallengeAnswer>().Where(x => x.ChallengeId == this.Id);
            }
        }

        public void Delete()
        {
            // delete challenge
            Persistence.Connection.Delete<Challenge>(this.Id);

            // delete any answers
            Persistence.Connection.Execute("delete from ChallengeAnswer where ChallengeId=?", this.Id);

            // delete any uses by boards
            Persistence.Connection.Execute("delete from BoardChallenge where ChallengeId=?", this.Id);
        }

        public Challenge Clone()
        {
            var challenge = new Challenge() { OwnerId = this.OwnerId, ChallengeCategoryId = this.ChallengeCategoryId, Question = this.Question };
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
