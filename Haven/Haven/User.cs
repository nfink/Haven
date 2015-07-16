using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Newtonsoft.Json;

namespace Haven
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Guid { get; set; }

        public string Username { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        public void SetPassword(string password)
        {
            string savedPasswordHash = Haven.Password.HashPassword(password);
            this.Password = savedPasswordHash;
            Persistence.Connection.Update(this);
        }

        public bool VerifyPassword(string password)
        {
            return Haven.Password.VerifyPassword(this.Password, password);
        }
    }
}
