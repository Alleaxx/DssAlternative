using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public static class NodeExtensions
    {
        /// <summary>
        /// Получить узлы группы, которая контролирует текущий элемент
        /// </summary>
        public static IEnumerable<INode> NodeControllers(this INode node)
        {
            return node.Hierarchy.Nodes.Where(n => n.Group == node.GroupOwner).ToArray();
        }
        
        /// <summary>
        /// Получить узлы группы, которая подконтрольна текущему элементу
        /// </summary>
        public static IEnumerable<INode> NodesControlled(this INode node)
        {
            return node.Hierarchy.Nodes.Where(n => n.GroupOwner == node.Group).ToArray();
        }



        /// <summary>
        /// Получить узлы уровня + 1 от текущего элемента
        /// </summary>
        public static IEnumerable<INode> NodesUpperLevel(this INode node)
        {
            return node.Hierarchy.Nodes.Where(n => n.Level == node.Level - 1).ToArray();
        }

        /// <summary>
        /// Получить узлы уровня - 1 от текущего элемента
        /// </summary>
        public static IEnumerable<INode> NodesLowerLevel(this INode node)
        {
            return node.Hierarchy.Nodes.Where(n => n.Level == node.Level + 1).ToArray();
        }

        /// <summary>
        /// Получить узлы того же уровня от текущего элемента
        /// </summary>
        public static IEnumerable<INode> NodesSameLevel(this INode node)
        {
            return node.Hierarchy.Nodes.Where(n => n.Level == node.Level);
        }
        
        /// <summary>
        /// Получить узлы с группой текущего элемента
        /// </summary>
        public static IEnumerable<INode> NodesSameGroup(this INode node)
        {
            return node.Hierarchy.Nodes.Where(n => n.Group == node.Group);
        }

        /// <summary>
        /// Получить узлы, контролируемые той же группой, что контролирует этот элемент
        /// </summary>
        public static IEnumerable<INode> NodesSameGroupOwner(this INode node)
        {
            return node.Hierarchy.Nodes.Where(n => n.GroupOwner == node.GroupOwner);
        }

        //Контролируемые узлы
        public static string LevelName(this INode node)
        {
            int maxLevel = node.Hierarchy.Nodes.Select(n => n.Level).Max();
            if (node.Level == 0)
            {
                return "Цель";
            }
            else if (node.Level == maxLevel)
            {
                return "Альтернативы";
            }
            else if (node.Level == 1)
            {
                return "Критерии";
            }
            else if (node.Level < 0)
            {
                return "???";
            }
            return "Подкритерии";
        }


        //Порядковые номера в группе и на уровне
        public static int OrderIndexLevel(this INode node)
        {
            return node.NodesSameLevel().ToList().IndexOf(node);
        }
        public static int OrderIndexGroup(this INode node)
        {
            return node.NodesSameGroupOwner().ToList().IndexOf(node);
        }
    }
}
