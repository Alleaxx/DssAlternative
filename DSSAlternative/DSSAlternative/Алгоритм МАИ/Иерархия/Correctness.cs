using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public interface ICorrectness
    {
        bool IsCorrect { get; }
        List<ICheck> Checks { get; }
    }

    public class HierarchyCorrectness : ICorrectness
    {
        private IHierarchy Hierarchy { get; set; }

        public HierarchyCorrectness(IHierarchy hierarchy)
        {
            Hierarchy = hierarchy;
            Check();
        }

        private void Check()
        {
            Checks.Clear();

            CheckElementsExisting();
            CheckElementsPlacement();
            CheckElementsAmount();
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
            int lastLevel = Hierarchy.GroupedByLevel.Last().Key;
            int levelsAmount = Hierarchy.GroupedByLevel.Count();

            if (lastLevel != levelsAmount - 1)
                SetFailed("Нарушена последовательность уровней");
        }
        private void CheckElementsAmount()
        {
            foreach (var group in Hierarchy.GroupedByLevel)
            {
                int level = group.Key;
                int levelAmount = group.Count();

                if (level == 0 && levelAmount != 1)
                    SetFailed("На главном уровне иерархии не 1 элемент");
                else if (level > 0 && levelAmount < 2)
                    SetFailed($"На {level} уровне иерархии меньше 2-х элементов");
            }
        }


        private void SetFailed(string comment)
        {
            string shortName = comment[0].ToString();
            Checks.Add(new CheckResult(shortName, "hier-fail", false, comment));
        }


        public bool IsCorrect => Checks.TrueForAll(c => c.Passed);
        public List<ICheck> Checks { get; private set; } = new List<ICheck>();
    }
}
