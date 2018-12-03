using System.IO;
using System.Threading.Tasks;
using NLog;

namespace Fleur.RawProviders
{
    internal class CachedRawProvider : IRawProvider
    {
        public CachedRawProvider(string cachePath)
        {
            _cachePath = cachePath;

            var cacheBookPath = Path.Combine(_cachePath, "books");

            if (!Directory.Exists(cacheBookPath))
            {
                throw new DirectoryNotFoundException("No cache to traverse!");
            }
        }

        public async Task<string> GetAllBooks()
        {
            var booksIndexPath = Path.Combine(_cachePath, "books", "index.json");

            if (!File.Exists(booksIndexPath))
            {
                throw new FileNotFoundException("Couldn't not find global index file!");
            }

            return await File.ReadAllTextAsync(booksIndexPath);
        }

        public async Task<string> GetBook(long bookId)
        {
            var indexPath = Path.Combine(_cachePath, "books", bookId.ToString(), "index.json");

            if (!File.Exists(indexPath))
            {
                Logger.Warn("Couldn't not find index file for book id {0}", bookId);
                return null;
            }

            return await File.ReadAllTextAsync(indexPath);
        }

        public async Task<string> GetExercisesFromPage(long bookId, long page)
        {
            var pagePath = Path.Combine(_cachePath, "books", bookId.ToString(), $"{page:D4}.json");

            if (!File.Exists(pagePath))
            {
                Logger.Warn("Couldn't not find page file for book id {0}, page {1}", bookId, page);
                return null;
            }

            return await File.ReadAllTextAsync(pagePath);
        }

        public async Task<string> GetUserInfo(long userId = 0)
        {
            var userPath = Path.Combine(_cachePath, "users", $"{userId}.json");

            if (!File.Exists(userPath))
            {
                Logger.Warn("Couldn't not find user file for id {0}", userId);
                return null;
            }

            return await File.ReadAllTextAsync(userPath);
        }

        private readonly string _cachePath;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    }
}
