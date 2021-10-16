using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{

    public class HierarchyCheck : Correctness
    {
        private readonly IHierarchy Hierarchy;
        public HierarchyCheck(IHierarchy hierarchy)
        {
            Hierarchy = hierarchy;
        }

        protected override void OwnCheck()
        {
            CheckElementsExisting();
            CheckElementsPlacement();
            CheckElementsAmount();
        }
        private void CheckElementsExisting()
        {
            string name = "Наличие";
            if (Hierarchy.Hierarchy.Count() < 2)
            {
                AddFail(name, "Отстутствует иерархия");
            }
            if (Hierarchy.MainGoal == null)
            {
                AddFail(name, "Отсутствует главная цель проблемы");
            }
            if (!Hierarchy.Criterias.Any())
            {
                AddWarning(name, "Отсутствуют критерии");
            }
            if (!Hierarchy.Alternatives.Any())
            {
                AddFail(name, "Отсутствуют альтернативы");
            }
        }
        private void CheckElementsPlacement()
        {
            string name = "Последовательность";
            int lastLevel = Hierarchy.GroupedByLevel.Last().Key;
            int levelsAmount = Hierarchy.GroupedByLevel.Count();

            if (lastLevel != levelsAmount - 1)
            {
                AddFail(name, "Нарушена последовательность уровней");
            }
        }
        private void CheckElementsAmount()
        {
            string name = "Количество узлов";
            foreach (var group in Hierarchy.GroupedByLevel)
            {
                int level = group.Key;
                int levelAmount = group.Count();

                if (level == 0 && levelAmount != 1)
                {
                    AddFail(name, "На главном уровне иерархии больше одной цели");
                }
                else if (level > 0 && levelAmount < 2)
                {
                    AddFail(name, $"На {level} уровне иерархии меньше 2-х элементов");
                }
            }
        }
    }
}
