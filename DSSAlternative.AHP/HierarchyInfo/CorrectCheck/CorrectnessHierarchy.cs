using DSSAlternative.AHP.HierarchyInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    /// <summary>
    /// Результат проверки на корректность структуры иерархии
    /// </summary>
    public class CorrectnessHierarchy : Correctness
    {
        private readonly IHierarchy Hierarchy;
        public CorrectnessHierarchy(IHierarchy hierarchy)
        {
            Hierarchy = hierarchy;
        }

        protected override IEnumerable<ICheckResult> CreateChecks()
        {
            List<ICheckResult> results = new List<ICheckResult>();

            AddElementsExistingChecks(results);
            AddElementsPlacementChecks(results);
            AddElementsAmountChecks(results);

            return results;
        }
        private void AddElementsExistingChecks(List<ICheckResult> results)
        {
            string name = "Наличие";
            if (Hierarchy.Nodes.Count() < 2)
            {
                AddFail(results, name, "Отстутствует иерархия");
            }
            if (Hierarchy.MainGoal == null)
            {
                AddFail(results, name, "Отсутствует главная цель проблемы");
            }
            //if (!Hierarchy.Criterias.Any())
            //{
            //    AddWarning(name, "Отсутствуют критерии");
            //}
            //if (!Hierarchy.Alternatives.Any())
            //{
            //    AddFail(name, "Отсутствуют альтернативы");
            //}
            if (Hierarchy.MainGoal != null && !Hierarchy.Nodes.Any(n => n.GroupOwner == Hierarchy.MainGoal.Group))
            {
                AddFail(results, name, "Отсутствуют элементы, подчиненные главной цели");
            }
            if (Hierarchy.Nodes.Any(n => n.Group == n.GroupOwner))
            {
                AddFail(results, name, "Есть элементы, зависимые от самих себя");
            }
            if (Hierarchy.Nodes.Any(n => !Hierarchy.GetGroupsOfNodes().Contains(n.Group)))
            {
                AddFail(results, name, "Есть элементы, зависимые от несуществующих групп");
            }
            //if (!Hierarchy.Any(n => Hierarchy.Any(n2 => n2.GroupIndex == n.Group)))
            //{
            //    AddFail(name, "Нет конечных альтернатив");
            //}
        }
        private void AddElementsPlacementChecks(List<ICheckResult> results)
        {
            string name = "Последовательность";
            int lastLevel = Hierarchy.NodesGroupedByLevel().Last().Key;
            int levelsAmount = Hierarchy.NodesGroupedByLevel().Count();

            if (lastLevel != levelsAmount - 1)
            {
                AddFail(results, name, "Нарушена последовательность уровней");
            }
        }
        private void AddElementsAmountChecks(List<ICheckResult> results)
        {
            string name = "Количество узлов";
            foreach (var group in Hierarchy.NodesGroupedByLevel())
            {
                int level = group.Key;
                int levelAmount = group.Count();

                if (level == 0 && levelAmount != 1)
                {
                    AddFail(results, name, "На главном уровне иерархии больше одной цели");
                }
                else if (level > 0 && levelAmount < 2)
                {
                    AddFail(results, name, $"На {level} уровне иерархии меньше 2-х элементов");
                }
            }
        }

        //Должны быть узлы, которые не являются ни для кого критериями

        //Проверка на группировку...
    }
}
