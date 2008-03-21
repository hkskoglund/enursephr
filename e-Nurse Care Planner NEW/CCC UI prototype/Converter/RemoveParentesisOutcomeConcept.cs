using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data; // IValueConverter
using System.Globalization; // CultureInfo

namespace CCC.UI
{
    public class RemoveParentesisOutcomeConceptConverter : IValueConverter
    {


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value != null)
            {

                string outcomeConcept = (string)value.ToString();
                int firstParentesisIndex = outcomeConcept.IndexOf("(");
                if (firstParentesisIndex != -1)
                    return outcomeConcept.Substring(0, firstParentesisIndex).ToLowerInvariant();
                else
                    return outcomeConcept.ToLowerInvariant();
               
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


