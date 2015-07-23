using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.TinyIoc;
using Nancy.Authentication.Forms;
using System.Web.Optimization;
using System.Web.Optimization.React;

namespace HavenWebApp
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("Fonts", @"Fonts"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("Scripts", @"Scripts"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("Uploads", @"Uploads"));
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

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            var reactBundle = new ScriptBundle("~/Scripts/react", "https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js")
                .Include("~/Scripts/react/react-{version}.js");

            var jsxTransformerBundle = new ScriptBundle("~/Scripts/jsxtransformer", "https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/JSXTransformer.js")
                .Include("~/Scripts/react/JSXTransformer-{version}.js");

            var jqueryBundle = new ScriptBundle("~/Scripts/jquery", "https://code.jquery.com/jquery-1.11.3.min.js")
                .Include("~/Scripts/jquery-{version}.js");
            
            var pageBundle = new ScriptBundle("~/Scripts/page", "https://cdn.rawgit.com/visionmedia/page.js/master/page.js")
                .Include("~/Scripts/page.js");

            var metroBundle = new ScriptBundle("~/Scripts/metro")
                .Include("~/Scripts/metro.js");

            var scriptBundle = new JsxBundle("~/Scripts/bundle")
                .IncludeDirectory("~/Scripts/Components/Admin", "*.js")
                .Include("~/Scripts/Components/Board/EditBoard.js")
                .IncludeDirectory("~/Scripts/Components/Game", "*.js")
                .IncludeDirectory("~/Scripts/Components", "*.js")
                .Include("~/Scripts/Menu.js")
                .Include("~/Scripts/Game.js");

            var styleBundle = new StyleBundle("~/Content/bundle")
                .Include("~/Content/Game.css")
                .Include("~/Content/Haven.css")
                .Include("~/Content/metro-icons.css")
                .Include("~/Content/metro.css");

            BundleTable.Bundles.UseCdn = true;
            BundleTable.Bundles.Add(reactBundle);
            BundleTable.Bundles.Add(jsxTransformerBundle);
            BundleTable.Bundles.Add(jqueryBundle);
            BundleTable.Bundles.Add(pageBundle);
            BundleTable.Bundles.Add(metroBundle);
            BundleTable.Bundles.Add(scriptBundle);
            BundleTable.Bundles.Add(styleBundle);

            BundleTable.EnableOptimizations = false;
        }
    }
}