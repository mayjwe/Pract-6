using System;
using System.Windows;
using System.Windows.Controls;
using Pract_3;
using Pract_3.Models;
using Pract_3.Services;

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
          
                var newStaff = new Staff
                {
                    ID_Authorization = int.TryParse(tbAuthorizationID.Text, out var AuthorizationID) ? AuthorizationID : 0,
                    Name = tbName.Text.Trim(),
                    Surname = tbSurname.Text.Trim(),
                    Patronymic = tbPatronymic.Text.Trim(),
                    Birthday = DateTime.TryParse(tbBirthday.Text, out var Birthday) ? Birthday : DateTime.MinValue,
                    Busyness = tbBusyness.Text.Trim(),
                    Phone_number = int.TryParse(tbPhoneNumber.Text, out var PhoneNumber) ? PhoneNumber : 0,
                };
            ValidateStaff validate = new ValidateStaff();
            string validationMessage = validate.ValidateStaf(newStaff);
            if (!string.IsNullOrEmpty(validationMessage))
            {
                MessageBox.Show(validationMessage, "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var context = Helper.GetContext())
                {
                    context.Staff.Add(newStaff);
                    context.SaveChanges();
                }

                MessageBox.Show("Сотрудник успешно добавлен", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении сотрудника: {ex.Message}", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            using (var context = Helper.GetContext())
                {
                    context.Staff.Add(newStaff);
                    context.SaveChanges();
                }

                MessageBox.Show("Сотрудник успешно добавлен", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.GoBack();
            
        }
    }
}
