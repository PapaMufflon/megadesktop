using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MegaDesktop.Ui
{
    public class DataGridHelper : DependencyObject
    {
        public static readonly DependencyProperty TextColumnStyleProperty =
            DependencyProperty.RegisterAttached("TextColumnStyle", typeof(Style), typeof(DataGridHelper), new PropertyMetadata(default(Style), OnStyleChanged));

        private static void OnStyleChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var grid = (DataGrid)dependencyObject;
            if (dependencyPropertyChangedEventArgs.OldValue == null && dependencyPropertyChangedEventArgs.NewValue != null)
                grid.Columns.CollectionChanged += (s, e) => UpdateColumnStyles(grid);
        }

        public static void SetTextColumnStyle(DataGrid element, Style value)
        {
            element.SetValue(TextColumnStyleProperty, value);
        }

        public static Style GetTextColumnStyle(DataGrid element)
        {
            return (Style)element.GetValue(TextColumnStyleProperty);
        }

        private static void UpdateColumnStyles(DataGrid grid)
        {
            var style = GetTextColumnStyle(grid);

            foreach (var column in grid.Columns.OfType<DataGridTextColumn>())
                foreach (var setter in style.Setters.OfType<Setter>())
                    if (setter.Value is BindingBase)
                        BindingOperations.SetBinding(column, setter.Property, setter.Value as BindingBase);
                    else
                        column.SetValue(setter.Property, setter.Value);
        }
    }
}