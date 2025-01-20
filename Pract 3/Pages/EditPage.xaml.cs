using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Pract_3.Models;

namespace Pract_3.Pages
{
    public partial class EditPage : Page
    {
        private object _currentEntity;
        private string _mode;

        // Конструктор для инициализации страницы с объектом (Staff или Client) и режимом (редактирование или добавление)
        public EditPage(object entity, string mode)
        {
            InitializeComponent();
            _currentEntity = entity;
            _mode = mode;
            InitializeFields();
            SetTitle();
        }

        // Инициализация полей для отображения данных объекта
        private void InitializeFields()
        {
            if (_currentEntity is Staff staff)
            {
                tbName.Text = staff.Name;
                tbSurname.Text = staff.Surname;
                tbPatronymic.Text = staff.Patronymic;
                tbPhoneNumber.Text = staff.Phone_number.ToString();
            }
            else if (_currentEntity is Clients client)
            {
                tbName.Text = client.Name;
                tbSurname.Text = client.Surname;
                tbPatronymic.Text = client.Patronymic;
                tbPhoneNumber.Text = client.Phone_number.ToString();
            }
        }

        // Установка заголовка в зависимости от режима
        private void SetTitle()
        {
            if (_mode == "Добавление")
            {
                TitleTextBlock.Text = "Добавление нового клиента/сотрудника";
            }
            else
            {
                TitleTextBlock.Text = "Редактирование";
            }
        }

        // Обработчик кнопки "Сохранить"
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var db = Helper.GetContext();

            try
            {
                // Преобразование номера телефона в int
                if (!int.TryParse(tbPhoneNumber.Text, out int phoneNumber))
                {
                    MessageBox.Show("Введите корректный номер телефона.");
                    return;
                }

                // Проверка типа объекта и сохранение данных
                if (_currentEntity is Staff staff)
                {
                    staff.Name = tbName.Text;
                    staff.Surname = tbSurname.Text;
                    staff.Patronymic = tbPatronymic.Text;
                    staff.Phone_number = phoneNumber;
                }
                else if (_currentEntity is Clients client)
                {
                    client.Name = tbName.Text;
                    client.Surname = tbSurname.Text;
                    client.Patronymic = tbPatronymic.Text;
                    client.Phone_number = phoneNumber;
                }
                else if (_mode == "Добавление")
                {
                    if (string.IsNullOrWhiteSpace(tbName.Text) || string.IsNullOrWhiteSpace(tbSurname.Text))
                    {
                        MessageBox.Show("Заполните все поля.");
                        return;
                    }

                }

                // Сохранение изменений в базе данных
                db.SaveChanges();
                MessageBox.Show("Изменения сохранены.");
                NavigationService.GoBack(); // Возвращение на предыдущую страницу
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}");
            }
        }

        // Обработчик кнопки "Отменить"
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack(); // Возвращение на предыдущую страницу
        }
    }
}
