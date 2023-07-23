using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllClasses
{
    interface IFilm
    {
        string genre { get; set; } //жанр фильма для поиска

        DateTime PremiereDate { get; set; } //дата выхода
    }
}
