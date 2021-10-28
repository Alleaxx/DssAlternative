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
    /// <summary>
    /// Логика взаимодействия для NewMatrixWindow.xaml
    /// </summary>
    public partial class NewMatrixWindow : Window
    {
        public NewMatrixWindow(NewGameInfo info)
        {
            DataContext = info;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
