using System;
using Windows.UI.Xaml.Data;

namespace UniversalAppSamplePDFViewerControlForWindows10.Controls.Converters
{
    public class PageIndexToPageNumberConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int pageIndex = (int) value;

            if (pageIndex == -1)
            {
                return "0";
            }

            return string.Format("{0}", pageIndex+1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new System.NotImplementedException();
        }
    }
}