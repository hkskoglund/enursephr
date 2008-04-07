using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Annotations;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Documents;

namespace CCC.UI
{
    [ValueConversion(typeof(object), typeof(string))]
    public class HighlightTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {

            IAnchorInfo info = AnnotationHelper.GetAnchorInfo(AnnotationService.GetService((FlowDocumentPageViewer)(App.Current.MainWindow).FindName("Viewer")), (Annotation) value);

            TextAnchor tAnchor = info.ResolvedAnchor as TextAnchor;

            TextRange range = new TextRange((TextPointer)tAnchor.BoundingStart, (TextPointer)tAnchor.BoundingEnd);

            return range.Text;




            
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            // we don't intend this to ever be called
            return null;
        }
    }
}
