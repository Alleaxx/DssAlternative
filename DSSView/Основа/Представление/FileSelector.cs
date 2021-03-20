using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    interface IFileSelector
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
        public DialogFileSelector(string defaultExt = ".xml",string defaultFolder = @"C:\Users\Alleaxx\Documents\Программы",string filter = "XML-файлы (*.xml) |*.xml| TXT-файлы (*.txt*)|*.txt")
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

}
