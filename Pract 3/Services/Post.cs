using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Pract_3.Services
{
    internal class Post
    {
        /// <summary>
        /// Почта
        /// </summary>
        public string SendEmail(string email)
        {
            try
            {
                MailAddress from = new MailAddress("hanna11haha@mail.ru", "Jerry");
                MailAddress to = new MailAddress(email);
                MailMessage m = new MailMessage(from, to);
                m.Subject = "Код подтверждения";
                string code = new Random().Next(1000, 9999).ToString();
                m.Body = $"Вот ваш код подтверждения: {code}, никому его не сообщайте";
                m.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient("smtp.mail.ru", 587);
                smtp.Credentials = new NetworkCredential("hanna11haha@mail.ru", "u8u4dWcwUudq7TzQGMqV");
                smtp.EnableSsl = true;
                smtp.Send(m);

                return code;
            }
            catch (SmtpException smtpEx)
            {
                MessageBox.Show($"SMTP ошибка: {smtpEx.Message}");
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
                return null;
            }
        }
    }
}
