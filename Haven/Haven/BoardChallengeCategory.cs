using SQLite;

namespace Haven
{
    public class BoardChallengeCategory
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int BoardId { get; set; }

        public int ChallengeCategoryId { get; set; }
    }
}
