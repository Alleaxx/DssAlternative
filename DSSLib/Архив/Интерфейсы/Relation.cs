using System;
using System.Collections.Generic;
using System.Text;

namespace DSSLib
{
    public class KeyValueRelation<K,V,R>
    {
        public K Key { get; set; }
        public V Value { get; set; }
        public R Result { get; set; }
        public KeyValueRelation(K key, V val, R res)
        {
            Key = key;
            Value = val;
            Result = res;
        }
    }

    public class AltCaseRelation : KeyValueRelation<Alternative, Case, double>
    {
        public AltCaseRelation(Alternative a, Case c, double d) : base(a,c,d)
        {

        }
    }


}
