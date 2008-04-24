using System;
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
    [ValueConversion(typeof(object), typeof(string))]
    public class ValidContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            Annotation annotation = value as Annotation;
            if (annotation.Anchors[0].Contents.Count == 0)
                return String.Empty;
           
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(annotation.Anchors[0].Contents[0].OuterXml);

            XmlNode xe = xdoc.SelectSingleNode("eNursePHR/Statement[1]");
            if (xe == null)
                return false;

            string contentInXml = xe.Attributes["Content"].Value;

            if (((WindowMain)App.Current.MainWindow).getText(annotation) == contentInXml)
                return true;
            else
                return false;
            
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            // we don't intend this to ever be called
            return null;
        }
    }
}
