using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DSSLib
{
    public interface IFileSelector
    {
        FileInfo Open();
        FileInfo Save();
        FileInfo SaveAs();
        FileInfo Create();
    }
    public class DialogFileSelector : IFileSelector
    {
        private string DefaultExt { get; set; }
        private string DefaultDirectory { get; set; }
        private string DefaultFilter { get; set; }
        public DialogFileSelector(string defaultExt = ".xml", string defaultFolder = @"C:\Users\Alleaxx\Documents\Программы", string filter = "XML-файлы (*.xml) |*.xml| TXT-файлы (*.txt*)|*.txt")
        {
            DefaultExt = defaultExt;
            DefaultDirectory = defaultFolder;
            DefaultFilter = filter;
        }

        public FileInfo Create()
        {
            throw new NotImplementedException();
        }

        public FileInfo Open()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = DefaultExt;
            dialog.InitialDirectory = DefaultDirectory;
            dialog.Filter = DefaultFilter;

            if (dialog.ShowDialog() == true)
            {
                return new FileInfo(dialog.FileName);
            }
            return null;
        }

        public FileInfo Save()
        {

            SaveFileDialog dialog = new SaveFileDialog();
            //dialog.FileName = $"{CurrentData.FileSource.Name}";
            dialog.DefaultExt = DefaultExt;
            dialog.InitialDirectory = DefaultDirectory;
            dialog.Filter = DefaultFilter;

            if (dialog.ShowDialog() == true)
            {
                return new FileInfo(dialog.FileName);
            }
            return null;
        }

        public FileInfo SaveAs()
        {
            throw new NotImplementedException();
        }
    }

    public interface ISaver<T>
    {
        T OpenFromFile();
        void SaveToFile(T obj);
    }
    public class DefaultSaver<T> : ISaver<T>
    {
        public T OpenFromFile()
        {
            IXMLProvider<T> provider = new DefaultXmlProvider<T>();
            IFileSelector selector = new DialogFileSelector();
            FileInfo file = selector.Open();

            if (file != null && file.Exists)
            {
                using (FileStream stream = File.OpenRead(file.FullName))
                {
                    byte[] array = new byte[stream.Length];
                    stream.Read(array, 0, array.Length);
                    string xml = Encoding.Default.GetString(array);

                    T problemProject = provider.FromXml(xml);
                    return problemProject;
                }
            }
            return default;
        }

        public void SaveToFile(T obj)
        {
            IXMLProvider<T> provider = new DefaultXmlProvider<T>();
            IFileSelector selector = new DialogFileSelector();
            FileInfo file = selector.Save();

            if (file != null)
            {
                string xml = provider.ToXml(obj);
                using (FileStream stream = new FileStream(file.FullName, FileMode.Create))
                {
                    byte[] array = Encoding.Default.GetBytes(xml);
                    stream.Write(array, 0, array.Length);
                }
            }
        }
    }
}
