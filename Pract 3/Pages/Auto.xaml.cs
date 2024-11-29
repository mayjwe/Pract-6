using Pract_3.Models;
using Pract_3.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pract_3.Pages
{
    /// <summary>
    /// Логика взаимодействия для Autho.xaml
    /// </summary>
    public partial class Auto : Page
    {
        int click;
        public Auto()
        {
            InitializeComponent();
            click = 0;
        }

        private void btnEnterGuests_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Client(null, null));
        }


        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            click += 1;
            string login = tbLogin.Text.Trim();
            string password = tbPassword.Text.Trim();
            string hashPassword = Hash.HashPassword(password);

            var db = Helper.GetContext();

            var user = db.Clients.Where(x => x.Mail == login && x.Password == password).FirstOrDefault();
            if (click == 1)
            {
                if (user != null)
                {
                    MessageBox.Show("Вы вошли под: " + user.Authorization.Role.ToString());
                    LoadPage(user.Authorization.Role.ToString(), user);
                    tbLogin.Text = "";
                    tbPassword.Text = "";
                    tbCaptcha.Text = "";
                    tblCaptcha.Visibility = Visibility.Hidden;
                    tbCaptcha.Visibility = Visibility.Hidden;
                }
                else
                {
                    MessageBox.Show("Вы ввели логин или пароль неверно!");
                    GenerateCapctcha();
                    tbPassword.Text = "";
                    tbCaptcha.Text = "";
                }

            }
            else if (click > 1)
            {
                if (user != null && tbCaptcha.Text == tblCaptcha.Text)
                {
                    MessageBox.Show("Вы вошли под: " + user.Authorization.Role.ToString());
                    LoadPage(user.Authorization.Role.ToString(), user);
                    tbLogin.Text = "";
                    tbPassword.Text = "";
                    tbCaptcha.Text = "";
                    tblCaptcha.Visibility = Visibility.Hidden;
                    tbCaptcha.Visibility = Visibility.Hidden;
                }
                else
                {
                    MessageBox.Show("Введите данные заново!");
                    GenerateCapctcha();
                    tbPassword.Text = "";
                    tbCaptcha.Text = "";
                }
            }



        }

        private void btnEnterGuest_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Client(null, null));
        }
        private void GenerateCapctcha()
        {
            tbCaptcha.Visibility = Visibility.Visible;
            tblCaptcha.Visibility = Visibility.Visible;

            string capctchaText = CaptchaGenerator.GenerateCaptchaText(6);
            tblCaptcha.Text = capctchaText;
            tblCaptcha.TextDecorations = TextDecorations.Strikethrough;
        }
        private void LoadPage(string _role, Clients user)
        {
            click = 0;
            switch (_role)
            {
                case "Клиент":
                    NavigationService.Navigate(new Client(user, _role));
                    break;
                case "Сотрудник":
                    NavigationService.Navigate(new StaffPage(user, _role));
                    break;

            }
        }


    }


}

