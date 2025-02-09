using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Pract_3.Models;
using Pract_3.Services;

namespace Pract_3.Pages
{

    public partial class Password : Page
    {
        private int? _clientId = null;
        private string _email;
        private string _confirmationCode;
        private DispatcherTimer timer;
        private int remainingTime;

        public Password(string login)
        {
            InitializeComponent();
            CreateTimer();
            FindUser(login);
        }

        /// <summary>
        /// Обработчик нахождения пользователя
        /// </summary>
        /// <param name="login"></param>
        private void FindUser(string login)
        {
            using (var context = Helper.GetContext())
            {
                var client = context.Clients
                    .AsNoTracking()
                    .FirstOrDefault(u => u.Mail == login);

                if (client != null)
                {
                    _clientId = client.ID_Clients;
                    _email = client.Mail;
                }
                else
                {
                    _clientId = null;
                    MessageBox.Show("Пользователь не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        /// <summary>
        /// Обработчик времени
        /// </summary>
        private void CreateTimer()
        {
            timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += Timer_Tick;
        }

        /// <summary>
        /// Обработчик времени, когда должен отправиться код повторно
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            remainingTime--;
            if (remainingTime <= 0)
            {
                timer.Stop();
                btnSend.IsEnabled = true;
                txtbTimer.Visibility = Visibility.Hidden;
                return;
            }
            txtbTimer.Text = $"Отправить код повторно \nчерез: {remainingTime} секунд";
        }

        /// <summary>
        /// Обработчик почты
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            if (_email != null)
            {
                Post confCode = new Post();
                _confirmationCode = confCode.SendEmail(_email);
                btnSend.IsEnabled = false;
                remainingTime = 60;
                txtbTimer.Visibility = Visibility.Visible;
                timer.Start();
            }
            else
            {
                MessageBox.Show("Email сотрудника не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Сохранение данных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!_clientId.HasValue)
            {
                MessageBox.Show("Ошибка: пользователь не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var context = Helper.GetContext())
            {
                var existingUser = context.Clients.Find(_clientId.Value);
                if (existingUser == null)
                {
                    MessageBox.Show("Ошибка: пользователь не найден в базе данных!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtbNewPassword.Text) || string.IsNullOrWhiteSpace(txtbConfirmPassword.Text))
                {
                    MessageBox.Show("Пароль не может быть пустым", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (txtbNewPassword.Text != txtbConfirmPassword.Text)
                {
                    MessageBox.Show("Пароли не совпадают", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string password = txtbConfirmPassword.Text;
                string hashPassw = Hash.HashPassword(password);
                if (hashPassw == existingUser.Password)
                {
                    MessageBox.Show("Новый пароль не может быть таким же, как старый", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                existingUser.Password = hashPassw;
                try
                {
                    context.SaveChanges();
                    MessageBox.Show("Данные успешно сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении пароля: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            NavigationService.GoBack();
        }

        /// <summary>
        /// Кнопка для продолжения смены пароля
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            if (txtbConfirmCode.Text == _confirmationCode)
            {
                txtbConfirmCode.IsEnabled = false;
                if (remainingTime > 0)
                {
                    timer.Stop();
                    txtbTimer.Visibility = Visibility.Hidden;
                }
                else
                {
                    btnSend.IsEnabled = false;
                }
                btnContinue.IsEnabled = false;
                txtbNewPassword.IsEnabled = true;
                txtbConfirmPassword.IsEnabled = true;
                btnSave.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("Неверный код подтверждения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
