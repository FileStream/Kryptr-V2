using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace KryptrGUI
{
    /// <summary>
    /// Interaction logic for TextOrRecover.xaml
    /// </summary>
    public partial class TextOrRecover : Window
    {

        readonly MainWindow ParentWindow;

        public TextOrRecover(MainWindow p)
        {
            InitializeComponent();
            ParentWindow = p;
        }

        private void EncryptClicked(object sender, RoutedEventArgs e)
        {
            ParentWindow.EncryptTextClicked(sender, e);
            Close();
        }

        private void RecoverClicked(object sender, RoutedEventArgs e)
        {
            ParentWindow.RecoverSelected(sender, e);
            Close();
        }
    }
}
