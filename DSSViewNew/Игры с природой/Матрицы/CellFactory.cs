using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    public class MtxCellFactory<R, C, V>
    {
        public override string ToString() => $"Фабрика ячеек ({typeof(R).Name}-{typeof(C).Name}-{typeof(V).Name})";

        public virtual MtxCell<R, C, V> NewCell() => new MtxCell<R, C, V>();
        public virtual MtxCell<R, C, V> NewCell(Coords coords, R r, C c, V v) => new MtxCell<R, C, V>();
        public virtual R NewRow => default;
        public virtual C NewCol => default;
        public virtual V NewValue => default;

    }
}
