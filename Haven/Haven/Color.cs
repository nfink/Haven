using SQLite;

namespace Haven
{
    public class Color : IEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public IRepository Repository { private get; set; }

        public string Name { get; set; }
    }
}
