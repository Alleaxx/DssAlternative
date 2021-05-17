using System;
using System.Collections.Generic;
using System.Text;

namespace DSSLib
{
    /// <summary>
    /// Объект, способный выводить своё текущее состояние в консоль
    /// </summary>
    interface IOutput
    {
        /// <summary>
        /// Выввести объект в консоль
        /// </summary>
        void Output();



        //void OutputFull();
    }
}
