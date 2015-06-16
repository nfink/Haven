using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Haven
{
    public class BibleVerse
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Book { get; set; }

        public int Chapter { get; set; }

        public int Verse { get; set; }

        public string Edition { get; set; }

        public string Text { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}:{2}", this.Book, this.Chapter, this.Verse);
        }
    }
}
