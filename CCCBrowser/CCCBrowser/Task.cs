using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace CCCBrowser
{
    public class Task : INotifyPropertyChanged 
    {
    //    public string Description { get; set; }
    //    public DateTime Start { get; set; }
    //    public DateTime? End { get; set; }
    //    public int RequiredPeople { get; set; }

        public Task(QuickPlanIntervention parent)
        {
            Hours = 1;
            Minutes = 3;
            Parent = parent;
        }

        public QuickPlanIntervention Parent {get; set; } // Reference to parent intervention 1:1

        private int _hours, _minutes;

        public int Hours
        {

            get

            { return _hours; }

            set
            {
                if (CalculateTaskTime != null)
                    CalculateTaskTime(Parent, (-_hours + value)*60);

                
                _hours = value;
                
                    NotifyPropertyChanged("Hours");
            }
        }

        public int Minutes
        {

            get

            { return _minutes; }

            set
            {
                if (CalculateTaskTime != null)
                    CalculateTaskTime(Parent,-_minutes+value);

                    _minutes = value;
                    
                    NotifyPropertyChanged("Minutes");
                
            }
        }

        private double prevTaskTime;

        public delegate void CalculateTaskTimeHandler(QuickPlanIntervention parent, double minutesDelta);
        public event CalculateTaskTimeHandler CalculateTaskTime;


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this,new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
