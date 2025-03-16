using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Pract_3.Models;
using Pract_3.Services;
using System.Data.Entity;

namespace Pract_3.Pages
{
    public partial class EditPage : Page
    {
        private object _currentEntity;
        private string _mode;
        private int? _entityId;

        public EditPage(object entity, string mode)
        {
            InitializeComponent();
            _mode = mode;

            if (entity is Staff staff)
                _entityId = staff.ID_Staff;
            else if (entity is Clients client)
                _entityId = client.ID_Clients;

            LoadEntity();
            SetTitle();
        }

        private void LoadEntity()
        {
            using (var db = new AtelierEntities())
            {
                _currentEntity = db.Staff.Find(_entityId);
                if (_currentEntity == null)
                {
                    _currentEntity = db.Clients.Find(_entityId);
                }

                InitializeFields();
            }
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
            TitleTextBlock.Text = _mode == "Добавление" ? "Добавление нового клиента/сотрудника" : "Редактирование";
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(tbPhoneNumber.Text, out int phoneNumber))
                {
                    MessageBox.Show("Введите корректный номер телефона.");
                    return;
                }

                using (var db = new AtelierEntities())
                {
                    if (_mode == "Редактирование" && _entityId.HasValue)
                    {
                        if (_currentEntity is Staff)
                        {
                            var staff = db.Staff.Find(_entityId);
                            if (staff != null)
                            {
                                staff.Name = tbName.Text;
                                staff.Surname = tbSurname.Text;
                                staff.Patronymic = tbPatronymic.Text;
                                staff.Phone_number = phoneNumber;
                                db.Entry(staff).State = EntityState.Modified;
                            }
                        }
                        else if (_currentEntity is Clients)
                        {
                            var client = db.Clients.Find(_entityId);
                            if (client != null)
                            {
                                client.Name = tbName.Text;
                                client.Surname = tbSurname.Text;
                                client.Patronymic = tbPatronymic.Text;
                                client.Phone_number = phoneNumber;
                                db.Entry(client).State = EntityState.Modified;
                            }
                        }
                    }
                    else if (_mode == "Добавление")
                    {
                        if (string.IsNullOrWhiteSpace(tbName.Text) || string.IsNullOrWhiteSpace(tbSurname.Text))
                        {
                            MessageBox.Show("Заполните все поля.");
                            return;
                        }

                        if (_currentEntity is Staff)
                        {
                            var newStaff = new Staff
                            {
                                Name = tbName.Text,
                                Surname = tbSurname.Text,
                                Patronymic = tbPatronymic.Text,
                                Phone_number = phoneNumber
                            };
                            db.Staff.Add(newStaff);
                        }
                        else if (_currentEntity is Clients)
                        {
                            var newClient = new Clients
                            {
                                Name = tbName.Text,
                                Surname = tbSurname.Text,
                                Patronymic = tbPatronymic.Text,
                                Phone_number = phoneNumber
                            };
                            db.Clients.Add(newClient);
                        }
                    }

                    db.SaveChanges();
                    MessageBox.Show("Изменения сохранены.");
                    NavigationService.GoBack();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}");
            }
        }

        private void btnCreateFile_Click(object sender, RoutedEventArgs e)
        {
            if (_entityId.HasValue)
            {
                Contract contract = new Contract();
                contract.GenerateContract(_entityId.Value);
            }
            else
            {
                MessageBox.Show("Сотрудник не выбран или не загружен.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }
}
