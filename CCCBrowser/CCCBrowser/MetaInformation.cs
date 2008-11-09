﻿using System;
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
using System.Globalization;

namespace CCCBrowser
{
    public class MetaInformation 
    {
        private string _authors;
        public string Authors
        {
            get
            {
                if (Language == "en-US")
                    return "Developed by " + _authors;
                else
                    return "Translated by " + _authors;
            }
            set
            {
                if (_authors != value)
                    _authors = value;
            }
        }
        public DateTime? LastUpdate { get; set; }
        
        public string LogoURL {get; set; }
        public string Version { get; set; }
        public string Language { get; set; }
        public string Name { get; set; }
        
        private string _lastUpdateString;
        public string LastUpdateString
        {

            get
            {
               
                if (LastUpdate == null)
                    return null;
                else
                    return LastUpdate.Value.ToString("D",new CultureInfo(Language));
            }
            set
            {
                _lastUpdateString = value;
                if (value == String.Empty)
                    LastUpdate = null;
                else
                    LastUpdate = DateTime.Parse(value);
            }
        }

        
    }
}
