using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Xml.Linq;
using Pract_3.Models;


namespace Pract_3.Pages
{
    public partial class Client : Page
    {
        private List<DisplayItem> _displayItems;
        private List<DisplayItem> _filteredItems;

        public Client(Clients user)
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


        /// <summary>
        /// Отображение пользователя
        /// </summary>
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

        /// <summary>
        /// Фильтр пользователей
        /// </summary>
        /// <param name="role"></param>
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

        /// <summary>
        /// Выбор роли
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = cbRole.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                FilterClients(selectedItem.Content.ToString());
            }
        }

        /// <summary>
        /// Текст
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = tbSearch.Text.ToLower();
            ClientsListView.ItemsSource = _filteredItems.Where(item =>
                item.FullName.ToLower().Contains(searchText) ||
                item.PhoneNumber.ToString().Contains(searchText)).ToList();
        }

        /// <summary>
        /// Переход на страницу добавления
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddStaffPage());
        }

        /// <summary>
        /// Обработчик двойного клика
        /// </summary>
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

        /// <summary>
        /// ПДФ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrintListButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем FlowDocument из FlowDocumentReader 

            FlowDocument doc = flowDocumentReader.Document;
            if (doc == null)
            {
                MessageBox.Show("Документ не найден.");
                return;
            }
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                IDocumentPaginatorSource idpSource = doc;
                printDialog.PrintDocument(idpSource.DocumentPaginator, "Список сотрудников и клиентов"); 
            }
        }

        /// <summary>
        /// Информация о моделях
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrintModelOrderButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var db = Helper.GetContext();
                var modelsList = db.Models.Select(m => new
                {
                    Name = m.Name,
                    Purpose = m.Purpose,
                    Size = m.Size.ToString(),

                }).ToList();
                if (modelsList.Count == 0)
                {
                    MessageBox.Show("Нет моделей");
                    return;
                }
                FlowDocument doc = new FlowDocument();
                doc.Blocks.Add(new Paragraph(new Run("Информация о моделях"))
                {
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center
                });

                Table table = new Table();
                table.Columns.Add(new TableColumn { Width = new GridLength(150) });
                table.Columns.Add(new TableColumn { Width = new GridLength(150) });
                table.Columns.Add(new TableColumn { Width = new GridLength(80) });

                TableRowGroup headerGroup = new TableRowGroup();
                TableRow headerRow = new TableRow();
                headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Название"))) { FontWeight = FontWeights.Bold });
                headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Назначение"))) { FontWeight = FontWeights.Bold });
                headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Размер"))) { FontWeight = FontWeights.Bold });

                headerGroup.Rows.Add(headerRow);
                table.RowGroups.Add(headerGroup);

                TableRowGroup rowGroup = new TableRowGroup();
                foreach (var model in modelsList)
                {
                    TableRow row = new TableRow();
                    row.Cells.Add(new TableCell(new Paragraph(new Run(model.Name))));
                    row.Cells.Add(new TableCell(new Paragraph(new Run(model.Purpose))));
                    row.Cells.Add(new TableCell(new Paragraph(new Run(model.Size))));
                    rowGroup.Rows.Add(row);
                }
                table.RowGroups.Add(rowGroup);
                doc.Blocks.Add(table);
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    IDocumentPaginatorSource idpSource = doc;
                    printDialog.PrintDocument(idpSource.DocumentPaginator, "Информация о моделях");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при печати: " + ex.Message);
            }
        }


    }
}
