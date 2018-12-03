using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Fleur.Models;
using Fleur.RawProviders;
using Fleur.Utils;
using Newtonsoft.Json;
using NLog;

namespace Fleur
{
    internal static class Cache
    {
        public static async Task UpdateCache(IRawProvider provider)
        {
            var cacheUserPath = Path.Combine(Program.Config.CachePath, "users");
            Directory.CreateDirectory(cacheUserPath);

            var response = await provider.GetUserInfo();

            var userInfo = RawToModel.GetUserInfo(response);

            if (!userInfo.IsPremium)
            {
                throw new Exception($"User {userInfo.Name} does not have premium access to odrabiamy.pl.");
            }

            if (userInfo.ActiveSubscriptions == null || userInfo.ActiveSubscriptions.Length == 0)
            {
                throw new Exception($"User {userInfo.Name} has no subscriptions available.");
            }

            await File.WriteAllTextAsync(Path.Combine(cacheUserPath, $"{userInfo.Id}.json"), response);

            await Work(provider, userInfo.ActiveSubscriptions.Select(sub => sub.Grade).ToArray());
        }

        private static async Task Work(IRawProvider provider, string[] availableGrades)
        {
            var cacheBookPath = Path.Combine(Program.Config.CachePath, "books");
            Directory.CreateDirectory(cacheBookPath);

            var response = await provider.GetAllBooks();

            var books = RawToModel.GetAllBooks(response, availableGrades);

            var processedBooksRaw = JsonConvert.SerializeObject(books, Formatting.None);
            await File.WriteAllTextAsync(Path.Combine(cacheBookPath, "index.json"), processedBooksRaw);

            foreach (var book in books)
            {
                await CacheBook(provider, book);
            }
        }

        private static async Task CacheBook(IRawProvider provider, Book book)
        {
            var fullResponse = await provider.GetBook(book.Id);
            var fullBook = RawToModel.GetBook(fullResponse);

            var uniqueName = $"{fullBook.Name.Trim()} ({fullBook.Kind}) ({fullBook.Released})";

            Logger.Info("\nCaching: {0}\nPages-count: {1}", uniqueName, fullBook.Pages.Length);

            var cacheBookPath = Path.Combine(Program.Config.CachePath, "books", fullBook.Id.ToString());

            if (Directory.Exists(cacheBookPath))
            {
                return;
            }

            Directory.CreateDirectory(cacheBookPath);

            await File.WriteAllTextAsync(Path.Combine(cacheBookPath, "index.json"), fullResponse);

            foreach (var page in fullBook.Pages)
            {
                await CachePage(provider, fullBook, page, cacheBookPath);
            }
        }

        private static async Task CachePage(IRawProvider provider, Book fullBook, long page, string cacheBookPath)
        {
            var exercisesResponse = await provider.GetExercisesFromPage(fullBook.Id, page);

            if (exercisesResponse == null)
            {
                Logger.Warn("Failed to grab exercises from page {0}", page);
                return;
            }

            await File.WriteAllTextAsync(Path.Combine(cacheBookPath, $"{page:D4}.json"), exercisesResponse);

            Logger.Info("Cached page {0}", page);
        }

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    }
}
