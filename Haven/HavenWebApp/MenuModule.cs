using Haven;
using Haven.Data;
using Nancy;
using Newtonsoft.Json;
using Nancy.TinyIoc;

namespace HavenWebApp
{
    public class MenuModule : NancyModule
    {
        public static DataLoad DataLoad;

        public MenuModule(IRootPathProvider pathProvider, IRepository repository)
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
                using (repository)
                {
                    return JsonConvert.SerializeObject(repository.FindAll<Piece>());
                }
            };

            Get["/Colors"] = parameters =>
            {
                using (repository)
                {
                    return JsonConvert.SerializeObject(repository.FindAll<Color>());
                }
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