using Haven;
using Nancy;
using Nancy.Responses;
using Newtonsoft.Json;

namespace HavenWebApp
{
    public class GameModule : NancyModule
    {
        public GameModule()
        {
            Get["/Play/{id}"] = parameters =>
            {
                return View["Views/Game.cshtml", (int)parameters.id];
            };

            Get["/Games/{id}"] = parameters =>
            {
                var game = Persistence.Connection.Get<Game>((int)parameters.id);
                return JsonConvert.SerializeObject(game);
            };

            Get["/Games/{id}/Players"] = parameters =>
            {
                var gameId = (int)parameters.id;
                var players = Persistence.Connection.Table<Player>().Where(x => x.GameId == gameId);
                return JsonConvert.SerializeObject(players);
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
        }
    }
}