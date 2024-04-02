using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FilmLibrary
{
    static class ApiHelper
    {
        async private static Task<List<string>> GetGenresAsync(BackgroundWorker worker = null)
        {
            var genres = new List<string>();
            using var httpClient = new HttpClient { BaseAddress = new Uri("https://api.kinopoisk.dev/v1/movie/possible-values-by-field") };
            {
                httpClient.DefaultRequestHeaders.Add("X-API-KEY", token);
                worker.ReportProgress(10);
                var task = httpClient.GetAsync("?field=genres.name");
                Task.WaitAll(task);
                worker.ReportProgress(40);
                using var response = task.Result;
                string responseHeaders = response.Headers.ToString();
                string responseData = await response.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(responseData);
                int count = jsonDoc.RootElement.EnumerateArray().Count();
                int i = 0;
                foreach (JsonElement genreName in jsonDoc.RootElement.EnumerateArray())
                {
                    genres.Add(genreName.GetProperty("name").GetString());
                    Console.WriteLine(genreName.GetProperty("name").GetString());
                    worker.ReportProgress(40 + (60/count * ++i));
                }
            }
            return genres;
        }

        public readonly static string token = CreateOrGetToken();

        private static string CreateOrGetToken()
        {
            string token = "";
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var appSettings = configFile.AppSettings.Settings;
            if (appSettings["token"] == null)
            {
                using var form = new AddToken();
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    appSettings.Add("token", form.token);
                    configFile.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
                    token = form.token;
                }
            }
            else
                token = appSettings["token"].Value;
            return token;
        }

        public static List<string> genres;

        async public static void FormGenres(BackgroundWorker worker = null)
        {
            worker.ReportProgress(5);
            genres = GetGenresAsync(worker).Result;
        }

        public static async Task<Film> KinopoiskSearch(string y, string n, string g)
        {
            var baseAddress = new Uri("https://api.kinopoisk.dev/v1.4/movie");
            var token = ApiHelper.token;
            if (!string.IsNullOrEmpty(n))
            {
                using var httpClient = new HttpClient { BaseAddress = baseAddress };
                {
                    httpClient.DefaultRequestHeaders.Add("X-API-KEY", token);
                    using var response = await httpClient.GetAsync("?limit=10&query=" + n);
                    string responseHeaders = response.Headers.ToString();
                    string responseData = await response.Content.ReadAsStringAsync();

                    Console.WriteLine("Status " + (int)response.StatusCode);
                    Console.WriteLine("Headers " + responseHeaders);
                    Console.WriteLine("Data " + responseData);
                }
            }
            else if (!string.IsNullOrEmpty(y))
            {
                using var httpClient = new HttpClient { BaseAddress = baseAddress };
                {
                    httpClient.DefaultRequestHeaders.Add("X-API-KEY", token);
                    var task = Task.Run(() => httpClient.GetAsync("?limit=10&year=" + y));
                    Task.WaitAll(task);
                    using var response = task.Result;
                    string responseHeaders = response.Headers.ToString();
                    string responseData = await response.Content.ReadAsStringAsync();

                    Console.WriteLine("Status " + (int)response.StatusCode);
                    Console.WriteLine("Headers " + responseHeaders);
                    Console.WriteLine("Data " + responseData);
                }
            }
            else if (!string.IsNullOrEmpty(g))
            {
                using var httpClient = new HttpClient { BaseAddress = baseAddress };
                {
                    httpClient.DefaultRequestHeaders.Add("X-API-KEY", token);
                    var task = Task.Run(() => httpClient.GetAsync("?limit=10&genres.name=" + g));
                    Task.WaitAll(task);
                    using var response = task.Result;
                    string responseHeaders = response.Headers.ToString();
                    string responseData = await response.Content.ReadAsStringAsync();

                    Console.WriteLine("Status " + (int)response.StatusCode);
                    Console.WriteLine("Headers " + responseHeaders);
                    Console.WriteLine("Data " + responseData);
                }
            }

            return null;
        }
    }
}
