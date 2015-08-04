using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Haven
{
    public class GameWinner
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int GameId { get; set; }

        public string Player { get; set; }

        public int Turn { get; set; }
    }
}