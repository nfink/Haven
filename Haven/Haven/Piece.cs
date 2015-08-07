using SQLite;

namespace Haven
{
    public class Piece : IEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public IRepository Repository { private get; set; }

        public string Name { get; set; }

        public string Image { get; set; }
    }
}
