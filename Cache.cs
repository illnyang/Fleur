using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fleur.Models;
using Fleur.Utils;
using NLog;

namespace Fleur
{
    internal static class Cache
    {
        public static async Task UpdateCache(Client client)
        {
            var cacheUserPath = Path.Combine(Program.Config.CachePath, "users");
            Directory.CreateDirectory(cacheUserPath);

            var response = await client.GetUserInfo();

            var userInfo = response.Model;

            if (!userInfo.IsPremium)
            {
                throw new Exception($"User {userInfo.Name} does not have premium access to odrabiamy.pl.");
            }

            if (userInfo.ActiveSubscriptions == null || userInfo.ActiveSubscriptions.Length == 0)
            {
                throw new Exception($"User {userInfo.Name} has no subscriptions available.");
            }

            await File.WriteAllTextAsync(Path.Combine(cacheUserPath, $"{userInfo.Id}.json"),
                response.RawContent);

            await Work(client, userInfo.ActiveSubscriptions.Select(sub => sub.Grade).ToArray());
        }

        private static async Task Work(Client client, string[] availableGrades)
        {
            var cacheBookPath = Path.Combine(Program.Config.CachePath, "books");
            Directory.CreateDirectory(cacheBookPath);

            var response = await client.GetAllBooks(availableGrades);

            await File.WriteAllTextAsync(Path.Combine(cacheBookPath, "index.json"), response.RawContent);

            var books = response.Model;

            foreach (var book in books)
            {
                await CacheBook(client, book);
            }
        }

        private static async Task CacheBook(Client client, Book book)
        {
            var fullResponse = await client.GetBook(book.Id);
            var fullBook = fullResponse.Model;

            var uniqueName = $"{fullBook.Name.Trim()} ({fullBook.Kind}) ({fullBook.Released})";

            Logger.Info("\nCaching: {0}\nPages-count: {1}", uniqueName, fullBook.Pages.Length);

            var cacheBookPath = Path.Combine(Program.Config.CachePath, "books", fullBook.Id.ToString());

            if (Directory.Exists(cacheBookPath))
            {
                return;
            }

            Directory.CreateDirectory(cacheBookPath);

            await File.WriteAllTextAsync(Path.Combine(cacheBookPath, "index.json"),
                fullResponse.RawContent);

            foreach (var page in fullBook.Pages)
            {
                await CachePage(client, fullBook, page, cacheBookPath);
            }
        }

        private static async Task CachePage(Client client, Book fullBook, long page, string cacheBookPath)
        {
            var exercisesResponse = await client.GetExercisesFromPage(fullBook.Id, page);

            if (exercisesResponse == null)
            {
                Logger.Warn("Failed to grab exercises from page {0}", page);
                return;
            }

            await File.WriteAllTextAsync(Path.Combine(cacheBookPath, $"{page:D4}.json"),
                exercisesResponse.RawContent);

            Logger.Info("Cached page {0}", page);
        }

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    }
}
