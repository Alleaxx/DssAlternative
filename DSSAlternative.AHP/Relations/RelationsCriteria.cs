using DSSAlternative.AHP.HierarchyInfo;
using DSSAlternative.AHP.MatrixMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSAlternative.AHP.Relations
{

    /// <summary>
    /// Список отношений для конкретного узла (критерия)
    /// </summary>
    public interface IRelationsCriteria : IUsable
    {
        /// <summary>
        /// Возникает при изменении значения какого-либо отношения из списка по этому критерию
        /// </summary>
        event Action<IRelationsCriteria, IRelationNode> OnAnyRelationValueChanged;


        /// <summary>
        /// Контекст отношений, к которому принадлежит этот критерий
        /// </summary>
        IRelationsHierarchy RelationContext { get; }

        /// <summary>
        /// Главный узел (критерий), по которому происходит сравнение в списке отношений
        /// </summary>
        INode NodeMain { get; }

        /// <summary>
        /// Список узлов, которые сравниваются друг с другом по данному критерию
        /// </summary>
        IEnumerable<INode> NodesControlled { get; }

        /// <summary>
        /// Список всех объектов-сравнений узлов по этому критерию.
        /// Включает зеркальные и отношения к самому себе
        /// </summary>
        IEnumerable<IRelationNode> NodeCompares { get; }

        /// <summary>
        /// Минимально возможный список объектов-сравнений с помощью которых можно целиком заполнить отношение.
        /// Не включает зеркальные и отношения к самим себе
        /// </summary>
        IEnumerable<IRelationNode> NodeComparesMini { get; }

        /// <summary>
        /// Установить значение по указанному отношению
        /// </summary>
        void SetValue(INode from, INode to, double value);
        
        /// <summary>
        /// Сбросить отношения по критерию
        /// </summary>
        void SetUnknown();

        /// <summary>
        /// Объект для удобного представления матрицы: сравнения сгрупированные по первичному узлу
        /// </summary>
        IEnumerable<IGrouping<INode, IRelationNode>> MtxView { get; }
        
        /// <summary>
        /// Матрица по критерию. При обращении считается заново
        /// </summary>
        IMatrix Mtx { get; }
    }

    /// <summary>
    /// Список отношений для конкретного узла
    /// </summary>
    public class RelationsCriteria : List<IRelationNode>, IRelationsCriteria
    {
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"- Критерий {NodeMain}: {NodeMain.Coefficient}");
            sb.Append($"{Mtx.GetText()}");
            sb.Append($"\n--- Корректность: {Correct}, согласованность {Consistent}, известность {Known}");
            return sb.ToString();
        }

        public event Action<IRelationsCriteria, IRelationNode> OnAnyRelationValueChanged;

        //Содержание отношений по критерию
        public INode NodeMain { get; init; }
        public IRelationsHierarchy RelationContext { get; init; }
        public IEnumerable<INode> NodesControlled { get; init; }

        public IEnumerable<IRelationNode> NodeCompares => this;
        private IRelationNode this[INode from, INode to]
        {
            get => this.FirstOrDefault(r => r.From == from && r.To == to);
        }


        //Требуемые отношения
        public IEnumerable<IRelationNode> NodeComparesMini => PreferTop ? TopMtxCellsNodeCompares : BottomMtxCellsNodeCompares;


        //Доступ к отдельным отношения
        private bool PreferTop = true;
        private IEnumerable<IRelationNode> TopMtxCellsNodeCompares { get; set; }
        private IEnumerable<IRelationNode> BottomMtxCellsNodeCompares { get; set; }
        private IEnumerable<IRelationNode> DiagonalMtxCellsCompares { get; set; }



        //Характеристика критерия
        public bool Correct => Known && Consistent;
        public bool Known => !Mtx.WithZeros();
        public bool Consistent => Mtx.IsCorrect;


        //Матричный вид
        public IEnumerable<IGrouping<INode, IRelationNode>> MtxView { get; private set; }
        public IMatrix Mtx => Matrix.CreateRelations(RelationContext, NodeMain);



        //Создание
        public RelationsCriteria(IRelationsHierarchy hier, INode node)
        {
            RelationContext = hier;
            NodeMain = node;
            NodesControlled = node.NodesControlled().ToArray();

            CreateRelations();
        }
        private void CreateRelations()
        {
            foreach (var fromNode in NodesControlled)
            {
                foreach (var toNode in NodesControlled)
                {
                    IRelationNode relation = new RelationNode(this, NodeMain, fromNode, toNode, 0);
                    Add(relation);
                    relation.OnValueChanged += RelationValueChanged;
                }
            }
            foreach (var relation in NodeCompares)
            {
                relation.Mirrored = this[relation.To, relation.From];
            }

            List<IRelationNode> top = new List<IRelationNode>();
            List<IRelationNode> bottom = new List<IRelationNode>();

            int count = NodesControlled.Count();
            for (int i = 0; i < count; i++)
            {
                for (int a = i + 1; a < count; a++)
                {
                    top.Add(this[i * count + a]);
                }
            }

            for (int i = count - 1; i >= 0; i--)
            {
                for (int a = i - 1; a >= count; a--)
                {
                    top.Add(this[i * count - a]);
                }
            }

            TopMtxCellsNodeCompares = top;
            BottomMtxCellsNodeCompares = bottom;
            DiagonalMtxCellsCompares = this.Where(r => r.Self).ToArray();

            MtxView = this.GroupBy(r => r.From);
        }
        private void RelationValueChanged(IRelationNode relation)
        {
            OnAnyRelationValueChanged?.Invoke(this, relation);
        }


        //Установка значений
        public void SetValue(INode from, INode to, double value)
        {
            this[from, to].Value = value;
        }
        public void SetUnknown()
        {
            foreach (var relation in TopMtxCellsNodeCompares)
            {
                relation.SetUnknown();
            }
        }

    }
}
