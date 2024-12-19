using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace RegIN_Kurlishuk.Elements
{
    /// <summary>
    /// Логика взаимодействия для ElementCapture.xaml
    /// </summary>
    public partial class ElementCapture : UserControl
    {
        public CorrectCapture HandlerCorrectCapture;
        public delegate void CorrectCapture();
        string strCapture = "";
        int ElementWidth = 280;
        int ElementHeight = 50;

        public ElementCapture()
        {
            InitializeComponent();
            CreateCapture();
        }

        /// <summary>
        /// Функция создания капчи
        /// </summary>
        public void CreateCapture()
        {
            //очищаем пользовательские элементы
            InputCapture.Text = "";
            Capture.Children.Clear();
            strCapture = "";
            //Вызывааем функцию создания заднего фона капчи
            CreateBackground();
            //Вызывааем функцию создания переднего фона капчи
            Background();
        }
        #region CreateCapture
        /// <summary>
        /// Функция создания заднего фона капчи
        /// </summary>
        void CreateBackground()
        {
            //инициализируем рандом
            Random ThisRandom = new Random();
            //Запускаем цикл от 0 до 100
            for (int i = 0; i < 100; i++)
            {
                int back = ThisRandom.Next(0, 10);
                //Инициализируем нвоый элемент типа Label
                Label LBackground = new Label()
                {
                    Content = back,
                    FontSize = ThisRandom.Next(10, 16),
                    FontWeight = FontWeights.Bold,
                    //указываем цвет шрифта (случайный)
                    Foreground = new SolidColorBrush(Color.FromArgb(100, (byte)ThisRandom.Next(0,255), 
                    (byte)ThisRandom.Next(0,255), (byte)ThisRandom.Next(0, 255))),
                    //Задаем отступы элемента (случайные)
                    Margin = new Thickness(ThisRandom.Next(0, ElementWidth - 20), ThisRandom.Next(0, ElementHeight - 20), 0, 0)
                };
                // добавляем новый элемент в Grid на сцене 
                Capture.Children.Add(LBackground);
            }
        }
        /// <summary>
        /// Функция создания переднего плана капчи
        /// </summary>
        void Background()
        {
            //инициализируем рандом
            Random ThisRandom = new Random();
            //Запускаем цикл от 0 до 100
            for (int i = 0; i < 100; i++)
            {
                int back = ThisRandom.Next(0, 10);
                //Инициализируем нвоый элемент типа Label
                Label LCode = new Label()
                {
                    Content = back,
                    FontSize = 30,
                    FontWeight = FontWeights.Bold,
                    //указываем цвет шрифта (случайный)
                    Foreground = new SolidColorBrush(Color.FromArgb(100, (byte)ThisRandom.Next(0, 255),
                    (byte)ThisRandom.Next(0, 255), (byte)ThisRandom.Next(0, 255))),
                    //Задаем отступы элемента (случайные)
                    Margin = new Thickness(ElementWidth/2 - 60 + i*30, ThisRandom.Next(-10, 10), 0, 0)
                };
                //Записываем цифру в текстовое значение капчи
                strCapture += back.ToString();
                // добавляем новый элемент в Grid на сцене 
                Capture.Children.Add(LCode);
            }
        }
        #endregion
        /// <summary>
        /// Функция проверки капчи
        /// </summary>
        /// <returns></returns>
        public bool OnCapture()
        {
            // Если значения равны : True
            // Иначе : False
            return strCapture == InputCapture.Text;
        }

        private void EnterCapture(object sender, KeyEventArgs e)
        {
            if (InputCapture.Text.Length == 4)
                // Если проверка на капчу не проходит
                if (!OnCapture())
                    CreateCapture();
                else if (HandlerCorrectCapture != null)
                    HandlerCorrectCapture.Invoke();
        }
    }
}
