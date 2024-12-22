using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RegIN_Kurlishuk.Pages
{
    /// <summary>
    /// Логика взаимодействия для Confirmation.xaml
    /// </summary>
    public partial class Confirmation : Page
    {
        /// <summary>
        /// Тип перечесления для чего используется подтверждение
        /// </summary>
        public enum TypeConfirmation
        {
            Login,
            Regin
        }
        /// <summary>
        /// Для его используется подтвверждение
        /// </summarгу>
        TypeConfirmation ThisTypeConfirmation;
        /// <summary>
        /// Код отправленный на почту пользователя
        /// </summary>
        public int Code = 0;

        public Confirmation(TypeConfirmation TypeConfirmation)
        {
            InitializeComponent();
            // Запоменаем полученный тип подтверждения
            ThisTypeConfirmation = TypeConfirmation;
            // Отправляем сообщение на почту пользователя
            SendMailCode();
        }

        /// <summary>
        /// Метод отправки сообщения на почту
        /// </summary>
        public void SendMailCode()
        {
            // Генерируем случайное число
            Code = new Random().Next(100000, 999999);
            // Отправляем число на почту авторизуемого польхователя
            Classes.SendMail.SendMessage($"Login code: {Code}", MainWindow.mainWindow.UserLogIn.Login);
            // Инициализируем процесс в потоке для отправки повторного письма
            Thread TSendMailCode = new Thread(TimerSendMailCode);
            // Отправляем письмо
            TSendMailCode.Start();
        }

        /// <summarу>
        /// Ожидание отправки нового кода
        /// </summarгу>
        public void TimerSendMailCode()
        {
            // Запускаем цикл в 60 шагов
            for (int i = 0; i < 60; i++)
            {
                // Выполняем вне потока
                Dispatcher.Invoke(() =>
                {
                    // Изменяем данные на текстовом поле
                    LTimer.Content = $"A second message can be sent after {(60 - 1)} seconds";
                    // Ждём 1 секунду
                });
                Thread.Sleep(1000);
                // По истечению таймера вне потока
            }
            Dispatcher.Invoke(() =>
            {
                // Включаем кнопку отправить повторно
                BSendMessage.IsEnabled = true;
                // Изменяем данные на текстовом поле
                LTimer.Content = "";
            });
        }

        /// <summary>
        /// Отправка кода подтверждения на почту пользователя
        /// </summary>
        private void SendMail(object sender, RoutedEventArgs e)
        {
            // Вызываем метод отправки сообщения на почту пользователя
            SendMailCode();
        }

        /// <summary>
        /// Вызыв метода проверки отправленного кода на почту и введённого пользователем
        /// </summary>
        

        /// <summary>
        /// метода проверки отправленного кода на почту и введённого пользователем
        /// </summary>
        void SetCode()
        {
            // Если текст в текстовом поле совпадает с кодом отправленным на почту
            // Если текстовое поле активировано
            if (TbCode.Text == Code.ToString() && TbCode.IsEnabled == true)
            {
                // Выключаем активацию поля
                TbCode.IsEnabled = false;
                // Если тип подтверждения является авторизацией
                if (ThisTypeConfirmation == TypeConfirmation.Login)
                    // Выводим сообщение о том что пользователь авторизовался
                    MessageBox.Show("Авторизация пользователя успешно подтверждена.");
                else
                {
                    // Если тип подтверждения является регистрацией
                    MainWindow.mainWindow.UserLogIn.SetUser();
                    // Выводим сообщение о том что пользователь зарегистрировался
                    MessageBox.Show("Регистрация пользователя успешно подтверждена.");
                }
            }
        }
        /// <summary>
        /// Метод открытия страницы авторизации
        /// </summary>
        private void OpenLogin(object sender, MouseButtonEventArgs e)
        {
            MainWindow.mainWindow.OpenPage(new Login());
        }

        private void SetCode(object sender, RoutedEventArgs e)
        {
            // Если текст введёный в поле 6 символов
            if (TbCode.Text.Length == 6)
                // Вызываем метод проверки кода
                SetCode();
        }
    }
}
