using SQLite;

namespace Haven
{
    public class BibleVerse : ICloneable<BibleVerse>
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

        public BibleVerse Clone()
        {
            var recall = new BibleVerse() { Text = this.Text };
            Persistence.Connection.Insert(recall);
            return recall;
        }
    }
}
