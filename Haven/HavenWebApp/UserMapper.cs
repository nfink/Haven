using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Haven;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Security;

namespace HavenWebApp
{
    public class UserMapper : IUserMapper
    {
        public IUserIdentity GetUserFromIdentifier(Guid identifier, NancyContext context)
        {
            string userGuid = identifier.ToString();
            var user = Persistence.Connection.Table<User>().Where(x => x.Guid == userGuid).FirstOrDefault();

            return user == null ? null : new PlayerIdentity() { UserName = user.Id.ToString() };
        }

        public static Guid? ValidateUser(string username, string password)
        {
            var user = Persistence.Connection.Table<User>().Where(x => x.Username == username).FirstOrDefault();

            if (user.VerifyPassword(password))
            {
                return new Guid(user.Guid);
            }
            else
            {
                return null;
            }
        }
    }
}