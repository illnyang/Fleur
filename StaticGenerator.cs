using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;
using Fleur.Models;
using Fleur.RawProviders;
using Fleur.Utils;
using Newtonsoft.Json;
using NLog;
using Scriban;

namespace Fleur
{
    internal static class StaticGenerator
    {
        // TODO: networked dump so we don't deserialize twice on first run?
        public static async Task GenerateFromProvider(IRawProvider provider)
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
            var allBooksRaw = await provider.GetAllBooks();
            var allBooks = RawToModel.GetAllBooks(allBooksRaw);

            foreach (var book in allBooks)
            {
                var bookRaw = await provider.GetBook(book.Id);

                if (bookRaw == null)
                {
                    continue;
                }

                var fullBook = RawToModel.GetBook(bookRaw);

                var dir = Path.Combine(Program.Config.CachePath, "books", fullBook.Id.ToString());

                var indexPath = Path.Combine(dir, "index.json");

                if (!File.Exists(indexPath))
                {
                    continue;
                }

                var uniqueName = $"{fullBook.Name.Trim()} ({fullBook.Kind}) ({fullBook.Released})";

                Logger.Info("\nGenerating: {0}\nPages-count: {1}", uniqueName, fullBook.Pages.Length);

                var outBookPath = Path.Combine(Program.Config.OutputPath, fullBook.Subject, FileUtils.MakeValidFileName(uniqueName));

                Directory.CreateDirectory(outBookPath);

                await File.WriteAllTextAsync(Path.Combine(outBookPath, "0000.html"),
                    FrontPageTemplate.Render(new { Book = fullBook, ResourcesPath = Path.GetFileNameWithoutExtension(Program.Config.ResourcesPath) }));

                foreach (var page in fullBook.Pages)
                {
                    var exercisesRaw = await provider.GetExercisesFromPage(fullBook.Id, page);
                    var exercises = RawToModel.GetExercisesFromPage(exercisesRaw);

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

                    await File.WriteAllTextAsync(Path.Combine(outBookPath, $"{page:D4}.html"),
                        PageTemplate.Render(new { Exercises = exercises, ResourcesPath = Path.GetFileNameWithoutExtension(Program.Config.ResourcesPath) }));

                    Logger.Info("Processed cache file {0}", $"{page:D4}.json");
                }
            }
        }

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly Template FrontPageTemplate = Template.Parse(File.ReadAllText(Path.Combine(Program.Config.TemplatesPath, "frontpage.html")));
        private static readonly Template PageTemplate = Template.Parse(File.ReadAllText(Path.Combine(Program.Config.TemplatesPath, "page.html")));
    }
}
