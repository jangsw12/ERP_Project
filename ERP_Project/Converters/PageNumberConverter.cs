using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ERP_Project.Converters
{
    public class PageNumberConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2)
                return "";

            int pageNumber = 0;
            int totalPages = 0;

            if (values[0] != null && int.TryParse(values[0].ToString(), out int pn))
                pageNumber = pn;

            if (values[1] != null && int.TryParse(values[1].ToString(), out int tp))
                totalPages = tp;

            if (totalPages == 0)
                return "0 / 0";

            if (pageNumber == 0)
                pageNumber = 1;

            return $"{pageNumber} / {totalPages}";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}