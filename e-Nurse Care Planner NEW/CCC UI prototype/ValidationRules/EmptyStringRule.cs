using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Globalization;

namespace CCC.UI
{
    
    class EmptyStringRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {


            if (value == null)
                return new ValidationResult(false, "Cannot accept an empty entry here");
            else if (value.ToString().Length == 0)
                return new ValidationResult(false, "Cannot accept an empty entry here");
            else
                return new ValidationResult(true,null);

        }
    }

}
