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
using CCC.BusinessLayer;

namespace CCC.UI
{
    /// <summary>
    /// Interaction logic for WindowChangeDiagnosis.xaml
    /// </summary>
    public partial class WindowChangeDiagnosis : Window
    {
        public WindowChangeDiagnosis()
        {
            InitializeComponent();
        }

        private void cbOutCome_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            myNursingDiagnosis m = (myNursingDiagnosis) this.DataContext;
            
            if (m == null)
                return;

            ComboBox c = sender as ComboBox;

            switch (c.SelectedIndex)
            {
                case 0: m.Outcome = 1; break;
                case 1: m.Outcome = 2; break;
                case 2: m.Outcome = 3; break;
            }
 }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
          //  cbOutCome.DataContext = WindowMain.cccFrameWork.Outcomes;
        }
    }
}
