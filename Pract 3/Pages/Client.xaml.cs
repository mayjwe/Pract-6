using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Pract_3.Models;

namespace Pract_3.Pages
{
    public partial class Client : Page
    {
        private List<DisplayItem> _displayItems;
        private List<DisplayItem> _filteredItems;

        public Client(Clients user, string role)
        {
            InitializeComponent();
            LoadClientsAndStaff();
            cbRole.SelectedIndex = 0; // Устанавливаем фильтр "Все" по умолчанию
        }

        public class DisplayItem
        {
            public string FullName { get; set; }
            public string Role { get; set; }
            public int PhoneNumber { get; set; }
            public string PhotoUrl { get; set; }
        }

        private void LoadClientsAndStaff()
        {
            try
            {
                var db = Helper.GetContext();

                var clients = db.Clients.ToList().Select(c => new DisplayItem
                {
                    FullName = $"{c.Surname} {c.Name} {c.Patronymic}",
                    Role = "Клиенты",
                    PhoneNumber = c.Phone_number,
                    PhotoUrl = "C:\\Users\\Администратор\\Downloads\\good_tick.png"
                }).ToList();

                var staff = db.Staff.ToList().Select(s => new DisplayItem
                {
                    FullName = $"{s.Surname} {s.Name} {s.Patronymic}",
                    Role = "Сотрудники",
                    PhoneNumber = s.Phone_number,
                    PhotoUrl = "C:\\Users\\Администратор\\Downloads\\p1.jpg"
                }).ToList();

                _displayItems = clients.Concat(staff).ToList();
                _filteredItems = new List<DisplayItem>(_displayItems);
                ClientsListView.ItemsSource = _filteredItems;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void FilterClients(string role)
        {
            if (role == "Все")
            {
                _filteredItems = new List<DisplayItem>(_displayItems);
            }
            else
            {
                _filteredItems = _displayItems.Where(item => item.Role == role).ToList();
            }

            ClientsListView.ItemsSource = _filteredItems;
        }

        private void cbRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = cbRole.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                FilterClients(selectedItem.Content.ToString());
            }
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = tbSearch.Text.ToLower();
            ClientsListView.ItemsSource = _filteredItems.Where(item =>
                item.FullName.ToLower().Contains(searchText) ||
                item.PhoneNumber.ToString().Contains(searchText)).ToList();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddStaffPage());
        }

        private void ClientsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ClientsListView.SelectedItem is DisplayItem selectedItem)
            {
                var db = Helper.GetContext();
                var entity = selectedItem.Role == "Сотрудники"
                    ? (object)db.Staff.FirstOrDefault(s => s.Phone_number == selectedItem.PhoneNumber)
                    : (object)db.Clients.FirstOrDefault(c => c.Phone_number == selectedItem.PhoneNumber);

                NavigationService.Navigate(new EditPage(entity, "Редактирование"));
            }
        }
    }
}
