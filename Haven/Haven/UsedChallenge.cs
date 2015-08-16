using SQLite;

namespace Haven
{
    public class UsedChallenge : IEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Ignore]
        public IRepository Repository { private get; set; }

        public int GameId { get; set; }

        public int ChallengeId { get; set; }
    }
}
