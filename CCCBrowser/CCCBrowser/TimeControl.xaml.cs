using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CCCBrowser
{
    public partial class TimeControl : UserControl
    {

        public readonly DependencyProperty HoursProperty =
          DependencyProperty.Register("Hours", typeof(int),
          typeof(TimeControl),null);

        
        public int Hours
        {
            get { return (int)GetValue(HoursProperty); }
            set
            {
                SetValue(HoursProperty, value);
            }
        }


        public static readonly DependencyProperty MinutesProperty =
          DependencyProperty.Register("Minutes", typeof(int),
          typeof(TimeControl),null);
         

        public int Minutes
        {
            get { return (int)GetValue(MinutesProperty); }
            set {
                
                SetValue(MinutesProperty, value);
            }
        }

        
        public TimeControl()
        {
            InitializeComponent();
          
        }

       

        private void nuHours_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Hours = Convert.ToInt16(e.NewValue);  
        }

        private void nuMinutes_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Minutes = Convert.ToInt16(e.NewValue);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            nuHours.Value = (this.DataContext as QuickPlanIntervention).Task.Hours;
            nuMinutes.Value = (this.DataContext as QuickPlanIntervention).Task.Minutes;  
          
            
        }


    }
}
