using System;
using System.Net;
using System.ComponentModel;

namespace CCCBrowser.Support
{
    public class ActionType : FrameworkElement, INotifyPropertyChanged
    {

        public ActionType()
        {
        }

        public ActionType(int code, string concept, string language, string version)
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

        public int Code { get; set; }


    }
}
