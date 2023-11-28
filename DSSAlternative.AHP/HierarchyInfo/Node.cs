using DSSAlternative.AHP.Relations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DSSAlternative.AHP.HierarchyInfo
{
    /// <summary>
    /// Элемент иерархии задачи принятия решений. Альтернатива / Критерий / Цель
    /// </summary>
    public interface INode : ICloneable
    {
        /// <summary>
        /// Возникает при изменении полей узла
        /// </summary>
        public event Action<INode> OnNodeFieldsChanged;

        /// <summary>
        /// ID узла
        /// </summary>
        int ID { get; }

        /// <summary>
        /// Название узла в иерархии
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Краткое описание узла
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Путь к изображению. Актуален для главного узла 
        /// </summary>
        string ImgPath { get; set; }


        /// <summary>
        /// Уровень узла в иерархии. В основном визуальный параметр
        /// </summary>
        int Level { get; set; }

        /// <summary>
        /// Собственная группа узла в иерархии
        /// </summary>
        string Group { get; set; }

        /// <summary>
        /// Группа, которой подчиняется данный узел
        /// </summary>
        string GroupOwner { get; set; }

        /// <summary>
        /// Итоговый весовой коэффициент для узла в его группе
        /// </summary>
        double Coefficient { get; set; }


        /// <summary>
        /// Сформировать хэш по полям узла для сравнения
        /// </summary>
        /// <returns>Уникальная строка</returns>
        string GetHashFromFields(bool includeName = true);

        /// <summary>
        /// Иерархия, содержащая данный узел
        /// </summary>
        IHierarchy Hierarchy { get; }

        /// <summary>
        /// Установить иерархию, в которой уже находится данный узел 
        /// </summary>
        void SetHierarchy(IHierarchy hier);

        /// <summary>
        /// Отношения, в которых участвует данный узел
        /// </summary>
        IRelationsCriteria Relations { get; }

        /// <summary>
        /// Установить отношения, в которых участвует данный узел
        /// </summary>
        void SetRelations(IRelationsCriteria relations);

        /// <summary>
        /// Скопировать незначимые поля с другого узла
        /// </summary>
        void CopyMinorFieldsFrom(INode node);
    }

    /// <summary>
    /// Элемент иерархии задачи принятия решений. Альтернатива / Критерий / Цель
    /// </summary>
    public class Node : INode
    {
        public override string ToString()
        {
            return $"{Name} [{Group}]";
        }


        //Константы для групп
        public const string MainGroupOwnerName = "Нет";
        public const string MainGroupName = "Цель";


        #region События

        public event Action<INode> OnNodeFieldsChanged;

        #endregion

        #region Свойства

        public int ID { get; private set; }

        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnNodeFieldsChanged?.Invoke(this);
            }
        }
        private string name;
        public string Description
        {
            get => description;
            set
            {
                description = value;
                OnNodeFieldsChanged?.Invoke(this);
            }
        }
        private string description;
        public string ImgPath
        {
            get => imgPath;
            set
            {
                imgPath = value;
                OnNodeFieldsChanged?.Invoke(this);
            }
        }
        private string imgPath;


        //Информация о структуре
        public int Level
        {
            get => level;
            set
            {
                if (IsHierarchySealed())
                {
                    return;
                }

                level = value > 0 ? value : 0;
                OnNodeFieldsChanged?.Invoke(this);
            }
        }                
        private int level;
        public string Group
        {
            get => group;
            set
            {
                if (IsHierarchySealed())
                {
                    return;
                }

                group = value;
                OnNodeFieldsChanged?.Invoke(this);
            }
        }                
        private string group;
        public string GroupOwner
        {
            get => groupOwner;
            set
            {
                if (IsHierarchySealed())
                {
                    return;
                }

                groupOwner = value;
                OnNodeFieldsChanged?.Invoke(this);
            }
        }      
        private string groupOwner;

        public double Coefficient { get; set; }


        public IHierarchy Hierarchy { get; private set; }
        public void SetHierarchy(IHierarchy hierarchy)
        {
            Hierarchy = hierarchy;
        }
        private bool IsHierarchySealed()
        {
            return Hierarchy?.IsSealed ?? false;
        }
        public IRelationsCriteria Relations { get; private set; }
        public void SetRelations(IRelationsCriteria relations)
        {
            Relations = relations;
        }

        #endregion

        #region Конструкторы
        private static int IDTotal = 0;

        public Node(int level, string name, string group, string groupOwner)
        {
            ID = IDTotal++;
            Name = name;
            Level = level;
            Group = group;
            GroupOwner = groupOwner;
        }
        
        #endregion


        //Реализация клонирования
        public object Clone()
        {
            return CloneThis();
        }
        public Node CloneThis()
        {
            return MemberwiseClone() as Node;
        }

        public void CopyMinorFieldsFrom(INode node)
        {
            this.name = node.Name;
            this.description = node.Description;
            this.imgPath= node.ImgPath;
        }

        public string GetHashFromFields(bool includeStructuralFields = true)
        {
            string nameText = includeStructuralFields ? Name : "";
            string imgText = includeStructuralFields ? ImgPath : "";
            string descrText = includeStructuralFields ? Description : "";
            return $"{Level}{Group}{GroupOwner}_{nameText}{imgText}{descrText}";
        }
    }
}
