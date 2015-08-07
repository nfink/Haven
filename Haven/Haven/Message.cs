using SQLite;

namespace Haven
{
    public class Message : IEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public IRepository Repository { private get; set; }

        public int PlayerId { get; set; }

        public string Text { get; set; }
    }
}
