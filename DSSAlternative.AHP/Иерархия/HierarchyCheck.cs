﻿using System;
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
            if (Hierarchy.Nodes.Count() < 2)
            {
                AddFail(name, "Отстутствует иерархия");
            }
            if (Hierarchy.MainGoal == null)
            {
                AddFail(name, "Отсутствует главная цель проблемы");
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
                AddFail(name, "Отсутствуют элементы, подчиненные главной цели");
            }
            if (Hierarchy.Nodes.Any(n => n.Group == n.GroupOwner))
            {
                AddFail(name, "Есть элементы, зависимые от самих себя");
            }
            if (Hierarchy.Nodes.Any(n => !Hierarchy.GroupsOfNodes().Contains(n.Group)))
            {
                AddFail(name, "Есть элементы, зависимые от несуществующих групп");
            }
            //if (!Hierarchy.Any(n => Hierarchy.Any(n2 => n2.GroupIndex == n.Group)))
            //{
            //    AddFail(name, "Нет конечных альтернатив");
            //}
        }
        private void CheckElementsPlacement()
        {
            string name = "Последовательность";
            int lastLevel = Hierarchy.NodesGroupedByLevel().Last().Key;
            int levelsAmount = Hierarchy.NodesGroupedByLevel().Count();

            if (lastLevel != levelsAmount - 1)
            {
                AddFail(name, "Нарушена последовательность уровней");
            }
        }
        private void CheckElementsAmount()
        {
            string name = "Количество узлов";
            foreach (var group in Hierarchy.NodesGroupedByLevel())
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

        //Должны быть узлы, которые не являются ни для кого критериями

        //Проверка на группировку...
    }
}
