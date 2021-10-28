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
    }


    public class DialogFileSelector : IFileSelector
    {
        private string DefaultExt { get; set; }
        private string DefaultDirectory { get; set; }
        private string DefaultFilter { get; set; }
        public DialogFileSelector(params FileExtInfo[] infos)
        {

        }
        public DialogFileSelector(string defaultExt = ".xml", string defaultFolder = null, string filter = "XML-файлы (*.xml) |*.xml|JSON-файлы (*.json) |*.json| TXT-файлы (*.txt*)|*.txt")
        {
            DefaultExt = defaultExt;
            DefaultDirectory = defaultFolder;
            DefaultFilter = filter;
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
    }

    public class FileExtInfo
    {
        public string Name { get; private set; }
        public string Extension { get; private set; }
        public string Filter { get; private set; }
        public int Priority { get; private set; }

        public FileExtInfo(string name, string ext, int priority = 1)
        {
            Name = name;
            Extension = ext;
            Priority = priority;
        }
    }
}
