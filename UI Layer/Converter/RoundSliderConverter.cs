using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Data;

namespace eNursePHR.userInterfaceLayer
{
    [ValueConversion(typeof(double), typeof(double))]

    public class RoundSliderConverter : IValueConverter
    {

        public object Convert(object value, Type targetType,

        object parameter,

        System.Globalization.CultureInfo culture)
        {


            return Math.Round((double)value, 0);

        }


        public object ConvertBack(object value,

        Type targetType,

        object parameter,

        System.Globalization.CultureInfo culture)
        {

            throw new Exception("The method or operation is not implemented.");

        }

    }


}
