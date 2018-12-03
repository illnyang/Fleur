using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NLog;

namespace Fleur.RawProviders
{
    internal class NetworkedRawProvider : IRawProvider
    {
        private const string BaseAddress = "https://odrabiamy.pl/";

        public NetworkedRawProvider(string sessionCookie)
        {
            var cookies = new CookieContainer();
            cookies.Add(new Uri("https://odrabiamy.pl"), new Cookie("_dajspisac_session_id", sessionCookie));

            _client = new HttpClient(new HttpClientHandler { UseCookies = true, CookieContainer = cookies });

            _client.DefaultRequestHeaders.Add("User-Agent", @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.87 Safari/537.36");
        }

        public async Task<string> GetAllBooks()
        {
            var response = await _client.GetAsync(BuildRequestUri("api/v1.3/ksiazki"));

            if (!response.IsSuccessStatusCode)
            {
                Logger.Error("Failed to get book index!");
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetBook(long bookId)
        {
            var response = await _client.GetAsync(BuildRequestUri("api/v1.3/ksiazki/{0}", bookId));

            if (!response.IsSuccessStatusCode)
            {
                Logger.Error("Failed to get book ({0})!", bookId);
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetExercisesFromPage(long bookId, long page)
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

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetUserInfo(long userId = 0)
        {
            var response = await _client.GetAsync(BuildRequestUri("api/v1.3/uzytkownicy/aktualny"));

            if (!response.IsSuccessStatusCode)
            {
                Logger.Error("Failed to get current user info!");
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }

        private string BuildRequestUri(string method, params object[] args)
        {
            return string.Format(BaseAddress + method, args);
        }

        private readonly HttpClient _client;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    }
}
