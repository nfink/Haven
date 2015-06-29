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
using Nancy.Responses;

namespace HavenWebApp
{
    public class GameModule : NancyModule
    {
        public GameModule()
        {
            Get["/NewGame"] = parameters =>
            {
                var game = Game.NewGame((int)this.Request.Query.BoardId, (int)this.Request.Query.NumberOfPlayers);
                game.Name = (string)this.Request.Query.Name;
                Persistence.Connection.Update(game);
                // remove actions that the user does not have access to

                return View["Game.cshtml", game];
            };

            Get["/Game/{id}"] = parameters =>
            {
                var game = Persistence.Connection.Get<Game>((int)parameters.id);
                // remove actions that the user does not have access to

                return View["Game.cshtml", game];
            };

            Get["/Game/{id}/Players"] = parameters =>
            {
                var gameId = (int)parameters.id;
                var players = Persistence.Connection.Table<Player>().Where(x => x.GameId == gameId);

                // remove actions that the user does not have access to
                //var passwords = parameters.passwords;

                return View["Players.cshtml", players];
            };

            Post["/Authenticate"] = parameters =>
            {
                var password = (string)this.Request.Form.Password;
                var player = Persistence.Connection.Get<Player>((int)this.Request.Form.PlayerId);
                if (player.VerifyPassword(password))
                {
                    return View["Actions.cshtml", new Player[] { player }];
                }
                else
                {
                    return new HtmlResponse(HttpStatusCode.Unauthorized);
                }
            };
        }
    }
}