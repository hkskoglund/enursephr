﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Annotations;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xml;
using System.Xml.XPath;

namespace eNursePHR.userInterfaceLayer
{
    [ValueConversion(typeof(string), typeof(string))]
    public class StatementContentTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml((string)value);
            
            XmlNode xe = xdoc.SelectSingleNode("eNursePHR/Statement[1]");
            if (xe == null)
                return "Failed to retrieve content type of annotated information";

            return xe.Attributes["ContentType"].Value;
        

        }

        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            // we don't intend this to ever be called
            return null;
        }
    }
}

