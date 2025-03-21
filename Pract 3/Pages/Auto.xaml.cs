﻿using Pract_3.Models;
using Pract_3.Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        /// <summary>
        /// Обработчик нажатия кнопки входа
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            if (failedAttempts >= 3 && DateTime.Now < lockEndTime)
            {
                MessageBox.Show("Слишком много неудачных попыток. Пожалуйста, подождите.");
                return;
            }

            if (!IsWithinWorkingHours())
            {
                MessageBox.Show("Доступ запрещён. Рабочее время с 10:00 до 19:00.");
                return;
            }

            string login = tbLogin.Text.Trim();
            string password = tbPassword.Text.Trim();
            string hashPassword = Hash.HashPassword(password);

            var db = Helper.GetContext();
            var user = db.Clients.Where(x => x.Mail == login && x.Password == hashPassword).FirstOrDefault();

            if (user != null)
            {
                GreetUser(user);
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

        /// <summary>
        /// Генерация капчи
        /// </summary>
        private void GenerateCaptcha()
        {
            tbCaptcha.Visibility = Visibility.Visible;
            tblCaptcha.Visibility = Visibility.Visible;

            string captchaText = CaptchaGenerator.GenerateCaptchaText(6);
            tblCaptcha.Text = captchaText;
            tblCaptcha.TextDecorations = TextDecorations.Strikethrough;
        }

        /// <summary>
        /// Блокировка элементов ввода при превышении количества попыток входа
        /// </summary>
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

        /// <summary>
        /// Таймер обратного отсчёта блокировки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// Разблокировка элементов ввода
        /// </summary>
        private void ResetControls()
        {
            tbLogin.IsEnabled = true;
            tbPassword.IsEnabled = true;
            btnEnter.IsEnabled = true;
            tbCaptcha.IsEnabled = true;
            btnEnterGuest.IsEnabled = true;
            failedAttempts = 0;
        }

        /// <summary>
        /// Очистка полей ввода
        /// </summary>
        private void ResetFields()
        {
            tbLogin.Text = "";
            tbPassword.Text = "";
            tbCaptcha.Text = "";
            tblCaptcha.Visibility = Visibility.Hidden;
            tbCaptcha.Visibility = Visibility.Hidden;
        }
        /// <summary>
        /// Загрузка страницы в зависимости от роли пользователя 
        /// </summary>
        /// <param name="_role"></param>
        /// <param name="user"></param>
        private void LoadPage(string _role, Clients user)
        {
            failedAttempts = 0;
            switch (_role)
            {
                case "клиент":
                    NavigationService.Navigate(new Client(user));
                    break;
                case "сотрудник":
                    NavigationService.Navigate(new StaffPage(user));
                    break;
            }
        }

        private void btnEnterGuest_Click(object sender, RoutedEventArgs e)
        {
        }


        /// <summary>
        /// Проверка на соответствие рабочего времени
        /// </summary>
        /// <returns></returns>
        private bool IsWithinWorkingHours()
        {
            TimeSpan startTime = new TimeSpan(0, 0, 0);
            TimeSpan endTime = new TimeSpan(23, 59, 0);
            TimeSpan currentTime = DateTime.Now.TimeOfDay;

            return currentTime >= startTime && currentTime <= endTime;
        }

        private void GreetUser(Clients user)
        {
            string greeting = GetGreetingBasedOnTime();
            MessageBox.Show($"{greeting} {user.Surname} {user.Name} {user.Patronymic}!");
        }

        /// <summary>
        /// Вывод сообщения в зависимости от времени
        /// </summary>
        /// <returns></returns>
        private string GetGreetingBasedOnTime()
        {
            TimeSpan morningStart = new TimeSpan(10, 0, 0);
            TimeSpan dayStart = new TimeSpan(12, 1, 0);
            TimeSpan eveningStart = new TimeSpan(17, 1, 0);
            TimeSpan currentTime = DateTime.Now.TimeOfDay;

            if (currentTime >= morningStart && currentTime < dayStart)
            {
                return "Доброе утро";
            }
            else if (currentTime >= dayStart && currentTime < eveningStart)
            {
                return "Добрый день";
            }
            else
            {
                return "Добрый вечер";
            }
        }

        /// <summary>
        /// Кнопка забытого пароля
        /// </summary>
        /// <param object="sender"></param>
        /// <param MouseButtonEventArgs="e"></param>
        private void tblForgotPassword_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (tbLogin.Text != null)
            {
                string login = tbLogin.Text;
                tbPassword.Clear();
                NavigationService.Navigate(new Password(login));
            }
            else
            {
                MessageBox.Show("Введите логин пользователя");
            }
        }

        private void btnForgotPassword_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
