using System;
using System.Windows;
using System.Windows.Threading;
using MegaApi;
using MegaDesktop.Services;

namespace MegaDesktop
{
    /// <summary>
    ///     Interaction logic for WindowLogin.xaml
    /// </summary>
    internal partial class WindowLogin
    {
        private readonly IUserManagement _userManagement;

        public WindowLogin(IUserManagement userManagement)
        {
            _userManagement = userManagement;

            InitializeComponent();
            textBoxPass_LostFocus(null, null);
            textBoxEmail_LostFocus(null, null);
        }

        private void textBoxPass_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxPass.Password))
            {
                textBoxPass.Password = "password";
            }
        }

        private void textBoxEmail_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxEmail.Text))
            {
                textBoxEmail.Text = "Email";
            }
        }

        private void textBoxEmail_GotFocus(object sender, RoutedEventArgs e)
        {
            if (textBoxEmail.Text == "Email")
            {
                textBoxEmail.Text = "";
            }
        }

        private void textBoxPass_GotFocus(object sender, RoutedEventArgs e)
        {
            if (textBoxPass.Password == "password")
            {
                textBoxPass.Password = "";
            }
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxPass.Password) ||
                string.IsNullOrEmpty(textBoxEmail.Text))
            {
                return;
            }

            textBoxStatus.Text = "Checking...";

            _userManagement.LoginUser(new MegaUser(textBoxEmail.Text, textBoxPass.Password))
                           .ContinueWith(x =>
                               {
                                   if (x.Exception == null)
                                       Close();
                                   else
                                       Invoke(() => textBoxStatus.Text = "Incorrect login or password");
                               });
        }

        private void Invoke(Action fn)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, fn);
        }
    }
}