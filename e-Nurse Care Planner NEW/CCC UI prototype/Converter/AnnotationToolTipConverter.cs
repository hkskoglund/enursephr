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

namespace eNurseCP.userInterfaceLayer
{
    [ValueConversion(typeof(object), typeof(string))]
    public class AnnotationToolTipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            Annotation annotation = (Annotation)value;
            return "Created: " + annotation.CreationTime + " by " + annotation.Authors[0];

        }

        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            // we don't intend this to ever be called
            return null;
        }
    }
}

