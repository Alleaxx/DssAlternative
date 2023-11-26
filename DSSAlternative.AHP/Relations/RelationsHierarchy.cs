using DSSAlternative.AHP.HierarchyInfo;
using DSSAlternative.AHP.Logs;
using DSSAlternative.AHP.MatrixMethods;
using DSSAlternative.AHP.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSAlternative.AHP.Relations
{
    /// <summary>
    /// Интерфейс отношений для удобной создания матрицы
    /// </summary>
    public interface IRelationsGrouped
    {
        IEnumerable<IGrouping<INode, IRelationNode>> RelationsGroupedMain(INode node);
    }

    /// <summary>
    /// Список отношений для целой иерархии
    /// Пересоздается при изменении текущей коллекции в проекте
    /// </summary>
    public interface IRelationsHierarchy : IRelationsGrouped, IUsable
    {
        /// <summary>
        /// Иерархия с узлами, для которой созданы отношения
        /// </summary>
        IHierarchy HierarchyContext { get; }

        /// <summary>
        /// Возникает при изменения значения одного из внутренних отношений
        /// </summary>
        event Action<IRelationsHierarchy, IRelationsCriteria, IRelationNode> OnInnerRelationValueChanged;

        /// <summary>
        /// Список отношений узлов-критериев
        /// </summary>
        IEnumerable<IRelationsCriteria> RelationsCriteria { get; }
        
        /// <summary>
        /// Получить отношения конкретно узла
        /// </summary>
        IRelationsCriteria this[INode main] { get; }

        /// <summary>
        /// Установить значение по главному критерию в сравнении указанных узлов
        /// </summary>
        void SetValue(INode main, INode from, INode to, double value);

        /// <summary>
        /// Сбросить все отношения
        /// </summary>
        void SetAllUnknown();

        /// <summary>
        /// Загрузить значения отношений из шаблона
        /// </summary>
        /// <param name="template"></param>
        void SetValuesFromTemplate(ITemplateProject template);
    }



    /// <summary>
    /// Список отношений для иерархии
    /// Пересоздается при изменении текущей коллекции в проекте
    /// </summary>
    public class RelationsHierarchy : List<IRelationsCriteria>, IRelationsHierarchy
    {
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Отношения из {Count} критериев | корректность: {Correct}, согласованность {Consistent}, известность {Known}");
            foreach (var criteria in this)
            {
                sb.Append($"\n{criteria}");
            }
            return sb.ToString();
        }

        public event Action<IRelationsHierarchy, IRelationsCriteria, IRelationNode> OnInnerRelationValueChanged;

        public IHierarchy HierarchyContext { get; init; }
        private Dictionary<INode, IRelationsCriteria> RelationsCriteriasDictionary;
        public IEnumerable<IRelationsCriteria> RelationsCriteria => this;



        //Создание критериев и отношений
        public RelationsHierarchy(IHierarchy hierarchy)
        {
            HierarchyContext = hierarchy;
            CreateCriterias();
            RecountGroupCoefficients();
        }
        private void CreateCriterias()
        {
            foreach (var node in HierarchyContext.Nodes)
            {
                var criteria = new RelationsCriteria(this, node);
                Add(criteria);
                criteria.OnAnyRelationValueChanged += InnerCriteriaValueChanged;
                node.SetRelations(criteria);
            }
            RelationsCriteriasDictionary = this.ToDictionary(c => c.NodeMain);
        }


        private void InnerCriteriaValueChanged(IRelationsCriteria relation, IRelationNode relationNode)
        {
            RecountGroupCoefficients();
            Logger.Default.AddInfo(relationNode, $"Обновлено отношение - {HierarchyContext.MainGoal.Name}", $"{relationNode.From.Name} - {relationNode.To.Name} ({relationNode.ValueRounded}), пересчитываем коэффициенты иерархии", LogCategory.Relations);
            OnInnerRelationValueChanged?.Invoke(this, relation, relationNode);
        }
        private void RecountGroupCoefficients()
        {
            HierarchyContext.MainGoal.Coefficient = 1;
            foreach (var groupNodes in HierarchyContext.NodesGroupedByGroup())
            {
                string group = groupNodes.Key;
                if (group != Node.MainGroupName)
                {
                    foreach (var node in groupNodes)
                    {
                        var coefficients = VectorMtx.CreateCoeffs(this, node);
                        double coeff = coefficients[node];

                        node.Coefficient = coeff;
                    }
                }
            }
        }



        //Получение отношений
        public IRelationsCriteria this[INode main]
        {
            get => RelationsCriteriasDictionary[main];
        }
        public IEnumerable<IGrouping<INode, IRelationNode>> RelationsGroupedMain(INode node)
        {
            return RelationsCriteriasDictionary[node].MtxView;
        }


        //Состояние пригодности
        public bool Correct => RelationsCriteria.All(c => c.Correct);
        public bool Consistent => RelationsCriteria.All(c => c.Consistent);
        public bool Known => RelationsCriteria.All(c => c.Known);


        //Установка значений
        public void SetValue(INode main, INode from, INode to, double value)
        {
            RelationsCriteriasDictionary[main].SetValue(from, to, value);
        }
        public void SetAllUnknown()
        {
            foreach (var criteria in RelationsCriteria)
            {
                criteria.SetUnknown();
            }
        }
        public void SetValuesFromTemplate(ITemplateProject template)
        {
            SetAllUnknown();
            foreach (var rel in template.Relations)
            {
                INode main = HierarchyContext.Nodes.FirstOrDefault(n => n.Name == rel.Main);
                INode from = HierarchyContext.Nodes.FirstOrDefault(n => n.Name == rel.From);
                INode to = HierarchyContext.Nodes.FirstOrDefault(n => n.Name == rel.To);
                if (main != null && from != null && to != null)
                {
                    SetValue(main, from, to, rel.Value);
                }
            }
        }
    }
}
