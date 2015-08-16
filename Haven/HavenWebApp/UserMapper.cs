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
        private IRepository Repository;

        public UserMapper(IRepository repository)
        {
            this.Repository = repository;
        }

        public IUserIdentity GetUserFromIdentifier(Guid identifier, NancyContext context)
        {
            string userGuid = identifier.ToString();
            var user = this.Repository.Find<User>(x => x.Guid == userGuid).SingleOrDefault();

            return user == null ? null : new PlayerIdentity() { UserName = user.Id.ToString() };
        }

        public static Guid? ValidateUser(IRepository repository, string username, string password)
        {
            var user = repository.Find<User>(x => x.Username == username).SingleOrDefault();

            if (user == null)
            {
                return null;
            }

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