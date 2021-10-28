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
    public partial class MatrixViewWindow : Window
    {
        public ViewMatrix ViewMatrix { get; set; }
        public MatrixViewWindow()
        {
            InitializeComponent();
            ViewMatrix = DataContext as ViewMatrix;
        }

        private void about_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow window = new AboutWindow();
            window.ShowDialog();
        }
    }
}
