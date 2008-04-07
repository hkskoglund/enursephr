using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data; // IValueConverter
using System.Globalization; // CultureInfo

namespace CCC.UI
{
    public class TitleConverter : IValueConverter
    {


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {



            if (value != null)
            {
                string title = (string)value.ToString();
                if (title.Length > 60)
                    title = title.Substring(0, 60) + "...";

                return title;

            }

            else

                return string.Empty;

        }



        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            throw new Exception("The method or operation is not implemented.");

        }

    }



}


