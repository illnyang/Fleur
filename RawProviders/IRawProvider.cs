using System.Threading.Tasks;

namespace Fleur.RawProviders
{
    interface IRawProvider
    {
        Task<string> GetAllBooks();
        Task<string> GetBook(long bookId);
        Task<string> GetExercisesFromPage(long bookId, long page);
        Task<string> GetUserInfo(long userId = 0);
    }
}
