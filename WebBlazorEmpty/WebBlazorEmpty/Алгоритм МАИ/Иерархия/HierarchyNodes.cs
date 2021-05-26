using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebBlazorEmpty.AHP
{
    public interface IHierarchy
    {
        IEnumerable<INode> Hierarchy { get; set; }
    }

    public class HierarchyNodes : IHierarchy
    {
        public override string ToString() => $"{MainGoal.Name} [{GroupedByLevel.Count()}] ({Hierarchy.Count()})";
        public HierarchyNodes(IEnumerable<INode> nodes)
        {
            Hierarchy = nodes;
        }

        public IEnumerable<INode> Hierarchy { get; set; }

        //Всего узлов
        public int NodesCount => Hierarchy.Count();
        public int LevelsCount => GroupedByLevel.Count();
        public int RelationsCount
        {
            get
            {
                var groupedLevel = GroupedByLevel;
                int amount = 0;

                int prevAmount = 1;
                foreach (var group in groupedLevel)
                {
                    amount += prevAmount * (group.Count() * group.Count() / 3 );
                    prevAmount = group.Count();
                }
                return amount;
            }
        }
        public int MaxLevel => Hierarchy.Select(s => s.Level).Max();


        //Является ли иерархия корректной для применения в МАИ
        public HierarchyCorrectness Correctness => new HierarchyCorrectness(this);


        //Сгруппированные по уровням
        public IEnumerable<IGrouping<int, INode>> GroupedByLevel => Hierarchy.OrderBy(n => n.Level).GroupBy(h => h.Level);
        protected Dictionary<int, INode[]> GetDictionary()
        {
            Dictionary<int, INode[]> dictionary = new Dictionary<int, INode[]>();
            foreach (var nodeGroup in GroupedByLevel)
            {
                dictionary.Add(nodeGroup.Key, nodeGroup.ToArray());
            }
            return dictionary;
        }

        //Узлы
        public INode MainGoal => Hierarchy.Where(h => h.Level == 0).FirstOrDefault();
        public IEnumerable<INode> Criterias => Hierarchy.Where(h => h.Level > 0 && h.Level < LevelsCount - 1);
        public IEnumerable<INode> Alternatives => Hierarchy.Where(h => h.Level == LevelsCount - 1);
        public IEnumerable<INode> HardNodes => Criterias.Prepend(MainGoal);
        


        //Сложность?
        //Ожидаемое время решения проблемы



        public string GetTextInfo(int level)
        {
            if (level == 0)
                return "Цель";
            else if (level == MaxLevel)
                return "Альтернативы";
            else if (level == 1)
                return "Критерии";
            else
                return "Подкритерии";
        }


        public static bool CompareEqual(HierarchyNodes a, HierarchyNodes b)
        {
            if (a == null || b == null)
                return false;

            var aNodes = a.Hierarchy.ToList();
            var bNodes = b.Hierarchy.ToList();

            if (aNodes.Count() != bNodes.Count())
                return false;

            foreach (var aNode in a.Hierarchy)
            {
                if (!bNodes.Exists(n => n.Name == aNode.Name && n.Level == aNode.Level))
                    return false;
            }
            return true;
        }
    }





    public class HierarchyCorrectness
    {
        private HierarchyNodes Hierarchy { get; set; }

        public HierarchyCorrectness(HierarchyNodes hierarchy)
        {
            Hierarchy = hierarchy;
            Check();
        }
        public void Check()
        {
            Result = true;
            Comments.Clear();

            CheckElementsExisting();
            CheckElementsPlacement();
            CheckElementsAmount();

            if(Result)
                SetSuccess();

        }
        private void CheckElementsExisting()
        {
            if (Hierarchy.MainGoal == null)
                SetFailed("Отсутствует главная цель проблемы");
            if (Hierarchy.Criterias.Count() == 0)
                SetFailed("Отсутствуют критерии");
            if (Hierarchy.Alternatives.Count() == 0)
                SetFailed("Отсутствуют альтернативы");

        }
        private void CheckElementsPlacement()
        {
            if (Hierarchy.Hierarchy.Where(n => n.Level == 0).Count() > 1)
                SetFailed("Больше одного узла на главном уровне проблемы");
            if (Hierarchy.GroupedByLevel.Last().Key != Hierarchy.GroupedByLevel.Count() - 1)
                SetFailed("Нарушена последовательность уровней");
        }
        private void CheckElementsAmount()
        {
            foreach (var group in Hierarchy.GroupedByLevel)
            {
                int level = group.Key;

                if (level == 0)
                    continue;
                else if (group.Count() < 2)
                    SetFailed($"На {level} уровне иерархии меньше 2-х элементов");
            }
        }

        
        private void SetFailed(string comment)
        {
            Set(false, comment);
        }
        private void SetSuccess()
        {
            Set(true, "Проверка прошла успешно");
        }

        private void Set(bool correct, string commentary)
        {
            Result = correct;
            Comments.Add(commentary);
        }


        public bool Result { get; set; }
        public List<string> Comments { get; set; } = new List<string>();
    }
}
