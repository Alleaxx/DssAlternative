using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public interface ICriteriaRelation : IGrouping<INode, INodeRelation>, IUsable
    {
        event Action<ICriteriaRelation> OnChanged;

        IEnumerable<INode> Nodes { get; }
        IEnumerable<INodeRelation> Required { get; }
        INodeRelation this[INode from, INode to] { get; }
        INodeRelation FirstRequired { get; }
        (bool correct, INodeRelation next) NextRequired(); 

        void Set(INode from, INode to, double value);
        void SetUnknown();

        IEnumerable<IGrouping<INode, INodeRelation>> MtxView { get; }
        IMatrix Mtx { get; }
    }
    
    //Список отношений по узлу
    public class CriteriaRelation : List<INodeRelation>, ICriteriaRelation
    {
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"- Критерий {Node}: {Node.Coefficient}");
            sb.Append($"{Mtx.GetText()}");
            sb.Append($"--- Корректность: {Correct}, согласованность {Consistent}, известность {Known}");
            return sb.ToString();
        }

        public event Action<ICriteriaRelation> OnChanged;

        //Управляющая информация
        public INode Key => Node;
        public readonly INode Node;
        private readonly IRelations Relations;
        public IEnumerable<INode> Nodes { get; init; }




        //Требуемые отношения
        public bool ReqLeft { get; set; } = true;
        public IEnumerable<INodeRelation> Required => ReqLeft ? LeftRelations : RightRelations;
        public INodeRelation FirstRequired => Required.FirstOrDefault();


        //Доступ к отдельным отношения
        private IEnumerable<INodeRelation> LeftRelations { get; set; }
        private IEnumerable<INodeRelation> RightRelations { get; set; }
        private IEnumerable<INodeRelation> DiagRelations { get; set; }
        public INodeRelation this[INode from, INode to]
        {
            get => this.FirstOrDefault(r => r.From == from && r.To == to);
        }
        public (bool correct, INodeRelation next) NextRequired()
        {
            return default;
        }


        //Характеристика критерия
        public bool Correct => Mtx.IsCorrect;
        public bool Known => !Mtx.WithZeros();
        public bool Consistent => Mtx.IsCorrect;


        //Матричный вид
        public IEnumerable<IGrouping<INode, INodeRelation>> MtxView { get; private set; }
        public IMatrix Mtx => Matrix.CreateRelations(Relations, Key);



        //Создание
        public CriteriaRelation(IRelations hier, INode node)
        {
            Relations = hier;
            Node = node;
            Nodes = node.Controlled().ToArray();

            CreateRelations();
        }
        private void CreateRelations()
        {
            foreach (var fromNode in Nodes)
            {
                foreach (var toNode in Nodes)
                {
                    INodeRelation relation = new NodeRelation(Node, fromNode, toNode, 0);
                    Add(relation);
                    relation.OnChanged += RelationValueChanged;
                }
            }
            foreach (var relation in this)
            {
                relation.Mirrored = this[relation.To, relation.From];
            }
            
            LeftRelations = Nodes.SelectMany(n => this.Where(r => r.From == n && !r.Self)).ToArray();
            RightRelations = Nodes.SelectMany(n => this.Where(r => r.To == n && !r.Self)).ToArray();
            DiagRelations = this.Where(r => r.Self).ToArray();

            MtxView = this.GroupBy(r => r.From);
        }
        private void RelationValueChanged(INodeRelation relation)
        {
            OnChanged?.Invoke(this);
        }


        //Установка значений
        public void Set(INode from, INode to, double value)
        {
            this[from, to].Value = value;
        }
        public void SetUnknown()
        {
            foreach (var relation in LeftRelations)
            {
                relation.SetUnknown();
            }
        }

    }

    public class RelationPair
    {
        public readonly NodeRelation FromRelation;
        public readonly NodeRelation ToRelation;

        public INode MainNode => FromRelation.Main;
        public INode FromNode => FromRelation.From;
        public INode ToNode => FromRelation.To;

        public IRating Rating { get; set; }
        public void SetRating(IRating rating)
        {

        }

        public RelationPair(NodeRelation from, NodeRelation to)
        {
            FromRelation = from;
            ToRelation = to;
        }
        public RelationPair(NodeRelation main)
        {
            FromRelation = main;
            ToRelation = main.Mirrored as NodeRelation;
        }
    }
}
