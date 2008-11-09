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
    public class ActionType : FrameworkElement, INotifyPropertyChanged
    {

        public ActionType()
        {
        }

        public ActionType(string code, string concept, string language, string version)
        {
            Code = code;
            Concept = concept;
            Language = language;
            Version = version;
        }
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

        public string Code { get; set; }


    }
}
