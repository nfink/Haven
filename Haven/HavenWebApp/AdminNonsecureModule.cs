using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.Security;
using Nancy.Authentication.Forms;

namespace HavenWebApp
{
    public class AdminNonsecureModule : NancyModule
    {
        public AdminNonsecureModule()
        {
            Get["/Login"] = parameters =>
            {
                return View["Views/Login.cshtml"];
            };

            Post["/Login"] = parameters =>
            {
                var userGuid = UserMapper.ValidateUser((string)this.Request.Form.Username, (string)this.Request.Form.Password);

                if (userGuid == null)
                {
                    return null;
                }
                else
                {
                    return this.LoginAndRedirect(userGuid.Value);
                }
            };

            Get["/Logout"] = parameters =>
            {
                return this.Logout("~/Login");
            };
        }
    }
}