using System;
using Windows.UI.Xaml.Data;

namespace UniversalAppSamplePDFViewerControlForWindows10.Controls.Converters
{
    public class DoubleToPercentageConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return string.Format("{0}%", (double)value*100);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new System.NotImplementedException();
        }
    }
}