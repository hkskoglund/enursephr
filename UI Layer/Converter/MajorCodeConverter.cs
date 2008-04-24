using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;


// Based on information from blog http://www.wiredprairie.us/journal/2007/01/how_to_display_a_formatted_num.html
// Accessed : 11 february 2008

namespace eNursePHR.userInterfaceLayer
{
    [ValueConversion(typeof(object), typeof(string))]
    public class MajorCodeConverter : IValueConverter
    {
   
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {

                string fmt = parameter as string; 

                if (!string.IsNullOrEmpty(fmt)) 
                {
                    return string.Format(culture, fmt, value);
                } else { 
                    return value.ToString(); 
                }


            }



            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {

                throw new Exception("The method or operation is not implemented.");

            }

        }


    
}

