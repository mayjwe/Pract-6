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

        public EditPage(object entity, string mode)
        {
            InitializeComponent();
            _currentEntity = entity;
            _mode = mode;
            InitializeFields();
            SetTitle();
        }

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

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var db = Helper.GetContext();

            try
            {
                if (!int.TryParse(tbPhoneNumber.Text, out int phoneNumber))
                {
                    MessageBox.Show("Введите корректный номер телефона.");
                    return;
                }

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

                db.SaveChanges();
                MessageBox.Show("Изменения сохранены.");
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}");
            }
        }

        
    }
}
