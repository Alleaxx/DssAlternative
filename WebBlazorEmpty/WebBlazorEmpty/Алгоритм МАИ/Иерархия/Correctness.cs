using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public interface ICorrectness
    {
        bool Result { get; }
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
            if (Hierarchy.GroupedByLevel.Last().Key != Hierarchy.GroupedByLevel.Count() - 1)
                SetFailed("Нарушена последовательность уровней");
        }
        private void CheckElementsAmount()
        {
            if (Hierarchy.Hierarchy.Where(n => n.Level == 0).Count() > 1)
                SetFailed("Больше одного узла на главном уровне проблемы");
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
            Checks.Add(new CheckResult(comment[0].ToString(), "hier-fail", false, comment));
        }


        public bool Result => Checks.TrueForAll(c => c.Passed);
        public List<ICheck> Checks { get; private set; } = new List<ICheck>();
    }
}
