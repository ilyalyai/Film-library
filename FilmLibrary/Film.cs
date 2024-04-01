using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Configuration;
using System.Windows.Forms;
using System.Linq;
using System.Net.Http.Headers;
using System.Xml.Linq;
using FilmLibrary.Properties;
using System.Runtime;
using System.Text.Json;

namespace FilmLibrary
{
    [Serializable]
    public class Film
    {
        public string year;
        public string name;
        public string genre;

        public Film()
        {
        }

        public Film(string y, string n, string g)
        {
            year = y; name = n; genre = g;
        }
    }

    public class FilmList
    {
        async public static void GetGenresAsync()
        {
            using var httpClient = new HttpClient { BaseAddress = new Uri("https://api.kinopoisk.dev/v1/movie/possible-values-by-field") };
            {
                httpClient.DefaultRequestHeaders.Add("X-API-KEY", CreateOrGetToken());
                using var response = await httpClient.GetAsync("?field=genres.name");
                string responseHeaders = response.Headers.ToString();
                string responseData = await response.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(responseData);
                var genresList = new List<string>();
                foreach (JsonElement genreName in jsonDoc.RootElement.EnumerateArray())
                {
                    Console.WriteLine(genreName.GetProperty("name").GetString());
                    genresList.Add(genreName.GetProperty("name").GetString());
                }
            }
        }

        private readonly string token = CreateOrGetToken();

        public readonly List<string> genres;

        public List<Film> list;

        public Film FindRandom(string y, string n, string g)
        {
            List<Film> newList;
            try
            {
                newList = list.FindAll(
                    film => (string.IsNullOrEmpty(n) || film.name.Contains(n))
                            && (string.IsNullOrEmpty(y) || film.year.Equals(y))
                            && (string.IsNullOrEmpty(g) || film.genre.Equals(g)));
            }
            catch (ArgumentNullException)
            {
                return null;
            }

            if (newList.Count == 0)
                return null;
            Random rnd = new Random();
            int r = rnd.Next(newList.Count);
            return newList[r];
        }

        public void Sync()
        {
            list = new List<Film>();

            //Сначала вытащим наш список уже записанных фильмов
            BinaryFormatter _bin = new BinaryFormatter();
            try
            {
                if (File.Exists("E:/Сохранения/FilmList/films.txt"))
                    using (FileStream sw = new FileStream("E:/Сохранения/FilmList/films.txt", FileMode.Open))
                        list = (List<Film>)_bin.Deserialize(sw);
            }
            catch (Exception)
            {
                Console.WriteLine("Файл со списком не найден или поврежден!");
                Environment.Exit(1);
            }
        }

        public void AddNew(Film newbie)
        {
            list ??= new List<Film>();
            list.Add(newbie);
            //И сохраним
            Save();
        }

        private void Save()
        {
            if (list != null)
            {
                BinaryFormatter _bin = new BinaryFormatter();
                using FileStream sw = new FileStream("E:/Сохранения/FilmList/films.txt", FileMode.OpenOrCreate);
                _bin.Serialize(sw, list);
            }
        }

        private static string CreateOrGetToken()
        {
            string token = "";
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var appSettings = configFile.AppSettings.Settings;
            if (appSettings["token"] == null)
            {
                using (var form = new AddToken())
                {
                    var result = form.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        appSettings.Add("token", form.token);
                        configFile.Save(ConfigurationSaveMode.Modified);
                        ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
                        token = form.token;
                    }
                }
            }
            else
                token = appSettings["token"].Value;
            return token;
        }

        public async Task<Film> KinopoiskSearch(string y, string n, string g)
        {
            var baseAddress = new Uri("https://api.kinopoisk.dev/v1.4/movie");
            if (!string.IsNullOrEmpty(n))
            {
                using var httpClient = new HttpClient { BaseAddress = baseAddress };
                {
                    httpClient.DefaultRequestHeaders.Add("X-API-KEY", CreateOrGetToken());
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
                    httpClient.DefaultRequestHeaders.Add("X-API-KEY", CreateOrGetToken());
                    var task = Task.Run(() => httpClient.GetAsync("?limit=10&year=" + y));
                    Task.WaitAll(task);
                    using (var response = task.Result)
                    {
                        string responseHeaders = response.Headers.ToString();
                        string responseData = await response.Content.ReadAsStringAsync();

                        Console.WriteLine("Status " + (int)response.StatusCode);
                        Console.WriteLine("Headers " + responseHeaders);
                        Console.WriteLine("Data " + responseData);
                    }
                }
            }
            else
            {
            }

            return null;
        }
    }
}