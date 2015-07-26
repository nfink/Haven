﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Haven
{
    public class Challenge : IDeletable
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int BoardId { get; set; }

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
            var challenge = new Challenge();

            Persistence.Connection.Insert(challenge);
            return challenge;
        }
    }
}
