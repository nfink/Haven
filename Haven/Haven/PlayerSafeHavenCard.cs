using SQLite;

namespace Haven
{
    public class PlayerSafeHavenCard : IEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Ignore]
        public IRepository Repository { private get; set; }

        public int PlayerId { get; set; }

        public int SafeHavenCardId { get; set; }
    }
}
