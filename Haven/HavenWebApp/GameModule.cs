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

    public class GameModule : NancyModule
    {
        public GameModule()
        {
            Get["/Game/{id}"] = parameters =>
            {
                var game = Persistence.Connection.Get<Game>((int)parameters.id);
                return View["Game.cshtml", game];
            };

            Get["/Game/{id}/Players"] = parameters =>
            {
                var gameId = (int)parameters.id;
                var players = Persistence.Connection.Table<Player>().Where(x => x.GameId == gameId);
                return View["Players.cshtml", players];
            };

            Post["/PerformAction"] = parameters =>
            {
                var action = Persistence.Connection.Get<Haven.Action>((int)this.Request.Form.Id);
                int gameId = Persistence.Connection.Table<Player>().Where(x => x.Id == action.OwnerId).First().GameId;
                action.PerformAction((string)this.Request.Form.Input);
                var players = Persistence.Connection.Table<Player>().Where(x => x.GameId == gameId);
                return View["Players.cshtml", players];
            };
        }
    }
}