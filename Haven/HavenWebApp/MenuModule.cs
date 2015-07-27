using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Haven;
using Haven.Data;
using Nancy.Session;
using Nancy.ModelBinding;
using Newtonsoft.Json;
using Nancy.Conventions;
using Nancy.Security;
using Nancy.Authentication.Forms;
using React;
using System.IO;

namespace HavenWebApp
{
    public class MenuModule : NancyModule
    {
        public static DataLoad DataLoad;

        public MenuModule(IRootPathProvider pathProvider)
        {
            if (DataLoad == null)
            {
                DataLoad = new DataLoad();
                DataLoad.LoadTables();
            }

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