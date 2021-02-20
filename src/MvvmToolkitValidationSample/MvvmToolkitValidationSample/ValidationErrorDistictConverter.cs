using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace MvvmToolkitValidationSample
{
    public class ValidationErrorDistictConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<ValidationError> errors)
            {
                return errors.Distinct(new ValidationErrorComparer());
            }
            else
            {
                return Array.Empty<ValidationError>();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    class ValidationErrorComparer : IEqualityComparer<ValidationError>
    {
        public bool Equals([AllowNull] ValidationError x, [AllowNull] ValidationError y)
        {
            return x?.ErrorContent == y?.ErrorContent;
        }

        public int GetHashCode([DisallowNull] ValidationError obj)
        {
            return obj.ErrorContent.GetHashCode();
        }
    }
}
