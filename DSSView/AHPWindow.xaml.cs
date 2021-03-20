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
    /// Логика взаимодействия для AHPWindow.xaml
    /// </summary>
    public partial class AHPWindow : Window
    {
        public ViewAHP View => DataContext as ViewAHP;
        public AHPWindow(ViewAHP dataContext)
        {
            DataContext = dataContext;
            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if(View.Selected != null)
                View.Selected.Selected = e.NewValue as IViewElement;
        }
    }
}
