using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    public class MtxCellFactory<R, C, V>
    {
        public override string ToString()
        {
            return $"Фабрика ячеек ({typeof(R).Name}-{typeof(C).Name}-{typeof(V).Name})";
        }

        public virtual MtxCell<R, C, V> NewCell()
        {
            return new MtxCell<R, C, V>();
        }

        public virtual MtxCell<R, C, V> NewCell(Coords coords, R r, C c, V v)
        {
            return new MtxCell<R, C, V>(coords, r, c, v);
        }

        public virtual R NewRow(int index)
        {
            return default;
        }
        public virtual C NewCol(int index)
        {
            return default;
        }
        public virtual V NewValue => default;

    }
}
