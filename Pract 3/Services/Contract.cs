using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using Xceed.Words.NET;
using Pract_3;
using Pract_3.Models;
using Pract_3.Services;

namespace Pract_3.Services
{
    internal class Contract
    {
        private string templatePath = "C:\\Users\\Администратор\\Desktop\\Учеба\\модули\\blank-trudovogo-dogovora.docx";

        private Dictionary<string, string> Info;

        public Contract()
        {
            string currentDay = DateTime.Now.Day.ToString();
            string currentMonth = DateTime.Now.ToString("MMMM", new CultureInfo("ru-RU"));

            Info = new Dictionary<string, string>
            {
                { "City", "Новосибирск" },
                { "Name_org", "ООО" },
                { "All_Org", "еее" },
                { "FullDirector", "Романов Александр Маркович" },
                { "Company", "Fabric Forward" },
                { "Address", "г. Новосибирск, ул. Ленина 34" },
                { "Start_data", "6 июня" },
                { "Year", "2025" },
                { "Number_month", "октябрь" },
                { "Salary", "100к" },
                { "FullSalary", "100000" },
                { "Prem", "Премия" },
                { "PremMoney", "15000" },
                { "Seria", "1496" },
                { "Number", "147856" },
                { "Issued", "Отдел УФМС России по Новосибирской области" },
                { "INN", "123654789" },
                { "CurrentDay", currentDay },
                { "CurrentMonth", currentMonth }
            };
        }

        public void GenerateContract(int staffId)
        {
            try
            {
                using (var context = Helper.GetContext())
                {
                    var staff = context.Staff.FirstOrDefault(s => s.ID_Staff == staffId);
                    if (staff == null)
                    {
                        MessageBox.Show($"Сотрудник с ID {staffId} не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    using (DocX document = DocX.Load(templatePath))
                    {
                        Dictionary<string, string> replacements = new Dictionary<string, string>
                        {
                            { "{ID}", staff.ID_Staff.ToString() },
                            { "{City}", Info["City"] },
                            { "{Name_org}", Info["Name_org"] },
                            { "{All_Org}", Info["All_Org"] },
                            { "{FIO}", $"{staff.Surname} {staff.Name} {staff.Patronymic}" },
                            { "{Post}", staff.Busyness },
                            { "{Company}", Info["Company"] },
                            { "{Address}", Info["Address"] },
                            { "{Number}", Info["Number"] },
                            { "{INN}", Info["INN"] },
                            { "{Start_data}", Info["Start_data"] },
                            { "{FullDirector}", Info["FullDirector"] },
                            { "{Year}", Info["Year"] },
                            { "{Number_month}", Info["Number_month"] },
                            { "{Salary}", Info["Salary"] },
                            { "{FullSalary}", Info["FullSalary"] },
                            { "{Prem}", Info["Prem"] },
                            { "{PremMoney}", Info["PremMoney"] },
                            { "{Seria}", Info["Seria"] },
                            { "{Issued}", Info["Issued"] },
                            { "{CurrentDay}", Info["CurrentDay"] },
                            { "{CurrentMonth}", Info["CurrentMonth"] }
                        };

                        foreach (var pair in replacements)
                        {
                            document.ReplaceText(pair.Key, pair.Value, false, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                        }

                        string savePath = Path.Combine("C:\\Users\\Администратор\\Desktop\\Учеба\\модули\\", $"Contract_{staff.Surname}_{staff.Name}.docx");
                        document.SaveAs(savePath);

                        MessageBox.Show($"Договор успешно сохранен в:\n{savePath}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при генерации договора: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
