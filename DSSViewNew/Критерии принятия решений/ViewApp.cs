using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using DSSLib;

namespace DSSView
{
    //Экземпляр приложения
    public class View
    {
        public static ViewMatrix Matrix { get; set; }
    }

    //Представление приложения
        //- Коллекция статистических игр

    //Статистическая игра
        //- Матрица
            //- Действия
            //- Случаи
        //- Итоги по критериям (использует матрицу)
            //- Вальд
            //- Гурвиц...
        //- Текстовый отчет (использует итоги и матрицу)
}
