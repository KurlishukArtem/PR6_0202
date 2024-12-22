﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using RegIN_Kurlishuk.Classes;
using Slack.Webhooks.Elements;

namespace RegIN_Kurlishuk.Pages
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        string OldLogin;
        int CountSetPassword = 2;
        bool IsCapture = false;
        public Login()
        {
            InitializeComponent();
            // Подписываемся на успешную авторизацию пользователя
            MainWindow.mainWindow.UserLogIn.HandlerCorrectLogin += CorrectLogin;
            MainWindow.mainWindow.UserLogIn.HandlerInCorrectLogin += InCorrectLogin;
            // Подписываемся на неуспешную авторизацию пользователя
            // Подписываемся на успешный ввод пароля
            Capture.HandlerCorrectCapture += CorrectCapture;
        }
        public void CorrectLogin()
        {
            // Если старый логин не соответствует логину введённому в поле
            if (OldLogin != TbLogin.Text)
            {
                // Вызываем метод уведомления, передавая сообщение, имя пользователя и цвет
                SetNotification("Hi, " + MainWindow.mainWindow.UserLogIn.Name, Brushes.Black);
                // Используем конструкцию try-catch
                try
                {
                    // Инициализируем BitmapImage, который будет содержать изображение пользователя
                    BitmapImage bilmg = new BitmapImage();
                    // Открываем поток, хранилищем которого является память и указываем в качестве источника масси

                    MemoryStream ms = new MemoryStream(MainWindow.mainWindow.UserLogIn.Image);
                    // Сиганлизируем о начале инициализации
                    bilmg.BeginInit();
                    // Указываем источник потока
                    bilmg.StreamSource = ms;
                    // Сигнализируем о конце инициализации
                    bilmg.EndInit();
                    // Получаем ImageSource
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
                        // Запускаем анимацию плавной смены на изображении
                        IUser.BeginAnimation(Image.OpacityProperty, StartAnimation);
                    };
                }
                catch (Exception exp)
                {
                    // Если возникла ошибка, выводим в дебаг
                    Debug.WriteLine(exp.Message);
                    // Запоминаем введёный логин
                    OldLogin = TbLogin.Text;
                };
                
            }
        }

        /// <summary>
        /// Метод не успешной авторизации
        /// </summary>
        public void InCorrectLogin()
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
            // Если пароль пользователя более в символов
            if (TbLogin.Text.Length > 0)
                // Вызываем метод отображения ошибки, указывая цвет красный
                SetNotification("Login is incorrect", Brushes.Red);
        }
        public void CorrectCapture()
        {
            // Отключаем элемент капчи
            Capture.IsEnabled = false;
            // Запоминаем что ввод капчи осуществлён
            IsCapture = true;
        }



        /// <summary>
        /// Ввод пароля
        /// </summary>
        private void SetPassword(object sender, KeyEventArgs e)
        {
            // Если пользователь нажал клавишу Enter
            if (e.Key == Key.Enter)
                // Вызываем метод ввода пароля
                SetPassword();
        }
        /// <summary>
        /// Ввод пароля
        /// </summary>
        public void SetPassword()
        {
            // Если пароль пользователья загруженного из БД не пустой
            // Значит что пользователь ввёл правельный логин
            if (MainWindow.mainWindow.UserLogIn.Password != String.Empty)
            {
                // Если капча пройдена
                if (IsCapture)
                {
                    // Если пароль загруженного пользователя совпадает с паролем введёным в поле
                    if (MainWindow.mainWindow.UserLogIn.Password == TbPassword.Password)
                    {
                        // Перенаправляем пользователя на страницу подтверждения
                        // Сообщаем страницы, что проходим подтверждение на авторизацию
                        MainWindow.mainWindow.OpenPage(new Confirmation(Confirmation.TypeConfirmation.Login));
                    }
                    else
                    {
                        // Если пароль не совпадает с загруженным пользователем
                        
                        if (CountSetPassword > 0)
                        {
                            // Выводим предупреждение, сколько попыток осталось, цвет = красный
                            SetNotification($"Password is incorrect, {CountSetPassword} attempts left", Brushes.Red);
                            // Вычитаем попытку ввода пароля
                            CountSetPassword--;
                        }
                        else
                        {
                            
                            
                            // Если попытки ввода пароля закончились
                            // Создаём поток
                            Thread TBlockAutorization = new Thread(BlockAutorization);
                            // Запускаем поток
                            TBlockAutorization.Start();
                            // Отправляем сообщение пользователю о том, что под его аккаунтом кто-то пытается
                            SendMail.SendMessage("An attempt was made to log into your account.",
                            MainWindow.mainWindow.UserLogIn.Login);
                        }
                    }
                }
                else
                    // Если капча не пройдена, вызываем ошибку, цвет красный
                    SetNotification($"Enter capture", Brushes.Red);
            }
        }


        /// <summary>
        /// Метод блокировки авторизации
        /// </summarгу>
        public void BlockAutorization()
        {
            // Запоминаем время блокировки
            DateTime StartBlock = DateTime.Now.AddMinutes(3);
            // Выполняем вне потока
            Dispatcher.Invoke(() =>
            {
                // Отключаем окно ввода логина
                TbLogin.IsEnabled = false;
                // Отключаем окно ввода пароля
                TbPassword.IsEnabled = false;
                // Отключаем окно ввода капчи
                Capture.IsEnabled = false;
            });
            
            // Запускаем цикл в 180 шагов| 180/60 = 3 минуты
            for (int i = 0; i < 180; i++)
            {
                // получаем оставшееся время
                TimeSpan TimeIdle = StartBlock.Subtract(DateTime.Now);
                // Полуаем минуты
                string s_minutes = TimeIdle.Minutes.ToString();
                // Если минуты меньше 10
                if (TimeIdle.Minutes < 10)
                    // Добавляем в
                    s_minutes = "0" + TimeIdle.Minutes;
                // Получаем секунды
                string s_seconds = TimeIdle.Seconds.ToString();
                // Если секунды меньше 10
                if (TimeIdle.Seconds < 10)
                    // Добавляем 0
                    s_seconds = "0" + TimeIdle.Seconds;
                // Вне потока
                Dispatcher.Invoke(() =>
                {
                });
                // Выводим время до разблокировки, цвет карсный
                SetNotification($"Reauthorization available in: {s_minutes}:{s_seconds}", Brushes.Red);
                // Ждём 1 секунду
                Thread.Sleep(1000);
            }
            
            Dispatcher.Invoke(() =>
            // Вне потока
            {
                
                // Выводим логин авторизованного пользователя, цвет чёрный
                SetNotification("Hi, " + MainWindow.mainWindow.UserLogIn.Name, Brushes.Black);
                // Включаем ввод логина
                TbLogin.IsEnabled = true;
                // Включаем ввод пароля
                TbPassword.IsEnabled = true;
                // Включаем капчу
                Capture.IsEnabled = true;
                // Вызываем генерацию новой капчи
                Capture.CreateCapture();
                // Запоминаем о том что капча не введена
                IsCapture = false;
                // Устанавливаем кол-во попыток 2
                CountSetPassword = 2;
            });
        }


        /// <summary>
        /// Ввод логина пользователя
        /// </summary>
        private void SetLogin(object sender, KeyEventArgs e)
        {
            // При нажатии на кнопку Enter
            if (e.Key == Key.Enter)
            {
                // Вызываем метод получения данных пользователя по логину
                MainWindow.mainWindow.UserLogIn.GetUserLogin(TbLogin.Text);
                // Если пароль пользователя введён
                if (TbPassword.Password.Length > 0)
                    // Вызываем метод ввода пароля
                    SetPassword();
            }
        }
        

        /// <summary>
        /// Метод уведомлений пользователя
        /// </summary>
        /// <param name="Message">Сообщение которое необходимо вывесте</param>
        /// <param name="_color">Цвет сообщения </раrаm>
        public void SetNotification(string Message, SolidColorBrush _color)
        {
            // Для текстового поля указываем текст
            LNameUser.Content = Message;
            // Для текстового поля указываем цвет
            LNameUser.Foreground = _color;
        }


        /// <summary>
        /// Метод открытия страницы восстановления пароля
        /// </summarу>
        private void RecoveryPassword(object sender, MouseButtonEventArgs e) =>
        MainWindow.mainWindow.OpenPage(new Recovery());
        /// <summarу>
        /// Метод открытия страницы регистрации
        /// </summary>
        private void OpenRegin(object sender, MouseButtonEventArgs e) =>
        MainWindow.mainWindow.OpenPage(new Regex());
    }
}
