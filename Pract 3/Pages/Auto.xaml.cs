
using Pract_3.Models;
using Pract_3.Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Pract_3.Pages
{
    public partial class Auto : Page
    {
        private int failedAttempts;
        private DateTime lockEndTime;
        private DispatcherTimer timer;

        public Auto()
        {
            InitializeComponent();
            failedAttempts = 0;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            if (failedAttempts >= 3 && DateTime.Now < lockEndTime)
            {
                MessageBox.Show("Слишком много неудачных попыток. Пожалуйста, подождите.");
                return;
            }

            string login = tbLogin.Text.Trim();
            string password = tbPassword.Text.Trim();
            string hashPassword = Hash.HashPassword(password);

            var db = Helper.GetContext();
            var user = db.Clients.Where(x => x.Mail == login && x.Password == hashPassword).FirstOrDefault();

            if (user != null)
            {
                MessageBox.Show("Вы вошли под: " + user.Authorization.Role.ToString());
                LoadPage(user.Authorization.Role.ToString(), user);
                ResetFields();
            }
            else
            {
                failedAttempts++;

                if (failedAttempts >= 3)
                {
                    LockControls();
                }
                else
                {
                    MessageBox.Show("Вы ввели логин или пароль неверно!");
                    tbLogin.Clear();
                    tbPassword.Clear();
                    tbCaptcha.Clear();
                    GenerateCaptcha();
                    tblCaptcha.Visibility = Visibility.Visible;
                    tblCaptcha.Text = CaptchaGenerator.GenerateCaptchaText(6);
 
                }
            }
        }

        private void GenerateCaptcha()
        {
            tbCaptcha.Visibility = Visibility.Visible;
            tblCaptcha.Visibility = Visibility.Visible;

            string captchaText = CaptchaGenerator.GenerateCaptchaText(6);
            tblCaptcha.Text = captchaText;
            tblCaptcha.TextDecorations = TextDecorations.Strikethrough;
        }

        private void LockControls()
        {
            tbLogin.IsEnabled = false;
            tbPassword.IsEnabled = false;
            tbCaptcha.IsEnabled = false;
            btnEnter.IsEnabled = false;
            btnEnterGuest.IsEnabled = false;
            StatusTextBlock.Visibility = Visibility.Visible;
            lockEndTime = DateTime.Now.AddSeconds(10);
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var remainingTime = lockEndTime - DateTime.Now;
            if (remainingTime.TotalSeconds > 0)
            {
                StatusTextBlock.Text = $"Ожидайте: {remainingTime.Seconds} секунд";
            }
            else
            {
                tblCaptcha.Text = CaptchaGenerator.GenerateCaptchaText(6);
                tbCaptcha.Text = "";
                ResetControls();
                StatusTextBlock.Visibility = Visibility.Hidden;
                timer.Stop();
            }
        }

        private void ResetControls()
        {
            tbLogin.IsEnabled = true;
            tbPassword.IsEnabled = true;
            btnEnter.IsEnabled = true;
            tbCaptcha.IsEnabled = true;
            btnEnterGuest.IsEnabled = true;
            failedAttempts = 0;
        }

        private void ResetFields()
        {
            tbLogin.Text = "";
            tbPassword.Text = "";
            tbCaptcha.Text = "";
            tblCaptcha.Visibility = Visibility.Hidden;
            tbCaptcha.Visibility = Visibility.Hidden;
        }

        private void LoadPage(string _role, Clients user)
        {
            failedAttempts = 0;
            switch (_role)
            {
                case "клиент":
                    NavigationService.Navigate(new Client(user, _role));
                    break;
                case "сотрудник":
                    NavigationService.Navigate(new StaffPage(user, _role));
                    break;
            }
        }

        private void btnEnterGuest_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}


