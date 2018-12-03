using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fleur.Models;
using Fleur.RawProviders;
using Newtonsoft.Json;

namespace Fleur.Utils
{
    internal static class RawToModel
    {
        public static List<Book> GetAllBooks(string raw, string[] availableGrades = null)
        {
            var result = JsonConvert.DeserializeObject<List<Book>>(raw);

            // skip books we don't have access to if availableGrades is specified
            if (availableGrades != null)
            {
                for (var i = result.Count - 1; i >= 0; i--)
                {
                    var book = result[i];

                    if (availableGrades.Except(book.Grades).Any())
                    {
                        result.RemoveAt(i);
                    }
                }
            }

            return result;
        }

        public static Book GetBook(string raw)
        {
            return JsonConvert.DeserializeObject<Book>(raw);
        }

        public static Exercise[] GetExercisesFromPage(string raw)
        {
            return JsonConvert.DeserializeObject<Exercise[]>(raw);
        }

        public static UserInfo GetUserInfo(string raw)
        {
            return JsonConvert.DeserializeObject<UserInfo>(raw);
        }
    }
}
