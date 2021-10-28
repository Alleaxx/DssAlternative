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

namespace DSSAHP
{
    public partial class AHPAdvicorWindow : Window
    {
        public AHPAdvicorWindow()
        {
            InitializeComponent();
            View.AHP = DataContext as ViewAHP;
        }

        private void relationsBtn_Click(object sender, RoutedEventArgs e)
        {
            Tab.SelectedIndex = 2;
        }

        private void hierarchyBtn_Click(object sender, RoutedEventArgs e)
        {
            Tab.SelectedIndex = 1;
        }
    }
}
