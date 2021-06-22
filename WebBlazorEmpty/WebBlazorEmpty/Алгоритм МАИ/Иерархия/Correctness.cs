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
    public interface ICheck : IStyled
    {
        string Name { get; }
        bool Passed { get; }
        string Message { get; }
    }
    public class CheckResult : ICheck
    {
        public string Name { get; private set; }
        public bool Passed { get; private set; }
        public string Message { get; private set; }
        private string Class { get; set; }
        public CheckResult(string name, string cl) : this(name, cl, true, "Проверка прошла успешно")
        {

        }
        public CheckResult(string name, string cl, bool passed, string message)
        {
            Name = name;
            Passed = passed;
            Message = message;
            Class = cl;
        }

        public string GetClass() => Passed ? $"passed {Class}" : $"errored {Class}";
        public string GetStyle() => "";
    }
}
