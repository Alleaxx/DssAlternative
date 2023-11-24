using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    /// <summary>
    /// Элемент иерархии задачи принятия решений. Альтернатива / Критерий / Цель
    /// </summary>
    public interface INode : ICloneable, IHierNode
    {
        public event Action<INode> OnNodeFieldsChanged;

        /// <summary>
        /// Название узла в иерархии
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// При редактировании также пытается обновить имя сходного узла в связанной иерархии (если такая есть)
        /// </summary>
        string NameActive { get; set; }

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
        /// Сформировать уникальный ключ для узла для сравнения
        /// </summary>
        /// <returns>Уникальная строка</returns>
        string GetNodeKeyID();
    }
    
    public interface IHierNode
    {
        IHierarchy Hierarchy { get; }
        void SetHierarchy(IHierarchy hier);
        void RemoveHierarchy();
    }

    /// <summary>
    /// Элемент иерархии задачи принятия решений. Альтернатива / Критерий / Цель
    /// </summary>
    public class Node : INode
    {
        public override string ToString()
        {
            return $"{Name} [{Level}]";
        }

        //События
        public event Action<INode> OnNodeFieldsChanged;

        //Общая информация
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

        [JsonIgnore]
        public string NameActive
        {
            get => Name;
            set
            {
                string oldName = Name;
                Name = value;
                if(Hierarchy.ConnectedHierarchy != null)
                {
                    var sameNode = Hierarchy.ConnectedHierarchy.Nodes.FirstOrDefault(n => n.Name == oldName && n.Level == Level);
                    if(sameNode != null)
                    {
                        sameNode.Name = value;
                    }
                }
            }
        }
        public string Description { get; set; }
        public string ImgPath { get; set; }


        //Информация о структуре
        public int Level
        {
            get => level;
            set
            {
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
                groupOwner = value;
                OnNodeFieldsChanged?.Invoke(this);
            }
        }      
        private string groupOwner;

        [JsonIgnore]
        public double Coefficient { get; set; }

        #region Конструкторы

        public Node()
        {

        }
        public Node(int level, string name, string group, string groupIndex)
        {
            Name = name;
            Level = level;
            Group = group;
            GroupOwner = groupIndex;
        }
        
        #endregion


        [JsonIgnore]
        public IHierarchy Hierarchy { get; private set; }
        public void SetHierarchy(IHierarchy hierarchy)
        {
            Hierarchy = hierarchy;
        }
        public void RemoveHierarchy()
        {
            Hierarchy = null;
        }


        //Реализация клонирования
        public object Clone()
        {
            return CloneThis();
        }
        public Node CloneThis()
        {
            return MemberwiseClone() as Node;
        }


        //Константы для групп
        public const string MainGroupOwnerName = "Нет";
        public const string MainGroupName = "Цель";

        public string GetNodeKeyID()
        {
            return $"{Name}{Level}{Group}{GroupOwner}";
        }
    }

}
