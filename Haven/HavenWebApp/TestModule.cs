using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Haven;
using Haven.Data;
using Nancy.Session;
using Nancy.ModelBinding;
using Newtonsoft.Json;

namespace HavenWebApp
{
    //public class Bootstrapper : DefaultNancyBootstrapper
    //{
    //    protected override void ApplicationStartup(Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
    //    {
    //        CookieBasedSessions.Enable(pipelines);
    //    }
    //}

    public class TestModule : NancyModule
    {
        public static DataLoad DataLoad;

        public TestModule()
        {
            if (DataLoad == null)
            {
                DataLoad = new DataLoad();
                DataLoad.LoadTables();
            }

            Get["/"] = parameters => View["Home.html", null];

            Get["/Space"] = parameters =>
            {
                return Persistence.Connection.Table<Space>();
            };

            Get["/NewGame"] = parameters =>
            {
                var game = Game.NewGame(1, 2);
                return game;// JsonConvert.SerializeObject(game);
            };

            Get["/Game/{id}"] = parameters =>
            {
                var game = Persistence.Connection.Get<Game>(parameters.id);
                return JsonConvert.SerializeObject(game);
            };

            Get["/Game/{id}/Players"] = parameters =>
            {
                var gameId = (int)parameters.id;
                var players = Persistence.Connection.Table<Player>().Where(x => x.GameId == gameId);
                return JsonConvert.SerializeObject(players);
            };

            Post["/PerformAction/{id}"] = parameters =>
            {
                var actionId = (int)parameters.id;
                var action = Persistence.Connection.Get<Haven.Action>(actionId);
                var message = action.PerformAction(this.Request.Body.ToString());
                return JsonConvert.SerializeObject(message);
            };
        }
    }
}