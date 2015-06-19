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
using Nancy.Conventions;

namespace HavenWebApp
{
    //public class Bootstrapper : DefaultNancyBootstrapper
    //{
    //    protected override void ApplicationStartup(Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
    //    {
    //        CookieBasedSessions.Enable(pipelines);
    //    }
    //}

    public class ApplicationBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("Fonts", @"Fonts"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("Scripts", @"Scripts"));
            base.ConfigureConventions(nancyConventions);
        }
    }

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

            Get["/Boards"] = parameters =>
            {
                return View["Boards.html", Persistence.Connection.Table<Board>()];
            };

            Get["/Games"] = parameters =>
            {
                return View["Games.html", Persistence.Connection.Table<Game>()];
            };

            Get["/NewGame"] = parameters =>
            {
                var game = Game.NewGame((int)this.Request.Query.BoardId, (int)this.Request.Query.NumberOfPlayers);
                game.Name = (string)this.Request.Query.Name;
                Persistence.Connection.Update(game);
                return JsonConvert.SerializeObject(game);
            };

            Get["/Game/{id}"] = parameters =>
            {
                var game = Persistence.Connection.Get<Game>((int)parameters.id);
                return JsonConvert.SerializeObject(game);
            };

            Get["/Game/{id}/Players"] = parameters =>
            {
                var gameId = (int)parameters.id;
                var players = Persistence.Connection.Table<Player>().Where(x => x.GameId == gameId);
                return JsonConvert.SerializeObject(players);
            };

            Post["/PerformAction"] = parameters =>
            {
                var action = Persistence.Connection.Get<Haven.Action>((int)this.Request.Form.Id);
                var message = action.PerformAction((string)this.Request.Form.Input);
                return JsonConvert.SerializeObject(message);
            };
        }
    }
}