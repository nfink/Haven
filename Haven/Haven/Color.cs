using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Haven
{
    public class Color
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        public static IEnumerable<Color> Colors
        {
            get
            {
                return Persistence.Connection.Table<Color>();
            }
        }
    }
}
