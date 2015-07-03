﻿using System;
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
using Nancy.Security;

namespace HavenWebApp
{
    public class GameModule : NancyModule
    {
        public GameModule()
        {
            Post["/NewGame"] = parameters =>
            {
                var game = Game.NewGame((int)this.Request.Form.BoardId, (int)this.Request.Form.NumberOfPlayers);
                game.Name = (string)this.Request.Form.Name;
                Persistence.Connection.Update(game);
                // remove actions that the user does not have access to

                return View["Views/Game.cshtml", game];
            };

            Get["/Game/{id}"] = parameters =>
            {
                var game = Persistence.Connection.Get<Game>((int)parameters.id);
                // remove actions that the user does not have access to

                return View["Views/Game.cshtml", game];
            };

            Get["/Game/{id}/Players"] = parameters =>
            {
                var gameId = (int)parameters.id;
                var players = Persistence.Connection.Table<Player>().Where(x => x.GameId == gameId);

                // remove actions that the user does not have access to
                //var passwords = parameters.passwords;

                return View["Views/Players.cshtml", players];
            };

            Post["/Authenticate"] = parameters =>
            {
                var password = (string)this.Request.Form.Password;
                var player = Persistence.Connection.Get<Player>((int)this.Request.Form.PlayerId);
                if (player.VerifyPassword(password))
                {
                    return new HtmlResponse(HttpStatusCode.OK);
                }
                else
                {
                    return new HtmlResponse(HttpStatusCode.Unauthorized);
                }
            };

            Get["/test"] = parameters =>
            {
                this.RequiresAuthentication();

                return this.Context.CurrentUser.UserName;
            };
        }
    }
}