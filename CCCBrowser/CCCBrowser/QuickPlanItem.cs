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
using System.Globalization;
using System.ComponentModel;

namespace CCCBrowser
{
    public class QuickPlanItem : INotifyPropertyChanged
    {

        public QuickPlanItem(DateTime date) {
            Date = date;
        }


        public DateTime Date { get; set; }


        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }


        private string _description;
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    NotifyPropertyChanged("Description");
                }
            }
        }


        // Enable databinding
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
 
        
    }

    public class QuickPlanDiagnosis : QuickPlanItem
    {
        

        //public QuickPlanDiagnosis(DateTime date) : base(date)
        //{
        //}


        public QuickPlanDiagnosis(DateTime date, Diagnosis tag) : base(date) 
        {
            Tag = tag;
        }

        //public QuickPlanDiagnosis(DateTime date, Diagnosis tag, OutcomeType outcome)
        //    : base(date)
        //{
        //    Tag = tag;
        //    Outcome = outcome;
        //}


        public Diagnosis Tag { get; set; }

    }

    public class QuickPlanIntervention : QuickPlanItem


    {

        public QuickPlanIntervention(DateTime date)
            : base(date)
        {
            Task = new Task(this);
           
        }

        public QuickPlanIntervention(DateTime date, Intervention tag) : base(date)
            
        {
            Tag = tag;
            Task = new Task(this);
        }

        //public QuickPlanIntervention(DateTime date, Intervention tag, ActionType actionType)
        //    : base(date)
        //{
        //    Tag = tag;
        //    ActionType = actionType;
        //}

        public Intervention Tag { get; set; }

        public Task Task { get; set; }

    }
}
