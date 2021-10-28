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

        public Saver() : this(new DialogFileSelector(), new XmlDefaultProvider<T>())
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


        public static Saver<T> CreateJsonSaver()
        {
            return new Saver<T>(new DialogFileSelector(".json"), new JsonDefaultProvider<T>());
        }
        public static Saver<T> CreateXmlSaver()
        {
            return new Saver<T>(new DialogFileSelector(".json"), new JsonDefaultProvider<T>());
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
}
