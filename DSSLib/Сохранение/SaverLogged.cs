using System;
using System.Collections.Generic;
using System.Text;

namespace DSSLib
{
    //Декоратор с логами
    public class SaverLogged<T> : Saver<T>
    {
        private ISaver<T> Saver { get; set; }
        public SaverLogged(ISaver<T> saver)
        {
            Saver = saver;
        }
        public override T Open(string path)
        {
            DateTime now = DateTime.Now;
            var res = Saver.Open(path);
            Console.WriteLine($"По пути {path} был открыт объект {res} за {(DateTime.Now - now).TotalMilliseconds} мс");
            return res;
        }
        public override void Save(string path, T obj)
        {
            DateTime now = DateTime.Now;
            Console.WriteLine($"По пути {path} был сохранен объект {obj} за {(DateTime.Now - now).TotalMilliseconds} мс");
            Saver.Save(path, obj);
        }
    }
}
