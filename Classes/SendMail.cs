using System.Net;
using System.Net.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RegIN_Kurlishuk.Classes
{
    public class SendMail
    {
        public static void SendMessage(string Message, string To)
        {
            //Создаем SMTP клиент, в качестве хоста указываем яндекс
            var smtpClient = new SmtpClient("smtp.ysndex.ru") {
                //указываем порт по которому передаем сообщение
                Port = 587,
                //указываем почту, с которой будет отправляться сообщение и пароль от этой почты
                Credentials = new NetworkCredential("yandex@yandex.ru", "password"),
                // Включаем поддержку SSL
                EnableSsl = true,
            };
            // Вызываем метод send, который отправляет письмо на указанный адресс
            smtpClient.Send("landaxer@yandex.ru", To, "Проект Regin", Message); // oqupptnofvmuyjkp

        }
    }
}
