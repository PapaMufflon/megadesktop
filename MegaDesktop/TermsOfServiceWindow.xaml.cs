using System;
using System.Windows;

namespace MegaDesktop
{
    /// <summary>
    ///     Interaction logic for TermsOfServiceWindow.xaml
    /// </summary>
    public partial class TermsOfServiceWindow : Window
    {
        public TermsOfServiceWindow()
        {
            InitializeComponent();
        }

        private void Grid_Loaded_1(object sender, RoutedEventArgs e)
        {
            TosBrowser.Navigate(new Uri("http://g.static.mega.co.nz/pages/terms.html"));
            AcceptTos.Focus();
        }

        private void AcceptTos_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void DeclineTos_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}