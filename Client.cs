using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Fleur.Models;
using Newtonsoft.Json;
using NLog;

namespace Fleur
{
    internal class Client
    {
        public class ModelRawPair<T>
        {
            public ModelRawPair(T model, string rawContent)
            {
                Model = model;
                RawContent = rawContent;
            }

            public T Model { get; }
            public string RawContent { get; }
        }

        private const string BaseAddress = "https://odrabiamy.pl/";

        public Client(string sessionCookie)
        {
            var cookies = new CookieContainer();
            cookies.Add(new Uri("https://odrabiamy.pl"), new Cookie("_dajspisac_session_id", sessionCookie));

            _client = new HttpClient(new HttpClientHandler { UseCookies = true, CookieContainer = cookies });

            _client.DefaultRequestHeaders.Add("User-Agent", @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.87 Safari/537.36");
        }

        public async Task<ModelRawPair<List<Book>>> GetAllBooks(string[] availableGrades = null)
        {
            var response = await _client.GetAsync(BuildRequestUri("api/v1.3/ksiazki"));

            if (!response.IsSuccessStatusCode)
            {
                Logger.Error("Failed to get book index!");
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<Book>>(content);

            // skip books we don't have access too if availableGrades is specified
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

            Logger.Info("Found {0} books.", result.Count);

            return new ModelRawPair<List<Book>>(result, content);
        }

        public async Task<ModelRawPair<Book>> GetBook(long bookId)
        {
            var response = await _client.GetAsync(BuildRequestUri("api/v1.3/ksiazki/{0}", bookId));

            if (!response.IsSuccessStatusCode)
            {
                Logger.Error("Failed to get book ({0})!", bookId);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Book>(content);

            return new ModelRawPair<Book>(result, content);
        }

        public async Task<ModelRawPair<Exercise[]>> GetExercisesFromPage(long bookId, long page)
        {
            var response = await _client.GetAsync(BuildRequestUri("api/v1.3/ksiazki/{0}/zadania/strona/{1}/premium", bookId, page));

            if (!response.IsSuccessStatusCode)
            {
                Logger.Error(
                    response.StatusCode == HttpStatusCode.Unauthorized
                        ? "No premium access! book: {0} {1}"
                        : "Failed to get exercises! book: {0} {1}", bookId, page);

                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Exercise[]>(await response.Content.ReadAsStringAsync());

            return new ModelRawPair<Exercise[]>(result, content);
        }

        public async Task<ModelRawPair<UserInfo>> GetUserInfo()
        {
            var response = await _client.GetAsync(BuildRequestUri("api/v1.3/uzytkownicy/aktualny"));

            if (!response.IsSuccessStatusCode)
            {
                Logger.Error("Failed to get current user info!");
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<UserInfo>(content);

            return new ModelRawPair<UserInfo>(result, content);
        }

        private string BuildRequestUri(string method, params object[] args)
        {
            return string.Format(BaseAddress + method, args);
        }

        private readonly HttpClient _client;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    }
}
