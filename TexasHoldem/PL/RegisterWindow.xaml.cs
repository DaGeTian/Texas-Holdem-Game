﻿using System.Windows;
using CLClient;
using CLClient.Entities;

namespace PL
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private Window loginWindow;

        public RegisterWindow(Window loginWindow)
        {
            InitializeComponent();
            this.loginWindow = loginWindow;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            loginWindow.Show();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            var user = CommClient.Register(username.Text, password.Text, email.Text, "");

            if (user != default(SystemUser)){
                LoginWindow.user = user;
                Close();
                new MainMenuWindow(loginWindow).Show();
            }
            else
            {
                errorMessage.Text ="Could not register at the moment.";
            }
        }
    }
}
