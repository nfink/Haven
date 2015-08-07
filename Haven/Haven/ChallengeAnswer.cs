using SQLite;

namespace Haven
{
    public class ChallengeAnswer : IEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public IRepository Repository { private get; set; }

        public int ChallengeId { get; set; }

        public string Answer { get; set; }

        public bool Correct { get; set; }
    }
}
