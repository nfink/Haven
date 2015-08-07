using SQLite;

namespace Haven
{
    public class BoardChallengeCategory : IEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public IRepository Repository { private get; set; }

        public int BoardId { get; set; }

        public int ChallengeCategoryId { get; set; }
    }
}
