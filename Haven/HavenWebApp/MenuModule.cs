using Haven;
using Haven.Data;
using Nancy;
using Newtonsoft.Json;

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

            Get["/Pieces"] = parameters =>
            {
                return JsonConvert.SerializeObject(Piece.Pieces);
            };

            Get["/Colors"] = parameters =>
            {
                return JsonConvert.SerializeObject(Color.Colors);
            };

            #if DEBUG
            Get["/RecompileJSX"] = parameters =>
            {
                return View["Views/RecompileJSX.cshtml", Bootstrapper.TransformJSX(pathProvider)];
            };
            #endif
        }
    }
}