using System;
using System.Globalization;
using System.Windows;

namespace Fasetto.Word
{
    /// <summary>
    /// The converter that takes in da boolean if a message was sent by me, and aligns to the right
    /// If not sent by me, alligns to the left
    /// </summary>
    public class SentByMeToAligmentConverter : BaseValueConverter<SentByMeToAligmentConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
