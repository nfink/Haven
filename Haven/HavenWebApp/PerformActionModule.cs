using Haven;
using Nancy;
using Nancy.Authentication.Stateless;
using Nancy.Security;
using Nancy.TinyIoc;
using Newtonsoft.Json;
using System.Linq;

namespace HavenWebApp
{
    public class PerformActionModule : NancyModule
    {
        public PerformActionModule(TinyIoCContainer container)
        {
            var performActionConfiguration =
                new StatelessAuthenticationConfiguration(context =>
                {
                    var actionId = (int)this.Request.Form.Id;
                    var password = (string)context.Request.Form.Password;
                    var player = container.Resolve<IRepository>().Get<Action>(actionId).Player;

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
                using (var repository = container.Resolve<IRepository>())
                {
                    var action = repository.Get<Haven.Action>((int)this.Request.Form.Id);
                    int gameId = repository.Find<Player>(x => x.Id == action.OwnerId).First().GameId;
                    action.PerformAction((string)this.Request.Form.Input);
                    var players = repository.Find<Player>(x => x.GameId == gameId);
                    repository.Commit();
                    return JsonConvert.SerializeObject(players);
                }
            };
        }
    }
}