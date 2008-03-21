using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel; // INotifyPropertyChanged

namespace CCC.BusinessLayer
{
    public partial class Item : global::System.Data.Objects.DataClasses.EntityObject
    {
        private bool _containsDiagnosis = false;
        private bool _containsIntervention = false;
        private bool _containsCareComponent = false;
        private bool _containsFolksonomy = false;

        //[global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute(IsNullable = false)]
        public bool ContainsDiagnosis
        {
            get { return this._containsDiagnosis; }
            set
            {
                if (value != this._containsDiagnosis)
                {
                    this._containsDiagnosis = value;
                    
                    //NotifyPropertyChanged("ContainsDiagnosis");
                }
            }
        }

        public bool ContainsIntervention
        {
            get { return this._containsIntervention; }
            set
            {
                if (value != this._containsIntervention)
                {
                    this._containsIntervention = value;
                    //NotifyPropertyChanged("ContainsIntervention");
                }
            }
            
        }

        public bool ContainsCareComponent
        {
            get { return this._containsCareComponent; }
            set
            {
                if (value != this._containsCareComponent)
                {
                    this._containsCareComponent = value;
        //            NotifyPropertyChanged("ContainsCareComponent");
                }
            }
            
        }

        public bool ContainsFolksonomy
        {
            get { return this._containsFolksonomy; }
            set
            {
                if (value != this._containsFolksonomy)
                {
                    this._containsIntervention = value;
          //          NotifyPropertyChanged("ContainsFolksonomy");
                }
            }
            
        }

        //public event PropertyChangedEventHandler PropertyChanged;

        //private void NotifyPropertyChanged(String info)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(info));
        //    }
        //}

    }
}
