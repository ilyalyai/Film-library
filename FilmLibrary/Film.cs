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
    }
}