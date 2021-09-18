using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    public class MtxCell<R, C, V>
    {
        public override string ToString()
        {
            return $"{Coords} {Value}: {From} - {To}";
        }

        public event Action<MtxCell<R, C, V>> OnValueUpdated;

        public R From { get; private set; }
        public C To { get; private set; }

        public Coords Coords { get; set; }
        public V Value
        {
            get => value;
            set
            {
                this.value = value;
                OnValueUpdated?.Invoke(this);
            }
        }
        private V value;


        public MtxCell()
        {

        }
        public MtxCell(Coords coords, R from, C to, V value)
        {
            Coords = coords;
            From = from;
            To = to;
            Value = value;
        }
    }
}
