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
using System.Windows.Data;
using CCCBrowser.Support;

namespace CCCBrowser
{
    public class AlignmentConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double leftAdjust = 5.0;
            if (value.GetType() == typeof(Intervention))
            {
                Intervention thisIntervention = value as Intervention;
                if (thisIntervention.MinorCode != 0)
                    return new Thickness(leftAdjust, 0, 0, 0);
            }

            if (value.GetType() == typeof(Diagnosis))
            {
                Diagnosis thisDiagnosis = value as Diagnosis;
                if (thisDiagnosis.MinorCode != 0)
                    return new Thickness(leftAdjust, 0, 0, 0);
            }

            return new Thickness();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
