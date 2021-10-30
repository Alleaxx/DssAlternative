using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public interface IUsable
    {
        bool Correct { get; }
        bool Consistent { get; }
        bool Known { get; }
    }
}
