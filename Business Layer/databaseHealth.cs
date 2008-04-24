using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data.EntityClient;

namespace eNursePHR.BusinessLayer
{
    public class DatabaseHealth
    {

       
        public bool OK { get; set; }

        private string _message;
        public string Message
        {

            get { return _message; }
            set
            {
                if (this._message != value)
                {
                    this._message = value;
                  //  NotifyPropertyChanged("Message");
                }

            }
        }

        public string Database {get; set; }

       // public string DataSource { get; set; }

        public DatabaseHealth(bool ok, string database, string message)
        {
            this.OK = ok;
            this.Database = database;
            this.Message = message;
        }


//        #region Event handling for INotifyPropertyChanged
//        public event PropertyChangedEventHandler PropertyChanged;

//    private void NotifyPropertyChanged(String info)
//    {
//        if (PropertyChanged != null)
//        {
//            PropertyChanged(this, new PropertyChangedEventArgs(info));
//        }
//    }

//#endregion
    }
}
