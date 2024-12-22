using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Mysqlx;
using Slack.Webhooks.Action;
using Imaging = Aspose.Imaging;

namespace RegIN_Kurlishuk.Pages
{
    /// <summary>
    /// Логика взаимодействия для Regin.xaml
    /// </summary>
    public partial class Regin : Page
    {
        /// <summary>
        /// Файловый диалог
        /// </summary>
        OpenFileDialog FileDialogImage = new OpenFileDialog();
        /// <summary>
        /// Проверка на ввод логина
        /// </summarгу>
        bool BCorrectLogin = false;
        /// <summary>
        /// Проверка на ввод пароля
        /// </summary>
        bool BCorrectPassword = false;
        /// <summary>
        /// Проверка на подтверждение пароля
        /// </summary>
        bool BCorrectConfirmPassword = false;
        /// <summary>
        /// Проверка на указания изображения
        /// </summary>
        bool BSetImages = false;

        public Regin()
        {
            InitializeComponent();
            // Подписываемся на правильно введёный логин
            MainWindow.mainWindow.UserLogIn.HandlerCorrectLogin += CorrectLogin;
            // Подписываемся на неправильно введёный логин
            MainWindow.mainWindow.UserLogIn.HandlerInCorrectLogin += InCorrectLogin;
            // Задаём диалоговому окну фотографии типы которые поддерживаем
            FileDialogImage.Filter = "PNG (*.png)|*.png|JPG (*.jpg)|*.jpg";
            // Задаём значения сохранения дирректории
            FileDialogImage.RestoreDirectory = true;
            // Указываем название диалогового окна
            FileDialogImage.Title = "Choose a photo for your avatar";
        }

        /// <summary>
        /// Функция правильно введённого логина
        /// </summarу>
        private void CorrectLogin()
        {
            // Выводим сообщения красным цветом, о том что такой логин уже используется
            SetNotification("Login already in use", Brushes.Red);
            // Отключаем проверку на ввод логина
            BCorrectLogin = false;
        }
        /// <summary>
        /// Функция не правильно введённого логина
        /// </summarгу>
        private void InCorrectLogin() =>
        // Выводим пустое сообщение чёрным цветом
        SetNotification("", Brushes.Black);

        private void SetLogin(object sender, KeyEventArgs e)
        {
            // Если нажата клавиша Enter
            if (e.Key == Key.Enter)
                // Вызываем метод ввода логина
                SetLogin(); 
        }
        /// <summary>
        /// Метод ввода логина
        /// </summarу>
        private void SetLogin(object sender, System.Windows.RoutedEventArgs e) =>
            // Вызываем метод ввода логина
            SetLogin();
        /// <summary>
        /// Метод ввода логина
        /// </summary>
        public void SetLogin()
        {
            // Регулярное выражение для почты
            Regex regex = new Regex(@"([a-zA-Z0-9._-]{4,}@[a-zA-Z0-9._-]{2,}\.[a-zA-Z0-9_-]{2,})");
            // Ввелён ли логин зависит от того регулярного выражения
            BCorrectLogin = regex.IsMatch(TbLogin.Text);
            // Если регулярное вырожение
            if (regex.IsMatch(TbLogin.Text) == true)
            {
                // Выводим пустое уведомление чёрным цветом
                SetNotification("", Brushes.Black);
                // Вызываем получение данных пользователя по логину
               MainWindow.mainWindow.UserLogIn.GetUserLogin(TbLogin.Text);
            }
            else
                // Если введён логин не усдовлетворяющий регулярное вырожение, выводим сообщение красным цветом
                SetNotification("Invalid login", Brushes.Red);
            // Вызываем метод авторизации
            OnRegin();
        }


        #region SetPassword
        /// <summary>
        /// Метод ввода пароля
        /// </summarу>
        private void SetPassword(object sender, System.Windows.RoutedEventArgs e) =>
        // Вызываем метод ввода пароля
        SetPassword();
        /// <summary>
        /// Метод ввода пароля
        /// </summarгу>
        private void SetPassword(object sender, KeyEventArgs e)
        {
            // Если нажата клавиша Enter
            if (e.Key == Key.Enter)
                // Вызываем метод ввода пароля
                SetPassword();
        }
        /// <summary>
        /// Метод ввода пароля
        /// </summary>
        public void SetPassword()
        {
            // Регулярное выражение
            Regex regex = new Regex(@"(?=.*[0-9])(?=.*[!@#$%^&?*\-_=])(?=.*[a-z])(?=.*[A-Z])[0-9a-zA-Z!@#$%^&?*\-_=]{10,}");
            // (?=.*[0 - 9]) строка содержит хотя бы одно число;
            //(?=.*[!@#$%^&?*\-_=]) - строка содержит хотя бы один спецсимвол;
            //(?=.*[a - z]) строка содержит хотя бы одну латинскую букву в нижнем регистре;
            //(?=.*[A - z]) - строка содержит хотя бы одну латинскую букву в верхнем регистре;
            //[0-9a-zA-Z!@#$%^&?*\-_=]{10,} - строка состоит не менее, чем из 10 вышеупомянутых символов.
            // Введён ли пароль зависит от резултата регулярного выражения
            BCorrectPassword = regex.IsMatch(TbPassword.Password);
            // Если введный пароль удовлетворяет регулярное выражение
            if (regex.IsMatch(TbPassword.Password) == true)
            {
                // Выводим пустое сообщение с чёрным цветом
                SetNotification("", Brushes.Black);
                // Если пароль уже введён для повтарения
                if (TbConfirmPassword.Password.Length > 0)
                    // Вызываем проверку повторения пароля
                    ConfirmPassword(true);
                // Вызываем функцию регистрации
                OnRegin();
            }
            else
                // Если введёный пароль не удовлетворяет регулярное выражение
                // Выводим сообщение с ошибкой красным цветом
                SetNotification("Invalid password", Brushes.Red);
        }
        #endregion

        #region SetConfirm Password
        /// <summary>
        /// Метод повторного ввода пароля
        /// </summarгу>
        private void ConfirmPassword(object sender, KeyEventArgs e)
        {
            // Если нажата клавиша Enter
            if (e.Key == Key.Enter)
                // Вызываем метод повторения пароля
                ConfirmPassword();
        }
        /// <summary>
        /// Метод повторного ввода пароля
        /// </summarу>
        private void ConfirmPassword(object sender, System.Windows.RoutedEventArgs e) =>
            // Вызываем метод повторения пароля
            ConfirmPassword();
        /// <summary>
        /// Метод повторного ввода пароля
        /// </summarгу>
        public void ConfirmPassword(bool Pass = false)
        {
            // Записываем рещультат сравнения паролей в переменную
            BCorrectConfirmPassword = TbConfirmPassword.Password == TbPassword.Password;
            // Если пароль не совпадает с повторением пароля
            if (TbConfirmPassword.Password != TbPassword.Password)
                // Выводим сообщение о том, что пароли не совпадают, красным цветом
                SetNotification("Passwords do not match", Brushes.Red);
            else
            {
                // Если пароли совпадают, выводим пустое сообщение чёрным цветом
                SetNotification("", Brushes.Black);
                // Если проверка идёт не из метода проверки пароля
                // Исключаем зацикливание методов
                if (!Pass)
                    // Вызываем проверку пароля
                    SetPassword();
            }
        }
        #endregion

        public void SetNotification(string Message, SolidColorBrush _color)
        {
            // Для текстового поля указываем текст
            LNameUser.Content = Message;
            // Для текстового поля указываем цвет
            LNameUser.Foreground = _color;
        }
        /// <summary>
        /// Метод регистрации
        /// </summary>
        void OnRegin()
        {
            // Если логин не введён
            if (!BCorrectLogin)
                // Останавливаем регистрацию
                return;
            // Если наименование не введено
            if (TbName.Text.Length == 0)
                // Останавливаем регистрацию
                return;
            // Если не введён пароль
            if (!BCorrectPassword)
                // Останавливаем регистрацию
                return;
            // Если пароль не подтверждён
            if (!BCorrectConfirmPassword)
                // Останавливаем регистрацию
                return;
            // Указываем пользовательский логин
            MainWindow.mainWindow.UserLogIn.Login = TbLogin.Text;
            // Указываем пользовательский пароль
            MainWindow.mainWindow.UserLogIn.Password = TbPassword.Password;
            // Указываем пользовательское имя
            
            if (BSetImages)
                MainWindow.mainWindow.UserLogIn.Name = TbName.Text;
            
        // Разбиваем изображение на байты
        MainWindow.mainWindow.UserLogIn.Image = File.ReadAllBytes(Directory.GetCurrentDirectory() + @"\IUser.jpg");
            // Указываем дату обновления
            MainWindow.mainWindow.UserLogIn.DateUpdate = DateTime.Now;
            // Указываем дату создания
            MainWindow.mainWindow.UserLogIn.DateCreate = DateTime.Now;
            // Открываем страницу подтверждения через почту
            MainWindow.mainWindow.OpenPage(new Confirmation(Confirmation.TypeConfirmation.Regin));
        }
        /// <summary>
        /// Ввод букв
        /// </summary>
        private void SetName(object sender, TextCompositionEventArgs e)
        {
            // Проверяем что символ относится к категории букв
            e.Handled = !(Char.IsLetter(e.Text, 0));
        }

        /// <summary>
        /// Функция выбора изображения пользователя
        /// </summarу>
        private void SelectImage(object sender, MouseButtonEventArgs e)
        {
            // Если статус открывшегося диалогового окна true
            if (FileDialogImage.ShowDialog() == true)
            {
                // создаём ширину изображения
                int NewWidth = 0;
                // Создаём высоту изображения
                int NewHeight = 0;
                // конвертируем размер изображения
                using (Imaging.Image image = Imaging.Image.Load(FileDialogImage.FileName))
                {
                    // проверяем какая из сторон больше
                    if (image.Width > image.Height)
                    {
                        // Расчитываем новую ширину относительно высоты
                        NewWidth = (int)(image.Width * (256f / image.Height));
                        // Задаём высоту изображения
                        NewHeight = 256;
                    }
                    else
                    {
                        // Задаём ширину изображения
                        NewWidth = 256;
                        // Расчитываем новую высоту относительно высоты
                        NewHeight = (int)(image.Height * (256f / image.Width));
                    }
                    // Изменяем изобружение
                    image.Resize(NewWidth, NewHeight);
                    // Сохраняем изображение
                    image.Save("IUser.jpg");
                }
                // обрезаем изображение
                using (Imaging.RasterImage rasterImage = (Imaging.RasterImage)Imaging.Image.Load("IUser.jpg"))
                {
                    // Перед кадрированием изображение следует кэшировать для лучшей производительности.
                    if (!rasterImage.IsCached)
                    {
                        rasterImage.CacheData();
                    }
                    // задаём х
                    int X = 0;
                    // Задаём ширину изображения
                    int Width = 256;
                    // Задаём Y
                    int Y = 0;
                    // Задаём высоту изображения
                    int Height = 256;
                    // Если ширина изображения больше чем высота
                    if (rasterImage.Width > rasterImage.Height)
                            // Расчитываем Х как середину изображения
                            X = (int)((rasterImage.Width - 256f) / 2);
                            else
                                Y = (int)((rasterImage.Height - 256f) / 2);
                    // Создайте экземпляр класса Rectangle нужного размера и обрежьте изображение.
                    Imaging.Rectangle rectangle = new Imaging.Rectangle(X, Y, Width, Height);
                    rasterImage.Crop(rectangle);
                    // Сохраните обрезанное изображение.
                    rasterImage.Save("IUser.jpg");
                }
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
                    IUser.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @"\IUser.jpg"));
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
                // Запоминаем что изображение указано
                BSetImages = true;
            }
            else
                // Запоминаем что изображение не указано
                BSetImages = false;
        }
        /// <summary>
        /// Метод перехода на авторизацию
        /// </summary>
        private void OpenLogin(object sender, MouseButtonEventArgs e)
        {
            MainWindow.mainWindow.OpenPage(new Login());
        }
    }
}

