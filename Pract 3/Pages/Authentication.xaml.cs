using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
using System.Windows.Threading;
using Pract_3.Models;
using Pract_3.Services;

namespace Pract_3.Pages
{
    /// <summary>
    /// Логика взаимодействия для Authentication.xaml
    /// </summary>
    public partial class Authentication : Page
    {

        private Clients _client;
        private string _positionAtWork;
        private string _email;
        private string _confirmationCode;
        private DispatcherTimer timer;
        private int remainingTime;

        public Authentication(Clients client, string idPositionAtWork)
        {
            InitializeComponent();
            CreateTimer();
            _client = client;
            _positionAtWork = idPositionAtWork;
            _email = client.Mail;
        }

        /// <summary>
        /// Обработчик времени
        /// </summary>
        private void CreateTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }

        /// <summary>
        /// Обработчик таймера
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
        /// Обработчик времени
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (txtbConfirmCode.Text == _confirmationCode)
            {
                LoadPage(_client, _positionAtWork);
            }
        }

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
                MessageBox.Show("Email сотрудника не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Обработчик роли
        /// </summary>
        /// <param name="client"></param>
        /// <param name="idPositionAtWork"></param>
        private void LoadPage( Clients client, string idPositionAtWork)
        {
            switch (idPositionAtWork)
            {
                case "клиент":
                    NavigationService.Navigate(new Client(client));
                    break;
                case "сотрудник":
                    NavigationService.Navigate(new StaffPage(client));
                    break;
            }
        }

        private void txtbConfirmCode_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
