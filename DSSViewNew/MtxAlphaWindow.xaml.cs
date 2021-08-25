using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DSSView
{
    /// <summary>
    /// Логика взаимодействия для MtxAlphaWindow.xaml
    /// </summary>
    public partial class MtxAlphaWindow : Window
    {
        public StatGameView Mtx { get; set; }
        public MtxAlphaWindow()
        {
            InitializeComponent();
            Mtx = DataContext as StatGameView;
        }
    }
}
