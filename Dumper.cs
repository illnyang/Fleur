using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;
using Fleur.Models;
using Fleur.Utils;
using Newtonsoft.Json;
using NLog;
using Scriban;

namespace Fleur
{
    internal static class Dumper
    {
        // TODO: networked dump so we don't deserialize twice on first run?
        public static async Task DumpFromCache()
        {
            // copy resources beforehand
            var outResourcesPath = Path.Combine(Program.Config.OutputPath, Path.GetFileNameWithoutExtension(Program.Config.ResourcesPath));
            Directory.CreateDirectory(outResourcesPath);

            foreach (var dirPath in Directory.GetDirectories(Program.Config.ResourcesPath, "*",
                SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(Program.Config.ResourcesPath, outResourcesPath));
            }

            foreach (var newPath in Directory.GetFiles(Program.Config.ResourcesPath, "*.*",
                SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(Program.Config.ResourcesPath, outResourcesPath), true);
            }

            // traverse cache
            var cacheBookPath = Path.Combine(Program.Config.CachePath, "books");

            if (!Directory.Exists(cacheBookPath))
            {
                Logger.Error("No cache to traverse!");
            }

            foreach (var dir in Directory.GetDirectories(cacheBookPath, "*"))
            {
                var indexPath = Path.Combine(dir, "index.json");

                if (!File.Exists(indexPath))
                {
                    continue;
                }

                var content = await File.ReadAllTextAsync(indexPath);
                var fullBook = JsonConvert.DeserializeObject<Book>(content);

                var uniqueName = $"{fullBook.Name.Trim()} ({fullBook.Kind}) ({fullBook.Released})";

                Logger.Info("\nDumping: {0}\nPages-count: {1}", uniqueName, fullBook.Pages.Length);

                var outBookPath = Path.Combine(Program.Config.OutputPath, $"{FileUtils.MakeValidFileName(uniqueName)}\\");

                Directory.CreateDirectory(outBookPath);

                await File.WriteAllTextAsync(Path.Combine(outBookPath, "0000.html"),
                    FrontPageTemplate.Render(new { Book = fullBook, ResourcesPath = Path.GetFileNameWithoutExtension(Program.Config.ResourcesPath) }));

                foreach (var file in Directory.GetFiles(dir, "*.json"))
                {
                    if (file.EndsWith("index.json"))
                    {
                        continue;
                    }

                    var pageContent = await File.ReadAllTextAsync(file);
                    var exercises = JsonConvert.DeserializeObject<Exercise[]>(pageContent);

                    var parser = new HtmlParser();

                    // resolve urls
                    foreach (var ex in exercises)
                    {
                        var document = parser.Parse(ex.Solution);

                        var images = document.All.Where(m => m.LocalName == "img");
                        foreach (var image in images)
                        {
                            var srcAttr = image.Attributes["src"];

                            if (srcAttr == null)
                            {
                                continue;
                            }

                            srcAttr.Value = srcAttr.Value.TrimStart('/');

                            if (string.IsNullOrWhiteSpace(image.Attributes["src"].Value))
                                continue;

                            var relUri = new Uri(srcAttr.Value, UriKind.RelativeOrAbsolute);
                            var baseUri = new Uri(@"https://odrabiamy.pl", UriKind.Absolute);
                            var remoteFullUri = new Uri(baseUri, relUri);

                            srcAttr.Value = remoteFullUri.ToString();
                        }

                        // svgs are not relative so we can skip them (possibly error-prone)

                        ex.Solution = document.DocumentElement.OuterHtml;
                    }

                    await File.WriteAllTextAsync(Path.Combine(outBookPath, $"{Path.GetFileNameWithoutExtension(file)}.html"),
                        PageTemplate.Render(new { Exercises = exercises, ResourcesPath = Path.GetFileNameWithoutExtension(Program.Config.ResourcesPath) }));

                    Logger.Info("Processed cache file {0}", Path.GetFileName(file));
                }
            }
        }

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly Template FrontPageTemplate = Template.Parse(File.ReadAllText(Path.Combine(Program.Config.TemplatesPath, "frontpage.html")));
        private static readonly Template PageTemplate = Template.Parse(File.ReadAllText(Path.Combine(Program.Config.TemplatesPath, "page.html")));
    }
}
