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
    public class TaskMinutes : INotifyPropertyChanged
    {

        public TaskMinutes(double min)
        {
            Minutes = min;
        }
        //private string _componentName;
        //public string ComponentName
        //{
        //    get { return _componentName; }
        //    set
        //    {
        //        if (_componentName != value)
        //            _componentName = value;
        //    }
        //}

        private double _minutes;
        public double Minutes
        {
            get { return _minutes; }
            set
            {
                if (_minutes != value)
                    _minutes = value;
                NotifyPropertyChanged("Minutes");
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string property)
    {
            if (PropertyChanged != null)
                PropertyChanged(this,new PropertyChangedEventArgs(property));
    }
}

        #endregion
    
}
