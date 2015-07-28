using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.TinyIoc;
using React;
using System.IO;

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
            var pathProvider = container.Resolve<IRootPathProvider>();
            TransformJSX(pathProvider);
        }

        private static void TransformJSX(IRootPathProvider pathProvider)
        {
            bool useHarmony = false;
            bool stripTypes = false;
            var config = React.AssemblyRegistration.Container.Resolve<IReactSiteConfiguration>();
            config
                .SetReuseJavaScriptEngines(false)
                .SetUseHarmony(useHarmony)
                .SetStripTypes(stripTypes);

            var environment = React.AssemblyRegistration.Container.Resolve<IReactEnvironment>();
            string root = pathProvider.GetRootPath();
            var files = Directory.EnumerateFiles(root + "Scripts", "*.jsx", SearchOption.AllDirectories);
            foreach (var path in files)
            {
                environment.JsxTransformer.TransformAndSaveJsxFile("~/" + path.Replace(root, string.Empty));
            }
        }
    }
}