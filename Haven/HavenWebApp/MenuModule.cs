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

        public MenuModule(TinyIoCContainer container, IRootPathProvider pathProvider)
        {
            if (DataLoad == null)
            {
                using (var repository = container.Resolve<IRepository>())
                {
                    try
                    {
                        var test = repository.Find<User>(x => true);
                    }
                    catch
                    {
                        DataLoad = new DataLoad();
                        DataLoad.LoadTables();
                    }
                }
            }

            Get["/Pieces"] = parameters =>
            {
                using (var repository = container.Resolve<IRepository>())
                {
                    return JsonConvert.SerializeObject(repository.FindAll<Piece>());
                }
            };

            Get["/Colors"] = parameters =>
            {
                using (var repository = container.Resolve<IRepository>())
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