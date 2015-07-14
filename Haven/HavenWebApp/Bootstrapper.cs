using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.TinyIoc;
using Nancy.Authentication.Forms;

namespace HavenWebApp
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("Fonts", @"Fonts"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("Scripts", @"Scripts"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("Uploads", @"Uploads"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("Admin/Content", @"Content"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("Admin/Fonts", @"Fonts"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("Admin/Scripts", @"Scripts"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("Views", @"Views"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("Templates", @"Templates"));
            base.ConfigureConventions(nancyConventions);
        }


        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            // We don't call "base" here to prevent auto-discovery of
            // types/dependencies
        }

        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);

            // Here we register our user mapper as a per-request singleton.
            // As this is now per-request we could inject a request scoped
            // database "context" or other request scoped services.
            container.Register<IUserMapper, UserMapper>();
        }

        //protected override void RequestStartup(TinyIoCContainer requestContainer, IPipelines pipelines, NancyContext context)
        //{
        //    // At request startup we modify the request pipelines to
        //    // include forms authentication - passing in our now request
        //    // scoped user name mapper.
        //    //
        //    // The pipelines passed in here are specific to this request,
        //    // so we can add/remove/update items in them as we please.
        //    var formsAuthConfiguration =
        //        new FormsAuthenticationConfiguration()
        //        {
        //            DisableRedirect = true,
        //            UserMapper = requestContainer.Resolve<IUserMapper>(),
        //        };

        //    FormsAuthentication.Enable(pipelines, formsAuthConfiguration);
        //}
    }
}