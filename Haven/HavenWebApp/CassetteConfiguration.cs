using Cassette;
using Cassette.Scripts;
using Cassette.Stylesheets;

namespace HavenWebApp
{
    /// <summary>
    /// Configures the Cassette asset bundles for the web application.
    /// </summary>
    public class CassetteBundleConfiguration : IConfiguration<BundleCollection>
    {
        public void Configure(BundleCollection bundles)
        {
            bundles.AddUrlWithAlias<ScriptBundle>("https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js", "react");
            bundles.AddUrlWithLocalAssets<ScriptBundle>("react",
                new LocalAssetSettings
                {
                    FallbackCondition = "!window.React",
                    Path = "~/Scripts/react/react-0.13.1.js"
                }
            );

            bundles.AddUrlWithAlias<ScriptBundle>("https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/JSXTransformer.js", "jsxtransformer");
            bundles.AddUrlWithLocalAssets<ScriptBundle>("jsxtransformer",
                new LocalAssetSettings
                {
                    FallbackCondition = "!window.React",
                    Path = "~/Scripts/react/JSXTransformer-0.13.1.js"
                }
            );

            bundles.AddUrlWithAlias<ScriptBundle>("https://code.jquery.com/jquery-1.11.3.min.js", "jquery");
            bundles.AddUrlWithLocalAssets<ScriptBundle>("jquery",
                new LocalAssetSettings
                {
                    FallbackCondition = "!window.jQuery",
                    Path = "~/Scripts/jquery-1.11.3.js"
                }
            );

            bundles.AddUrlWithAlias<ScriptBundle>("https://cdn.rawgit.com/visionmedia/page.js/master/page.js", "page");
            bundles.AddUrlWithLocalAssets<ScriptBundle>("page",
                new LocalAssetSettings
                {
                    FallbackCondition = "!window.page",
                    Path = "~/Scripts/page.js"
                }
            );

            bundles.Add<ScriptBundle>("metro.js", "~/Scripts/metro.js");

            bundles.Add<ScriptBundle>("~/Scripts/Components", new FileSearch() { Pattern = "*.js", SearchOption = System.IO.SearchOption.AllDirectories });

            bundles.Add<ScriptBundle>("bundle.js", "~/Scripts/Game.js", "~/Scripts/Menu.generated.js");

            bundles.Add<StylesheetBundle>("bundle.css", "~/Content/Game.css", "~/Content/Haven.css", "~/Content/metro-icons.css", "~/Content/metro.css");


        }
    }
}