using System;
using System.Net;

using System.ComponentModel;

namespace CCCBrowser.Support
{
    public class Diagnosis : FrameworkElement, INotifyPropertyChanged
    {

       public OutcomeType Outcome { get; set; }
 
        public string ComponentCode { get; set; }
     
        private string _componentName;
        public string ComponentName
        {
            get
            {
               // _componentName = (App.Current.RootVisual as Page).xAPI.getComponentName(ComponentCode, Language, Version);
                return _componentName;
            }
            set
            {
                if (_componentName != value)
                {
                    _componentName = value;
                    NotifyPropertyChanged("ComponentName");
                }
            }
        }

        public int MajorCode { get; set; }
        public int MinorCode { get; set; }
       
        public override string ToString()
        {
            return Concept;


        }

        public override string Definition
        {
            get
            {
                return base.Definition;
            }
            set
            {
                base.Definition = value;
                NotifyPropertyChanged("Definition");
            }
        }

        public override string Concept
        {
            get
            {
                return base.Concept;
            }
            set
            {
                base.Concept = value;
                NotifyPropertyChanged("Concept");
            }
        }

        // Enable databinding
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        
    }

}
