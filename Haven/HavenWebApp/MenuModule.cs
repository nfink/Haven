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
using Nancy.Security;

namespace HavenWebApp
{
    //public class Bootstrapper : DefaultNancyBootstrapper
    //{
    //    protected override void ApplicationStartup(Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
    //    {
    //        CookieBasedSessions.Enable(pipelines);
    //    }
    //}

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
        }
    }
}