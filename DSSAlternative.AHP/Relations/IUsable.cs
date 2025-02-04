using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSAlternative.AHP.Relations
{
    /// <summary>
    /// Интерфейс для применимости (отношений)
    /// </summary>
    public interface IUsable
    {
        /// <summary>
        /// Отношения известны и согласованны
        /// </summary>
        bool Correct { get; }

        /// <summary>
        /// Отношения согласованны
        /// </summary>
        bool Consistent { get; }

        /// <summary>
        /// Отношения известны
        /// </summary>
        bool Known { get; }
    }
}
