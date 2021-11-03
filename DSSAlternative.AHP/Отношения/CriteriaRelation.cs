using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    
    //Список отношений по узлу
    public class CriteriaRelation : List<INodeRelation>, ICriteriaRelation
    {
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"- Критерий {Node}: {Node.Coefficient}");
            sb.Append($"{Mtx.GetText()}");
            sb.Append($"\n--- Корректность: {Correct}, согласованность {Consistent}, известность {Known}");
            return sb.ToString();
        }

        public event Action<ICriteriaRelation> OnChanged;

        //Управляющая информация
        public INode Key => Node;
        public readonly INode Node;
        private readonly IRelations Relations;
        public IEnumerable<INode> Nodes { get; init; }




        //Требуемые отношения
        public bool PreferTop { get; set; } = true;
        public IEnumerable<INodeRelation> Required => PreferTop ? TopRelations : BottomRelations;
        public INodeRelation FirstRequired => Required.FirstOrDefault();


        //Доступ к отдельным отношения
        private IEnumerable<INodeRelation> TopRelations { get; set; }
        private IEnumerable<INodeRelation> BottomRelations { get; set; }
        private IEnumerable<INodeRelation> DiagRelations { get; set; }
        public INodeRelation this[INode from, INode to]
        {
            get => this.FirstOrDefault(r => r.From == from && r.To == to);
        }


        //Характеристика критерия
        public bool Correct => Known && Consistent;
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

            List<INodeRelation> top = new List<INodeRelation>();
            List<INodeRelation> bottom = new List<INodeRelation>();

            int count = Nodes.Count();
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

            TopRelations = top;
            BottomRelations = bottom;
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
            foreach (var relation in TopRelations)
            {
                relation.SetUnknown();
            }
        }

    }


}
