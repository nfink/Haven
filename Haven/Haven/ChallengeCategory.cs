using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Haven
{
    public class ChallengeCategory : IDeletable
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        public int OwnerId { get; set; }

        public void Delete()
        {
            // delete category
            Persistence.Connection.Delete<ChallengeCategory>(this.Id);

            // remove links from any challeneges
            Persistence.Connection.Execute("update Challenge set ChallengeCategoryId=0 where ChallengeCategoryId=?", this.Id);
        }
    }
}
