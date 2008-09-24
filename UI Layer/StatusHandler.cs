using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace eNursePHR.userInterfaceLayer
{
    public class StatusHandler : INotifyPropertyChanged
    {
        private string _statusMsg;
        public string StatusMsg
        {
            get { return _statusMsg; }
            set { if (value != _statusMsg)
            {
                _statusMsg = value;
                NotifyPropertyChanged("StatusMsg");
            }}
        }

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
