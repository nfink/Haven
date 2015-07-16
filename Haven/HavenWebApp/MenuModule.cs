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
using Nancy.Authentication.Forms;

namespace HavenWebApp
{
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

            //Get["/"] = parameters =>
            //{
            //    return View["Views/Haven.cshtml"];
            //};

            //Get["/Boards"] = parameters =>
            //{
            //    return JsonConvert.SerializeObject(Persistence.Connection.Table<Board>().Where(x => x.Active));
            //};

            //Get["/Games"] = parameters =>
            //{
            //    return JsonConvert.SerializeObject(Persistence.Connection.Table<Game>());
            //};

            //Get["/Pieces"] = parameters =>
            //{
            //    return JsonConvert.SerializeObject(Piece.Pieces);
            //};

            //Get["/Colors"] = parameters =>
            //{
            //    return JsonConvert.SerializeObject(Color.Colors);
            //};
        }
    }
}