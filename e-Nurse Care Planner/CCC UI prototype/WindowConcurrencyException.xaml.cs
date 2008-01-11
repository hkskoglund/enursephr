using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;
namespace CCC.UI
{
    /// <summary>
    /// Interaction logic for WindowConcurrencyException.xaml
    /// </summary>
    public partial class WindowConcurrencyException : Window
    {
        public WindowConcurrencyException()
        {
            InitializeComponent();
        }

        public void ShowException(OptimisticConcurrencyException ex)
        {
            tbStatus.Text = ex.Message;
        }
    }
}
