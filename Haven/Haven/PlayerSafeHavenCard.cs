using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Haven
{
    class PlayerSafeHavenCard
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int PlayerId { get; set; }

        public int SafeHavenCardId { get; set; }
    }
}
