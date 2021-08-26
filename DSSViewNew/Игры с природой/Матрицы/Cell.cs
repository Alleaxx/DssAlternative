using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    public class MtxCell<R, C, V>
    {
        public override string ToString() => $"{Coords} {Value}: {From} - {To}";

        public event Action<MtxCell<R, C, V>> OnValueUpdate;

        public Coords Coords { get; set; }
        public R From { get; set; }
        public C To { get; set; }
        public V Value
        {
            get => value;
            set
            {
                this.value = value;
                OnValueUpdate?.Invoke(this);
            }
        }
        private V value;
    }
}
