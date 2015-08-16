using Haven;
using Nancy;
using Nancy.Responses;
using Nancy.TinyIoc;
using Newtonsoft.Json;

namespace HavenWebApp
{
    public class GameModule : NancyModule
    {
        public GameModule(TinyIoCContainer container)
        {
            Get["/Play/{id}"] = parameters =>
            {
                return View["Views/Game.cshtml", (int)parameters.id];
            };

            Get["/Games/{id}"] = parameters =>
            {
                using (var repository = container.Resolve<IRepository>())
                {
                    var game = repository.Get<Game>((int)parameters.id);
                    return JsonConvert.SerializeObject(game);
                }
            };

            Get["/Games/{id}/Players"] = parameters =>
            {
                using (var repository = container.Resolve<IRepository>())
                {
                    var gameId = (int)parameters.id;
                    var players = repository.Find<Player>(x => x.GameId == gameId);
                    return JsonConvert.SerializeObject(players);
                }
            };

            Post["/Authenticate"] = parameters =>
            {
                using (var repository = container.Resolve<IRepository>())
                {
                    var password = (string)this.Request.Form.Password;
                    var player = repository.Get<Player>((int)this.Request.Form.PlayerId);
                    if (player.VerifyPassword(password))
                    {
                        return new HtmlResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        return new HtmlResponse(HttpStatusCode.Unauthorized);
                    }
                }
            };
        }
    }
}