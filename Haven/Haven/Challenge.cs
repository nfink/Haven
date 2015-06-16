using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Haven
{
    public class Challenge
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int BoardId { get; set; }

        public string Question { get; set; }

        public IEnumerable<ChallengeAnswer> Answers
        {
            get
            {
                return Persistence.Connection.Table<ChallengeAnswer>().Where(x => x.ChallengeId == this.Id);
            }
        }
    }
}
