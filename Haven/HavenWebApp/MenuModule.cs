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

    public class MenuModule : NancyModule
    {
        public static DataLoad DataLoad;

        public MenuModule()
        {
            if (DataLoad == null)
            {
                DataLoad = new DataLoad();
                DataLoad.LoadTables();
            }

            Get["/"] = parameters => View["Haven.cshtml", null];

            Get["/Boards"] = parameters =>
            {
                return View["BoardsMenu.cshtml", Persistence.Connection.Table<Board>()];
            };

            Get["/Games"] = parameters =>
            {
                return View["GamesMenu.cshtml", Persistence.Connection.Table<Game>()];
            };

            Get["/NewGame"] = parameters =>
            {
                var game = Game.NewGame((int)this.Request.Query.BoardId, (int)this.Request.Query.NumberOfPlayers);
                game.Name = (string)this.Request.Query.Name;
                Persistence.Connection.Update(game);
                return View["Game.cshtml", game];
            };
        }
    }
}