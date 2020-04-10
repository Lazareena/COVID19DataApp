using System;
using Xamarin.Forms;
using System.Globalization;

///<sourceurl>https://github.com/jamesmontemagno/xamarin.forms-toolkit/blob/master/FormsToolkit/FormsToolkit/Converters/UpperTextConverter.cs</sourceurl>
///<sourcepermalink>https://github.com/jamesmontemagno/xamarin.forms-toolkit/blob/2cd9da5c9682c2a711f69cad19b48434697fb493/FormsToolkit/FormsToolkit/Converters/UpperTextConverter.cs</sourcepermalink>
///<contentsha>6301604b93ddd8dc8057684f51a51bd7a068ad65</contentsha>
///<licensetype>MIT License</licensetype>
///<licensepermalink>https://github.com/jamesmontemagno/xamarin.forms-toolkit/blob/2cd9da5c9682c2a711f69cad19b48434697fb493/LICENSE.md</licensepermalink>
namespace COVID19_DataApp.Converters
{
    public class UpperTextConverter : IValueConverter
    {
        /// <summary>
        /// Init this instance.
        /// </summary>
        public static void Init()
        {
            var time = DateTime.UtcNow;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            var text = ((string)value);

            return text.ToUpperInvariant();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

