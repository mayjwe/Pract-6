using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Pract_3;
using Pract_3.Models;

namespace Pract_3.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddStaffPage.xaml
    /// </summary>
    public partial class AddStaffPage : Page
    {
        public AddStaffPage()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка на корректность введённых данных
                if (string.IsNullOrWhiteSpace(tbName.Text) ||
                    string.IsNullOrWhiteSpace(tbSurname.Text) ||
                    string.IsNullOrWhiteSpace(tbPatronymic.Text) ||
                    string.IsNullOrWhiteSpace(tbBirthday.Text) ||
                    string.IsNullOrWhiteSpace(tbBusyness.Text) ||
                    string.IsNullOrWhiteSpace(tbPhoneNumber.Text) ||
                    string.IsNullOrWhiteSpace(tbAuthorizationID.Text))
                {
                    MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Проверка на целочисленное значение для ID авторизации и номера телефона
                if (!int.TryParse(tbAuthorizationID.Text, out int authorizationID))
                {
                    MessageBox.Show("Неверный формат ID Авторизации. Пожалуйста, введите целое число.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(tbPhoneNumber.Text, out int phoneNumber))
                {
                    MessageBox.Show("Неверный формат номера телефона. Пожалуйста, введите только цифры.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Проверка даты рождения
                if (!DateTime.TryParse(tbBirthday.Text, out DateTime birthday))
                {
                    MessageBox.Show("Неверный формат даты рождения. Пожалуйста, введите дату в формате дд.мм.гггг.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Создание нового сотрудника без указания ID_Staff
                var newStaff = new Staff
                {
                    ID_Authorization = authorizationID, // ID авторизации
                    Name = tbName.Text,                  // Имя
                    Surname = tbSurname.Text,            // Фамилия
                    Patronymic = tbPatronymic.Text,      // Отчество
                    Birthday = birthday,                 // Дата рождения
                    Busyness = tbBusyness.Text,          // Занятость
                    Phone_number = phoneNumber           // Номер телефона
                };

                // Создание и использование контекста базы данных в одном блоке using
                using (var context = Helper.GetContext())
                {
                    context.Staff.Add(newStaff);  // Добавление нового сотрудника в контекст
                    context.SaveChanges();       // Сохранение изменений в базе данных
                }

                MessageBox.Show("Сотрудник успешно добавлен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.GoBack();      // Возврат на предыдущую страницу
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении сотрудника: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }
}
