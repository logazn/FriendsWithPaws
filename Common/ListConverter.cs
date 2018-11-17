using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Data;

namespace FriendsWithPaws.Common
{
    class ListConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ObservableCollection<String> items = (ObservableCollection<String>)value;
            StringBuilder Builder = new StringBuilder();
            foreach (var item in items)
            {
                Builder.Append(item);
                Builder.Append("\r\n");

            }
            return Builder.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
