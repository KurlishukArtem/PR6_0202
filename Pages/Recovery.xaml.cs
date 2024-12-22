using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Mysqlx.Expr;
using RegIN_Kurlishuk.Pages;

namespace RegIN_Kurlishuk.Pages
{

    public partial class Recovery : Page
    {
        /// <summary>
        /// Логин введёный пользователем
        /// </summary>
        string OldLogin;
        /// <summary>
        /// Переменная отвечающая за ввод капчи
        /// </summary>
        bool IsCapture = false;
        public Recovery()
        {
            InitializeComponent();
            // Подписываемся на успешную авторизацию пользователя
            MainWindow.mainWindow.UserLogIn.HandlerCorrectLogin += CorrectLogin;
            MainWindow.mainWindow.UserLogIn.HandlerInCorrectLogin += InCorrectLogin;
            // Подписываемся на неуспешную авторизацию пользователя
            // Подписываемся на успешный ввод пароля
            Capture.HandlerCorrectCapture += CorrectCapture;
        }
        /// <summary>
        /// Метод правильного ввода логина
        /// </summarгу>
        private void CorrectLogin()
        {
            // Если предыдущей введённый логин не равен тому что введён сейчас
            if (OldLogin != TbLogin.Text)
            {
                // Вызываем метод уведомления, передавая сообщение, имя пользователя и цвет
                SetNotification("Hi, " + MainWindow.mainWindow.UserLogIn.Name, Brushes.Black);
                // Используем конструкцию try-catch
                try
                {
                    // Инициализируем BitmapImage, который будет содержать изображение пользователя
                    BitmapImage bilmg = new BitmapImage();
                    // Открываем поток, хранилищем которого является память и указываем в качестве источни
                    MemoryStream ms = new MemoryStream(MainWindow.mainWindow.UserLogIn.Image);
                    // Сиганлизируем о начале инициализации
                    bilmg.BeginInit();
                    // Указываем источник потока
                    bilmg.StreamSource = ms;
                    // Сигнализируем о конце инициализации
                    bilmg.EndInit();
                    // Получаем ImageSoruce
                    ImageSource imgsrc = bilmg;
                    // Создаём анимацию старта
                    DoubleAnimation StartAnimation = new DoubleAnimation();
                    // Указываем значение от которого она выполняется
                    StartAnimation.From = 1;
                    // Указываем значение до которого она выполняется
                    StartAnimation.To = 0;
                    // Указываем продолжительность выполнения
                    StartAnimation.Duration = TimeSpan.FromSeconds(0.6);
                    // Присваиваем событие при конце анимации
                    StartAnimation.Completed += delegate
                    {
                        // Устанавливаем изображение
                        IUser.Source = imgsrc;
                        // Создаём анимацию конца
                        DoubleAnimation EndAnimation = new DoubleAnimation();
                        // Указываем значение от которого она выполняется
                        EndAnimation.From = 0;
                        // Указываем значение до которого она выполняется
                        EndAnimation.To = 1;
                        // Указываем продолжительность выполнения
                        EndAnimation.Duration = TimeSpan.FromSeconds(1.2);
                        // Запускаем анимацию плавной смены на изображении
                        IUser.BeginAnimation(Image.OpacityProperty, EndAnimation);
                    };
                    // Запускаем анимацию плавной смены на изображении
                    IUser.BeginAnimation(Image.OpacityProperty, StartAnimation);
                }

                catch (Exception exp)
                {
                    // Если возникла ошибка, выводим в дебаг
                    Debug.WriteLine(exp.Message);
                    // Запоминаем введёный логин
                    oldLogin = TbLogin.Text;
                    // Вызываем метод создания нового пароля
                    SendNewPassword();
                };
            }
        }
        /// <summary>
        /// Метод неправильно ввода логина
        /// </summary>
        private void InCorrectLogin()
        {
            // Если пользователь идентифицирован как личность, или указаны ошибки
            if (LNameUser.Content != "")
            {
                // Очищаем приветствие пользователя
                LNameUser.Content = "";
                // Создаём анимацию старта
                DoubleAnimation StartAnimation = new DoubleAnimation();
                // Указываем значение от которого она выполняется
                StartAnimation.From = 1;
                // Указываем значение до которого она выполняется
                StartAnimation.To = 0;
                // Указываем продолжительность выполнения
                StartAnimation.Duration = TimeSpan.FromSeconds(0.6);
                // Присваиваем событие при конце анимации
                StartAnimation.Completed += delegate
                {
                    // Указываем стандартный логотип в качестве изображения пользователя
                    IUser.Source = new BitmapImage(new Uri("pack://application:,,,/Images/ic-user.png"));
                    // Создаём анимацию конца
                    DoubleAnimation EndAnimation = new DoubleAnimation();
                    // Указываем значение от которого она выполняется
                    EndAnimation.From = 0;
                    // Указываем значение до которого она выполняется
                    EndAnimation.To = 1;
                    // Указываем продолжительность выполнения
                    EndAnimation.Duration = TimeSpan.FromSeconds(1.2);
                    // Запускаем анимацию плавной смены на изображении
                    IUser.BeginAnimation(OpacityProperty, EndAnimation);
                };
                // Запускаем анимацию плавной смены на изображении
                IUser.BeginAnimation(OpacityProperty, StartAnimation);
            }
            // сообщение о том что логин ввелён не парвильно
            if (TbLogin.Text.Length > 0)
                // Выводим сообщение о том, что логин введён не верно, цвет текста красный
                SetNotification("Login is incorrect", Brushes.Red);
        }
        /// <summary>
        /// Метод успешного ввода капчи
        /// </summary>
        private void CorrectCapture()
        {
            // Отключаем элемент капчи
            Capture.IsEnabled = false;
            // Запоминаем что ввод капчи осуществлён
            IsCapture = true;
            // Вызываем генерацию нового пароля
            SendNewPassword();
        }
        /// <summaryу>
        /// Метод ввода логина
        /// </summary>
        private void SetLogin(object sender, KeyEventArgs e)
        {
            // Если нажата клавиша Enter
            if (e.Key == Key.Enter)
                // Вызываем полуение данных пользователя по логину
                MainWindow.mainWindow.UserLogIn.GetUserLogin(TbLogin.Text);
        }
        private void setLogin(object sender, RoutedEventArgs e) =>
        // Вызываем полуение данных пользователя по логину
        MainWindow.mainWindow.UserLogIn.GetUserLogin(TbLogin.Text);

        public void SendNewPassword()
        {
            // Если пройдена капча
            if (IsCapture)
            {
                // Если пароль не является пустым, а это значит пользователь ввёл правильную почту
                if (MainWindow.mainWindow.UserLogIn.Password != String.Empty)
                {
                    // Создаём анимацию старта
                    DoubleAnimation StartAnimation = new DoubleAnimation();
                    // Указываем значение от которого она выполняется
                    // Указываем значение до которого она выполняется
                    StartAnimation.From = 1;
                    StartAnimation.To = 0;
                    // Указываем продолжительность выполнения
                    StartAnimation.Duration = TimeSpan.FromSeconds(0.6);
                    // Присваиваем событие при конце анимации
                    StartAnimation.Completed += delegate
                    {
                        // Указываем стандартный логотип в качестве изображения пользователя
                        IUser.Source = new BitmapImage(new Uri("pack://application:,,,/Images/ic-mail.png"));
                        // Создаём анимацию конца
                        DoubleAnimation EndAnimation = new DoubleAnimation();
                        // Указываем значение от которого она выполняется
                        EndAnimation.From = 0;
                        // Указываем значение до которого она выполняется
                        EndAnimation.To = 1;
                        // Указываем продолжительность выполнения
                        EndAnimation.Duration = TimeSpan.FromSeconds(1.2);
                        // Запускаем анимацию плавной смены на изображении
                        IUser.BeginAnimation(OpacityProperty, EndAnimation);
                        // Запускаем анимацию плавной смены на изображении
                    };
                    IUser.BeginAnimation(OpacityProperty, StartAnimation);
                    // Выводим сообщение о том что новый пароль будет отправлен на почту
                    SetNotification("An email has been sent to your email.", Brushes.Black);
                    // Вызываем функцию создания нового пароля
                    MainWindow.mainWindow.UserLogIn.CreateNewPassword();
                }
            }
        }
        private void OpenLogin(object sender, MouseButtonEventArgs e)
        {
            MainWindow.mainWindow.OpenPage(new Login());
        }
    }
}
