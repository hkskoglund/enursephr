
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
    /// <summary>
    /// Converts major diagnosis/interventions font style to bold
    /// </summary>
    public class MajorConverter : IValueConverter
    {


        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.GetType() == typeof(Intervention))
            {
                Intervention thisIntervention = value as Intervention;
                if (thisIntervention.MinorCode == 0)
                    return FontWeights.Bold;
            }

            if (value.GetType() == typeof(Diagnosis)) {
                Diagnosis thisDiagnosis = value as Diagnosis;
                if (thisDiagnosis.MinorCode == 0)
                        return FontWeights.Bold;
            }

            return FontWeights.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
