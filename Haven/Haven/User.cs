using Newtonsoft.Json;
using SQLite;

namespace Haven
{
    public class User : IEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public IRepository Repository { private get; set; }

        [JsonIgnore]
        public string Guid { get; set; }

        public string Username { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        public void SetPassword(string password)
        {
            string savedPasswordHash = Haven.Password.HashPassword(password);
            this.Password = savedPasswordHash;
            this.Repository.Update<User>(this);
        }

        public bool VerifyPassword(string password)
        {
            return Haven.Password.VerifyPassword(this.Password, password);
        }
    }
}
