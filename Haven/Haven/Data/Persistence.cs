using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Haven
{
    public class Persistence
    {
        public static SQLiteConnection Connection = new SQLiteConnection("C:\\Users\\Nolan\\OneDrive\\Code\\Haven\\Haven\\Haven\\bin\\Debug\\Data\\SafeHavenGame.sqlite");

    }
}
