using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using DSSLib;

namespace DSSAHP
{
    //Экземпляр приложения
    public class View : NotifyObj
    {
        public static ViewAHP AHP { get; set; }
    }
}
