using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{

    interface ILevelGroup
    {

        public int Level { get; set; }
        public INodeAdv[] Nodes { get; set; }


        public MtxLocalCoeffs MtxLocalCoeffs { get; set; }
        public MtxCoeffs MtxCoeffs { get; set; }
    }
    interface INodeAdv : INode
    {
        IMatrixRelations MtxRelations { get; }

        //Все вышестоящие критерии
        //Учитываемые вышестоящие критерии

        //Соседи из одной группы
        //Все соседи

        //Все узлы, для которых критерий учитывается
        //Все нижестоящие узлы

    }

    public class NodeAdvanced : Node, INodeAdv
    {
        private IProblem Problem { get; set; }

        public IMatrixRelations MtxRelations => new MtxRelations(Problem, this);

        public NodeAdvanced(IProblem hierarchy, INode node)
        {
            Problem = hierarchy;
            Name = node.Name;
            Level = node.Level;
            Contents = node.Contents;
            ContentsControlled = node.ContentsControlled;

        }
    }
}
