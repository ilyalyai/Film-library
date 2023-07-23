using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

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
        private const string token = Keys.apiToken;

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
            catch (ArgumentNullException e)
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
            catch (Exception e)
            {
                Console.WriteLine("Файл со списком не найден или поврежден!");
                Environment.Exit(1);
            }
        }

        public void addNew(Film newbie)
        {
            if (list == null)
                list = new List<Film>();
            list.Add(newbie);
            //И сохраним
            Save();
        }

        private void Save()
        {
            if (list != null)
            {
                BinaryFormatter _bin = new BinaryFormatter();
                using (FileStream sw = new FileStream("E:/Сохранения/FilmList/films.txt", FileMode.OpenOrCreate))
                    _bin.Serialize(sw, list);
            }
        }

        public async Task<Film> KinopoiskSearch(string y, string n, string g)
        {
            var baseAddress = new Uri("https://сloud-api.kinopoisk.dev");
            if (!string.IsNullOrEmpty(n))
            {
                using var httpClient = new HttpClient { BaseAddress = baseAddress };
                {
                    using (var response = await httpClient.GetAsync("/movie?search=" + n + "&field=name&isStrict=false&token=" + token))
                    {
                        string responseHeaders = response.Headers.ToString();
                        string responseData = await response.Content.ReadAsStringAsync();

                        Console.WriteLine("Status " + (int)response.StatusCode);
                        Console.WriteLine("Headers " + responseHeaders);
                        Console.WriteLine("Data " + responseData);
                    }
                }
            }
            else if (!string.IsNullOrEmpty(y))
            {
                using var httpClient = new HttpClient { BaseAddress = baseAddress };
                {
                    using (var response = await httpClient.GetAsync("/movie?search=" + y + "&field=year&token=" + token))
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