using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using DSSLib;

namespace DSSConsole
{
    class Program
    {


        static void Main(string[] args)
        {
            Control();
        }

        static ChoiceAHP ChoiceCurrent { get; set; }

        static DirectoryInfo Directory = new DirectoryInfo(@"C:\Users\Alleaxx\Documents\Программы\Базы данных\Решения АНР");

        static XmlSerializer ChoiceFormatter = new XmlSerializer(typeof(ChoiceAHP));



        static void Control()
        {
            PrintInfo();
            while(Console.ReadLine() is string text && text != "exit")
            {
                switch (text)
                {
                    case "1":
                        Load();
                        break;
                    case "2":
                        Save();
                        break;
                }
                PrintInfo();
            }


            void PrintInfo()
            {
                Console.WriteLine("1) - Загрузка файлов");
                Console.WriteLine("2) - Сохранение файлов файлов");
                Console.WriteLine("exit) - Выход");
            }
        }
        static void Load()
        {
            FileInfo[] files = Directory.GetFiles();

            Console.WriteLine($"Доступно {files.Length} файлов");
            for (int i = 0; i < files.Length; i++)
            {
                Console.WriteLine($"{i + 1}) {files[i].Name}");
            }
            if(files.Length > 0)
            {
                Console.WriteLine("Введите номер файла для открытия решения АНР");
                int index = EnterIndex(1, files.Length);
                FileInfo file = files[index - 1];
                Console.WriteLine($"Открытие {file}");

                XmlSerializer formatter = new XmlSerializer(typeof(ChoiceAHP));
                using (FileStream fs = new FileStream(file.FullName, FileMode.Open))
                {
                    try
                    {
                        ChoiceCurrent = (ChoiceAHP)formatter.Deserialize(fs);
                        ChoiceCurrent.AfterLoad();
                        var check = ChoiceCurrent.CheckAll();
                        Console.WriteLine(check.Result);
                        foreach (string message in check.Messages)
                        {
                            Console.WriteLine(message);
                        }

                        if (check.Success)
                        {
                            ChoiceCurrent.Calculate();
                            ShowInfo();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

        }



        static void ShowInfo()
        {
            ChoiceCurrent.Criterias.ToList().ForEach(c => c.Output());
            ChoiceCurrent.Alternatives.ToList().ForEach(a => a.Output());
            ChoiceCurrent.CriteriaMatrix.Output();
            ChoiceCurrent.AlternativeComparisons.ToList().ForEach(c => c.Value.Output());
            ChoiceCurrent.Output();
        }
        static void Save()
        {
            Console.WriteLine("Введите имя файла, куда сохранить текущее решение АНР");
            string fileName = Console.ReadLine();
            FileInfo newFile = new FileInfo(Directory.FullName + $"\\{fileName}.xml");
            using(FileStream fs = new FileStream(newFile.FullName, FileMode.Create))
            {
                ChoiceFormatter.Serialize(fs, ChoiceCurrent);
                Console.WriteLine($"Файл успешно сохранен в {newFile.FullName}");
            }
        }



        static int EnterIndex(int min, int max)
        {
            int res = -1;
            while(res < min || res > max)
            {
                if (int.TryParse(Console.ReadLine(), out int newRes))
                    res = newRes;
            }
            return res;
        }

    }
}
