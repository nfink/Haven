using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Haven
{
    public class ChallengeAnswer
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int ChallengeId { get; set; }

        public string Answer { get; set; }

        public bool Correct { get; set; }
    }
}
