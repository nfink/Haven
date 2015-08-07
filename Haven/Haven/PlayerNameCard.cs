using SQLite;

namespace Haven
{
    public class PlayerNameCard : IEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public IRepository Repository { private get; set; }

        public int PlayerId { get; set; }

        public int NameCardId { get; set; }
    }
}
