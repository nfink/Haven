using SQLite;

namespace Haven
{
    public class GameWinner : IEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Ignore]
        public IRepository Repository { private get; set; }

        public int GameId { get; set; }

        public string Player { get; set; }

        public int Turn { get; set; }
    }
}