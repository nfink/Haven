using Haven;
using Haven.Data;
using Nancy;

namespace HavenWebApp
{
    public class MenuModule : NancyModule
    {
        public static DataLoad DataLoad;

        public MenuModule(IRootPathProvider pathProvider)
        {
            if (DataLoad == null)
            {
                try
                {
                    var test = Persistence.Connection.Table<User>().Count();
                }
                catch
                {
                    DataLoad = new DataLoad();
                    DataLoad.LoadTables();
                }
            }
        }
    }
}