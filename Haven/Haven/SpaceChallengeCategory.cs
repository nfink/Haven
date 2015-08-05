using SQLite;

namespace Haven
{
    public class SpaceChallengeCategory
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int SpaceId { get; set; }

        public int ChallengeCategoryId { get; set; }
    }
}