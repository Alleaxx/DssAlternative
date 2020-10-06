using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using DSSLib;

namespace DSSConsole
{
    class Command
    {
        public static List<Command> Current { get; set; } = new List<Command>();
        public static List<Command> Selection { get; set; } = new List<Command>();
        public static void Add(List<Command> list,string input, string name, Action action)
        {
            list.Add(new Command() { Input = input, Name = name, Action = action });
        }
        public static Command GetCommand(List<Command> list,string input) => list.Find(c => c.Input == input);



        public string Name { get; set; }
        public string Input { get; set; }
        public Action Action { get; set; }

        private Command()
        {

        }


        public string Description => $"{Input} - {Name}";
    }
    class Program
    {
        static void Main(string[] args)
        {
            Command.Add(Command.Selection,"1","Загрузить проблему",Load);
            Command.Add(Command.Selection,"2","Сохранить проблему",Save);
            Command.Add(Command.Selection,"3","Показать текущую проблему",() => Control(Command.Current));
            Command.Add(Command.Selection,"4","Установить образцовую проблему (МАИ)",SetExampleProblemAHP);
            Command.Add(Command.Selection,"5","Установить образцовую проблему (критерии)",SetExampleProblemCriterias);
            Command.Add(Command.Selection,"6","Установить образцовую проблему (деревья)",SetExampleProblemTree);
            Command.Add(Command.Selection,"7","Создать валютную проблему",CreateCurrencyProblem);

            Command.Add(Command.Current, "1", "Показать проблему целиком", () => ProblemCurrent.Output());
            Command.Add(Command.Current, "2", "Показать альтернативы проблемы", () => ProblemCurrent.Alternatives.ForEach(p => p.Output()));
            Command.Add(Command.Current, "3", "Показать критерии проблемы", () => ProblemCurrent.Criterias.ForEach(p => p.Output()));
            Command.Add(Command.Current, "4", "Показать исходы проблемы", () => ProblemCurrent.Cases.ForEach(p => p.Output()));
            Command.Add(Command.Current, "5", "Показать решение по МАИ", ShowDecisionAHP);
            Command.Add(Command.Current, "6", "Показать решение по критериям", ShowDecisionCr);
            Command.Add(Command.Current, "7", "Показать дерево решений", ShowDecisionTree);

            Control(Command.Selection);
        }

        static Problem ProblemCurrent { get; set; }

        static DirectoryInfo Directory = new DirectoryInfo(@"C:\Users\Alleaxx\Documents\Программы\Базы данных\Решения АНР");

        static XmlSerializer ProblemFormatter = new XmlSerializer(typeof(Problem));
        
        //Управление программой
        static void Control(List<Command> commands)
        {
            PrintInfo(commands);
            while(Console.ReadLine() is string text && Command.GetCommand(commands,text) is Command command)
            {
                command.Action?.Invoke();
                PrintInfo(commands);
            }
        }
        static void PrintInfo(IEnumerable<Command> commands)
        {
            string problemCurrent = ProblemCurrent != null ? ProblemCurrent.Name : "отсутствует";
            Console.WriteLine("-----------------------");
            Console.WriteLine($"ТЕКУЩАЯ ПРОБЛЕМА - {problemCurrent}");
            Console.WriteLine("Доступные действия:");
            commands.ToList().ForEach(v => Console.WriteLine(v.Description));
            Console.WriteLine("-----------------------");
        }

        //Установить образцовую проблему
        static void SetExampleProblemAHP()
        {
            string Name = "Выбор университета";
            Criteria place = new Criteria("Place","Место", 7);
            Criteria reputation = new Criteria("Rep","Репутация", 3);
            Criteria program = new Criteria("Programm","Программа", 9);
            Alternative GUU = new Alternative("GUU","ГУУ").SetCriteriasPrior((place, 9), (reputation, 5), (program, 5));
            Alternative GAIK = new Alternative("GAIK","МИИГАИК").SetCriteriasPrior((place, 5), (reputation, 5), (program, 5));
            Alternative STANKIN = new Alternative("STANKIN","СТАНКИН").SetCriteriasPrior((place, 3), (reputation, 3), (program, 7));
            ProblemCurrent = new Problem()
            {
                Name = Name,
                Criterias = new List<Criteria>() { place,reputation,program },
                Alternatives = new List<Alternative>() { GUU, GAIK, STANKIN }
            };
            Control(Command.Current);
        }
        static void SetExampleProblemTree()
        {
            string Name = "Вложение в инвестиционные фонды";
            Alternative A1 = new Alternative("A1", "Фонд A1").SetCaseAccept((new Case("1","1",0.6), 500),(new Case("2","2",0.4), -200));
            Alternative A2 = new Alternative("A2", "Фонд A2").SetCaseAccept((new Case("3","3",0.6), 250),(new Case("4","4",0.4), -100));
            Alternative A3 = new Alternative("A3", "Фонд A3").SetCaseAccept((new Case("5","5",0.6), 120),(new Case("6","6",0.4), -50));
            ProblemCurrent = new Problem()
            {
                Name = Name,
                Alternatives = new List<Alternative>() { A1, A2, A3 }
            };
            Control(Command.Current);
        }
        static void SetExampleProblemCriterias()
        {
            string Name = "Выбор управленческой стратегии для склада";
            Case c1 = new Case("C1","C1 тыс. комплектов", 0.1);
            Case c2 = new Case("C2","C2 тыс. комплектов", 0.3);
            Case c3 = new Case("C3","C3 тыс. комплектов", 0.4);
            Case c4 = new Case("C4","C4 тыс. комплектов", 0.2);
            Alternative A1 = new Alternative("A1","A1 Склад 60 м2").SetCaseProfits((c1,68), (c2, 69), (c3,71), (c4, 78));
            Alternative A2 = new Alternative("A2","A2 Склад 90 м2").SetCaseProfits((c1,72), (c2, 66), (c3,68), (c4, 55));
            Alternative A3 = new Alternative("A3","A3 Склад 140 м2").SetCaseProfits((c1,68), (c2, 56), (c3,67), (c4, 73));
            Alternative A4 = new Alternative("A4","A4 Склад 180 м2").SetCaseProfits((c1,60), (c2, 77), (c3,54), (c4, 67));
            Alternative A5 = new Alternative("A5","A5 Склад 220 м2").SetCaseProfits((c1,59), (c2, 78), (c3,62), (c4, 56));
            ProblemCurrent = new Problem()
            {
                Name = Name,
                Cases = new List<Case>() { c1,c2,c3,c4 },
                Alternatives = new List<Alternative>() { A1,A2,A3,A4,A5 }
            };
            Control(Command.Current);

        }

        static void CreateCurrencyProblem()
        {
            Console.WriteLine("Введите сумму");
            int sum = int.Parse(Console.ReadLine());
            Console.WriteLine("Введите первую валюту");
            string val1 = Console.ReadLine();
            Console.WriteLine("Введите вторую валюту");
            string val2 = Console.ReadLine();
            ProblemCurrent = new CurrencyProblem(sum, val1, val2);
            Control(Command.Current);
        }


        //Загрузить проблему
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
                Console.WriteLine("Введите номер файла для открытия проблемы");
                int index = EnterIndex(1, files.Length);
                FileInfo file = files[index - 1];
                Console.WriteLine($"Открытие {file}");

                using (FileStream fs = new FileStream(file.FullName, FileMode.Open))
                {
                    try
                    {
                        ProblemCurrent = (Problem)ProblemFormatter.Deserialize(fs);
                        ProblemCurrent.AfterLoad();
                        Control(Command.Current);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

        }

        //Отобразить информацию по текущей проблеме
        static void ShowDecisionAHP()
        {
            var check = DecisionAHP.CheckAll(ProblemCurrent);
            Console.WriteLine(check.Result);
            foreach (string message in check.Messages)
            {
                Console.WriteLine(message);
            }

            if (check.Success)
            {
                ProblemCurrent.GetDesizionAHP().Output();
            }
        }
        static void ShowDecisionCr()
        {
            var check = DecisionCriterias.CheckAll(ProblemCurrent);
            Console.WriteLine(check.Result);
            foreach (string message in check.Messages)
            {
                Console.WriteLine(message);
            }

            if (check.Success)
            {
                ProblemCurrent.GetDesizionCR().Output();
            }
        }
        static void ShowDecisionTree()
        {
            var check = DecisionTree.CheckAll(ProblemCurrent);
            Console.WriteLine(check.Result);
            foreach (string message in check.Messages)
            {
                Console.WriteLine(message);
            }

            if (check.Success)
            {
                ProblemCurrent.GetDesizionTree().Output();
            }
        }

        //Сохранить проблему в текущий файл
        static void Save()
        {
            Console.WriteLine("Введите имя файла, куда сохранить текущую проблему");
            string fileName = Console.ReadLine();
            FileInfo newFile = new FileInfo(Directory.FullName + $"\\{fileName}.xml");
            using(FileStream fs = new FileStream(newFile.FullName, FileMode.Create))
            {
                ProblemFormatter.Serialize(fs, ProblemCurrent);
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
