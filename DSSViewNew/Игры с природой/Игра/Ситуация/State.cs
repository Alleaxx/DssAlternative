using DSSLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    public class State<T>
    {
        public override string ToString()
        {
            return Name;
        }

        public readonly T Type;
        public readonly Dictionary<T, Note> Comparison = new Dictionary<T, Note>();

        public string Name { get; protected set; }
        public State(T type)
        {
            Type = type;
        }
        protected void AddCompare(T type, double profit, string message)
        {
            Comparison.Add(type, new Note(message, profit));
        }


        public Note CompareWith(State<T> type)
        {
            return CompareWith(type.Type);
        }
        public Note CompareWith(T type)
        {
            return Comparison.ContainsKey(type) ? Comparison[type] : new Note("Возражений нет", 1);
        }


        public override bool Equals(object obj)
        {
            if(obj is State<T> stat)
            {
                return stat.Type.Equals(Type);
            }
            else
            {
                return false;
            }
        }
        public override int GetHashCode()
        {
            return Type.GetHashCode();
        }
    }
}
