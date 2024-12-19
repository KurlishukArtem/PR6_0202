using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace RegIN_Kurlishuk.Classes
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public byte[] Image = new byte[0];
        public DateTime DateUpdate { get; set; }
        public DateTime DateCreate { get; set; }
        /// <summary>
        /// событие успешной авторизации
        /// </summary>
        public CorrectLogin HandlerCorrectLogin;
        public InCorrectLogin HandlerInCorrectLogin;

        public delegate void CorrectLogin();
        public delegate void InCorrectLogin();

        public void GetUserLogin(string Login)
        {
            this.Id = -1;
            this.Login = String.Empty;
            this.Password = String.Empty;
            this.Name = String.Empty;
            this.Image = new byte[0];
            MySqlConnection myConnection = WorkingDB.OpenConnection();
            if (WorkingDB.OpenConnection(myConnection))
            {
                MySqlDataReader userQuery = WorkingDB.Query($"SELECT * FROM users WHERE Login = '{Login}'", myConnection);
                if (userQuery.HasRows)
                {
                    userQuery.Read();
                    this.Id = userQuery.GetInt32(0);
                    this.Login = userQuery.GetString(1);
                    this.Name = userQuery.GetString(2);
                    if (!userQuery.IsDBNull(4))
                    {
                        this.Image = new byte[64 * 1024];
                        userQuery.GetBytes(4, 0, Image, 0, Image.Length);
                    }
                    this.DateUpdate = userQuery.GetDateTime(5);
                    this.DateCreate = userQuery.GetDateTime(6);
                    HandlerCorrectLogin.Invoke();
                }
                else
                    // если данные чтения не существуют, вызываем события провальной обработк
                    HandlerInCorrectLogin.Invoke(); //8 разрабока класса User
            }
            else
            {
                //если соединение открыть не удается, вызываем событие провальной авторизации
                HandlerInCorrectLogin.Invoke();
                //закрываем соединение с БД
                WorkingDB.CloseConnection(myConnection);
            }
        }   
        ///<summary>
        /// Функция сохранения польователя
        ///</summary>
        public void SetUser()
        {
            MySqlConnection mySqlConnection = WorkingDB.OpenConnection();
            //проверяем соединение на открыто/закрыто
            if (WorkingDB.OpenConnection(mySqlConnection))
            {
                //запрос на добавление пользователя
                MySqlCommand mySqlCommand = new MySqlCommand("INSERT INTO users (Login, Password, Name, Image, DateUpdate, DateCreate) VALUES (@Login, @Password, @Name, @Image, @DateUpdate, @DateCreate)", mySqlConnection);
                //добавляем параметр логина
                mySqlCommand.Parameters.AddWithValue("@Login", this.Login);
                //добавляем параметр пароля и т.д
                mySqlCommand.Parameters.AddWithValue("@Password", this.Password);
                mySqlCommand.Parameters.AddWithValue("@Name", this.Name);
                mySqlCommand.Parameters.AddWithValue("@Image", this.Image);
                mySqlCommand.Parameters.AddWithValue("@DateUpdate", this.DateUpdate);
                mySqlCommand.Parameters.AddWithValue("@DateCreate", this.DateCreate);
                mySqlCommand.ExecuteNonQuery();
            }
            //закрываем подключение к БД
            WorkingDB.CloseConnection(mySqlConnection);
        }
        /// summary
        /// Функция создания нового пароля
        /// summary
        public void CreateNewPassword()
        {
            //Если наш логин не равен пустому значению
            // А это значит что пользователь существует
            if(Login != String.Empty)
            {
                //Вызываем функцию генерации пароля
                Password = GeneratePass();
                // Открываем подключение к БД
                MySqlConnection mysqlConnection = WorkingDB.OpenConnection(); 
                if (WorkingDB.OpenConnection(mysqlConnection))
                {
                    WorkingDB.Query($"UPDATE users SET Password = '{this.Password}' WHERE Login = '{this.Login}'", mysqlConnection);
                }
                WorkingDB.CloseConnection(mysqlConnection);
                SendMail.SendMessage($"Your accaunt password has been changed \nNew password: {this.Password}", this.Login);
            }
        }
        public string GeneratePass()
        {
            //создаем коллекцию состоящую из символов
            List<char> NewPassword = new List<char>();
            //Инициализация Рандом, которая случайно выбирает числа
            Random rnd = new Random();
            // Символы нумерации
            char[] ArrNumbers = { '0', '1', '3', '4', '5', '6', '7', '8', '9' };
            char[] ArrSymbols = { '|', '-', '_', '!', '@', '#', '$', '%', '&', '*', '=', '+' };
            //символы английской расскладки
            char[] ArrUppercase = { 'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p', 'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'z', 'x', 'c', 'v', 'b', 'n', 'm'};
            //выбираем одну случайную цифру
            for (int i = 0; i < 1; i++)
                NewPassword.Add(ArrNumbers[rnd.Next(0, ArrNumbers.Length)]);
            for (int i = 0; i < 1; i++)
                NewPassword.Add(ArrSymbols[rnd.Next(0, ArrSymbols.Length)]);
            for (int i = 0;i<2 ; i++)
                NewPassword.Add((char.ToUpper(ArrUppercase[rnd.Next(0,ArrUppercase.Length)])));
            //выбираем 6 случайные буквы расскладки в коллекцию
            for (int i = 0; i<6; i++)
                NewPassword.Add(ArrUppercase[rnd.Next(0,ArrUppercase.Length)]);
            //перебираем коллекцию
            //тем самым перемешиваем коллекцию символов
            for (int i = 0; i<NewPassword.Count;i++)
            {
                //выбираем случайный символ
                int RandomSymbol = rnd.Next(0, NewPassword.Count);
                //запоминаем случайный символ
                char Symbol = NewPassword[RandomSymbol];
                //Меняем случайный символ на порядковый символ
                NewPassword[RandomSymbol] = NewPassword[i];
                //Меняем порядковый символ в коллекции на случайные
                NewPassword[i] = Symbol;
            }
            //Объявляем переменную, которая будет содержать пароль
            string NPassword = "";
            //Перебираем коллецию d
            for (int i = 0; i < NewPassword.Count;i++)
                NPassword += NewPassword[i];
            //Возвращаем пароль
            return NPassword;  


        }
    }
}




    

