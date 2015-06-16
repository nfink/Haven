using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Haven
{
    class UsedChallenge
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int GameId { get; set; }

        public int ChallengeId { get; set; }
    }
}
