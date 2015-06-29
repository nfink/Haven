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
using Nancy.Security;
using Nancy.Authentication.Stateless;

namespace HavenWebApp
{
    public class PerformActionModule : NancyModule
    {
        public PerformActionModule()
        {
            var performActionConfiguration =
                new StatelessAuthenticationConfiguration(context =>
                {
                    var actionId = (int)this.Request.Form.Id;
                    var password = (string)context.Request.Form.Password;
                    var player = Persistence.Connection.Query<Player>("select Player.* from Player join Action on Player.Id=Action.OwnerId where Action.Id=?", actionId).First();

                    if (player.Password == null || player.VerifyPassword(password))
                    {
                        return new PlayerIdentity() { UserName = player.Id.ToString() };
                    }
                    else
                    {
                        return null;
                    }
                });

            StatelessAuthentication.Enable(this, performActionConfiguration);

            this.RequiresAuthentication();

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