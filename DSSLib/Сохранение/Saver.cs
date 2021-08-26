using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DSSLib
{
    public interface ISaver<T>
    {
        T Open();
        T Open(string path);

        void Save(T obj);
        void Save(string path, T obj);
    }
    public class Saver<T> : ISaver<T>
    {
        protected IFileSelector FileDialog { get; set; }
        protected ITextProvider<T> Provider { get; set; }

        public Saver() : this(new DialogFileSelector(), new XmlProvider<T>())
        {

        }
        public Saver(ITextProvider<T> provider) : this(new DialogFileSelector(), provider)
        {

        }
        public Saver(IFileSelector selector, ITextProvider<T> provider)
        {
            FileDialog = selector;
            Provider = provider;
        }


        public virtual T Open()
        {
            FileInfo file = FileDialog.Open();
            if (file != null && file.Exists)
            {
                return Open(file.FullName);
            }
            return default;
        }
        public virtual T Open(string path)
        {
            string xml = File.ReadAllText(path, Encoding.Default);
            T obj = Provider.FromTextString(xml);
            return obj;
        }
        public virtual void Save(T obj)
        {
            FileInfo file = FileDialog.Save();
            if (file != null)
            {
                Save(file.FullName, obj);
            }
        }
        public virtual void Save(string path, T obj)
        {
            string xml = Provider.ToTextString(obj);
            File.WriteAllText(path, xml, Encoding.Default);
        }
    }

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
