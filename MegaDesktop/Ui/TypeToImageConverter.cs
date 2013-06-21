using System;
using System.Globalization;
using System.Windows.Data;
using MegaDesktop.ViewModels;

namespace MegaDesktop.Ui
{
    public class TypeToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is NodeType))
                throw new ArgumentException("The value was no NodeType.");

            switch ((NodeType)value)
            {
                case NodeType.File:
                case NodeType.Folder:
                case NodeType.Dummy:
                    return "pack://application:,,,/MegaDesktop;component/resources/folder.png";
                case NodeType.RootFolder:
                    return "pack://application:,,,/MegaDesktop;component/resources/cloudIcon.png";
                case NodeType.Inbox:
                    return "pack://application:,,,/MegaDesktop;component/resources/messages.png";
                case NodeType.Trash:
                    return "pack://application:,,,/MegaDesktop;component/resources/rubbishBin.png";
                default:
                    throw new ArgumentOutOfRangeException("value");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}