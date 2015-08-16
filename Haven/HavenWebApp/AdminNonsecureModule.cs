using Haven;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.TinyIoc;

namespace HavenWebApp
{
    public class AdminNonsecureModule : NancyModule
    {
        public AdminNonsecureModule(TinyIoCContainer container)
        {
            Get["/Login"] = parameters =>
            {
                return View["Views/Login.cshtml"];
            };

            Post["/Login"] = parameters =>
            {
                using (var repository = container.Resolve<IRepository>())
                {
                    var userGuid = UserMapper.ValidateUser(repository, (string)this.Request.Form.Username, (string)this.Request.Form.Password);

                    if (userGuid == null)
                    {
                        return null;
                    }
                    else
                    {
                        return this.LoginAndRedirect(userGuid.Value);
                    }
                }
            };

            Get["/Logout"] = parameters =>
            {
                return this.Logout("~/Login");
            };
        }
    }
}