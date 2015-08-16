using SQLite;

namespace Haven
{
    public class SpaceChallengeCategory : IEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Ignore]
        public IRepository Repository { private get; set; }

        public int SpaceId { get; set; }

        public int ChallengeCategoryId { get; set; }
    }
}