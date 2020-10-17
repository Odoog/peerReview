using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Media;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Net;

namespace apple_splution
{

    class Talk
    {

        //Класс отвечающий за ввод и вывод в консоль.
        //Содержит приведения типов, разбиение строки разделителем, и форматированный вывод.

        private static Dictionary<string, string> formatKeys;
        private static Dictionary<string, ConsoleColor> colors;

        public static void Initialize()
        {
            //Метод инициализации внутренних полей.

            colors = new Dictionary<string, ConsoleColor>();


            colors.Add("black", ConsoleColor.Black);
            colors.Add("blue", ConsoleColor.Blue);
            colors.Add("cyan", ConsoleColor.Cyan);
            colors.Add("dblue", ConsoleColor.DarkBlue);
            colors.Add("dcyan", ConsoleColor.DarkCyan);
            colors.Add("dgray", ConsoleColor.DarkGray);
            colors.Add("dgreen", ConsoleColor.DarkGreen);
            colors.Add("dmagenta", ConsoleColor.DarkMagenta);
            colors.Add("dred", ConsoleColor.DarkRed);
            colors.Add("dyellow", ConsoleColor.DarkYellow);
            colors.Add("gray", ConsoleColor.Gray);
            colors.Add("green", ConsoleColor.Green);
            colors.Add("magenta", ConsoleColor.Magenta);
            colors.Add("red", ConsoleColor.Red);
            colors.Add("white", ConsoleColor.White);
            colors.Add("yellow", ConsoleColor.Yellow);


        }
        public static Tuple<int, string> GetStringColorEntry(string haystack)
        {
            //Метод поиска в строке подстроки формата "colorName<".
            //
            //Возвращает Turple<int, string> где .
            //          {0} - Индекс начала подстроки или -1 в случае отсутствия.
            //          {1} - Найденная строка.
            //haystack: Строка для поиска | string.

            int minFoundPosition = int.MaxValue;
            string minFoundString = "";

            foreach (var color in colors)
            {
                if (haystack.IndexOf(color.Key + "<") != -1) //Если подстрока найдена.
                {
                    if (haystack.IndexOf(color.Key + "<") < minFoundPosition) //Если у найденной подстроки минимальный индекс среди других найденных подстрок.
                    {
                        minFoundPosition = haystack.IndexOf(color.Key + "<");
                        minFoundString = color.Key;
                    }
                }
            }

            if (minFoundPosition != int.MaxValue)
            {
                return new Tuple<int, string>(minFoundPosition, minFoundString);
            }
            else
            {
                //Строка не найдена.
                return new Tuple<int, string>(-1, "");
            }
        }
        public static void AddFormatKey(string formatKey, string value)
        {
            //Метод добавления ключа в словарь фраз форматированного вывода.
            //
            //formatKey: Ключ словаря | string.
            //value: Фраза вывода по ключу | string.

            if (formatKeys == null) formatKeys = new Dictionary<string, string>();

            if (formatKeys.ContainsKey(formatKey)) //Если такой ключ уже существует - заменить значение.
            {
                formatKeys[formatKey] = value;
            }
            else //Иначе добавить в словарь.
            {
                formatKeys.Add(formatKey, value);
            }


        }

        public static string Get(ConsoleColor color = ConsoleColor.White)
        {
            //Метод чтения из консоли.
            //Возвращает одну строку.
            //color: Цвет для ввода пользователя | ConsoleColor.

            Console.ForegroundColor = color;

            return Console.ReadLine();
        }

        public static KeyValuePair<bool, dynamic> Get(string type, string customInput = "", string secondaryType = "", char separator = default(char), ConsoleColor color = ConsoleColor.White)
        {
            //Метод чтения из консоли с указанным типом.
            //Возвращает пару true + значение указанного типа если преобразование успешно.
            //Возвращает пару false + строка с ошибкой преобразования если преобразование не успешно.
            //type: Тип переменной | string {"int", "double", "uint", "array"}.
            //custom input: Строка для разбора (Переобределяет Console.ReadLine) | string.
            //secondaryType: Тип переменных в массиве в случае если type = "string" | string {"int", "double", "string", "uint"}.
            //separator: Разделитель строки для формирования массива | char.
            //color: Цвет для ввода пользователя | ConsoleColor.

            try
            {
                Console.ForegroundColor = color;

                string input = customInput == "" ? Console.ReadLine() : customInput;

                switch (type)
                {
                    case "int":
                        {
                            int value = 0;
                            if (!int.TryParse(input, out value))
                            {
                                return new KeyValuePair<bool, dynamic>(false, input);
                            }
                            return new KeyValuePair<bool, dynamic>(true, value);
                        }
                    case "double":
                        {
                            double value = 0;
                            if (!double.TryParse(input, out value))
                            {
                                return new KeyValuePair<bool, dynamic>(false, input);
                            }
                            return new KeyValuePair<bool, dynamic>(true, value);
                        }
                    case "uint":
                        {
                            uint value = 0;
                            if (!uint.TryParse(input, out value))
                            {
                                return new KeyValuePair<bool, dynamic>(false, input);
                            }
                            return new KeyValuePair<bool, dynamic>(true, value);
                        }
                    case "char":
                        {
                            return new KeyValuePair<bool, dynamic>(true, input[0]);
                        }
                    case "string":
                        {
                            return new KeyValuePair<bool, dynamic>(true, input);
                        }
                    case "array":
                        {

                            List<string> inputList = new List<string>();

                            if (separator == default(char))
                            {
                                //Если разделителя нет - делим строку на массив символов.
                                string temp = string.Join(";", input.ToCharArray());
                                inputList = temp.Split(';').ToList();
                            }
                            else
                            {
                                //Если разделитель есть - делим строку разделителем.
                                inputList = input.Split(separator).ToList();
                            }

                            dynamic resultList = new List<dynamic>();

                            //Приводим итоговый массив к типу secondaryType.
                            switch (secondaryType)
                            {
                                case "int":
                                    resultList = new List<int>();
                                    break;
                                case "double":
                                    resultList = new List<double>();
                                    break;
                                case "string":
                                    resultList = new List<string>();
                                    break;
                                case "uint":
                                    resultList = new List<string>();
                                    break;

                            }

                            //Вызываем для каждого элемена массива string приведение его к типу secondary type.
                            foreach (var el in inputList)
                            {
                                if (Get(secondaryType, el).Key)
                                {
                                    resultList.Add(Get(secondaryType, el).Value);
                                }
                                else
                                {
                                    return new KeyValuePair<bool, dynamic>(false, Get(secondaryType, el).Value);
                                }
                            }

                            return new KeyValuePair<bool, dynamic>(true, resultList);
                        }
                }
            }
            catch (Exception e) //Если ошибка (Которая явно не отлавливается внутри try) вернуть пару false,false.
            {
                return new KeyValuePair<bool, dynamic>(false, false);
            }

            return new KeyValuePair<bool, dynamic>(false, false);
        }

        private static void tryFormatOut(string formatString, dynamic value)
        {
            //Метод печати строки с подстановкой значения.
            //Выводит строку с подстановкой {0} <= value если в строке содержится {0}.
            //Иначе выводит строку без подстановки.
            //
            //formatString: Строка форматированного вывода | string.
            //value: значение для подстановки | dynamic.

            if (formatString.IndexOf("{0}") != -1) //Если в строке есть {0}.
            {
                Console.Write(formatString, value);
            }
            //Если в строке нет {0}.
            else
            {
                Console.Write(formatString);
            }
        }
        private static void tryFormatOut(string formatString, dynamic value1, dynamic value2)
        {
            //Метод печати строки с подстановкой значений.
            //Выводит строку с подстановкой {0} <= value1 {1} <= value2 если в строке содержится {0} и {1}.
            //Выводит строку с подстановкой {0} <= value1 если в строке содержится только {0}.
            //Выводит строку с подстановкой {1} <= value2 если в строке содержится только {1}.
            //Иначе выводит строку без подстановки.
            //
            //formatString: Строка форматированного вывода | string.
            //value1: значение для подстановки | dynamic.
            //value2: значение для подстановки | dynamic.

            if (formatString.IndexOf("{0}") != -1 && formatString.IndexOf("{1}") != -1) //Если есть оба значения.
            {
                Console.Write(formatString, value1, value2);
            }
            else
            {
                if (formatString.IndexOf("{0}") != -1) //Если есть только {0}.
                {
                    Console.Write(formatString, value1);
                }
                else
                {
                    if (formatString.IndexOf("{1}") != -1) //Если есть только {1}.
                    {
                        Console.Write(formatString, value1, value2);
                    }
                    else
                    //Если в строке нет {0} и нет {1}.
                    {
                        Console.Write(formatString);
                    }
                }
            }
        }
        public static void Out(dynamic[] value, char separator = ' ', ConsoleColor color = ConsoleColor.White, string end = "\n")
        {
            //Метод вывода в консоль массива.
            //Выводит в строку все элементы массива через разделитель.
            //
            //value: Массив для вывода | dynamic[].
            //separator: Разделитель для вывода | char.
            //color: Цвет вывода | ConsoleColor .
            //end: Завершение строки (\n по умолчанию) | string.

            Console.ForegroundColor = color;

            foreach (var el in value)
            {
                Console.Write(el);
                Console.Write(separator);
            }
            Console.Write(end);

            Console.ResetColor();
        }

        public static void Out(int[] value, char separator = ' ', ConsoleColor color = ConsoleColor.White, string end = "\n")
        {
            //Метод вывода в консоль массива > перегрузка для int.
            //Выводит в строку все элементы массива через разделитель.
            //
            //value: Массив для вывода | int[].
            //separator: Разделитель для вывода | char.
            //color: Цвет вывода | ConsoleColor .
            //end: Завершение строки (\n по умолчанию) | string.

            Console.ForegroundColor = color;

            foreach (var el in value)
            {
                Console.Write(el);
                Console.Write(separator);
            }
            Console.Write(end);

            Console.ResetColor();
        }

        public static void Out(double[] value, char separator = ' ', ConsoleColor color = ConsoleColor.White, string end = "\n")
        {
            //Метод вывода в консоль массива > перегрузка для double.
            //Выводит в строку все элементы массива через разделитель.
            //
            //value: Массив для вывода | double[].
            //separator: Разделитель для вывода | char.
            //color: Цвет вывода | ConsoleColor .
            //end: Завершение строки (\n по умолчанию) | string.

            Console.ForegroundColor = color;

            foreach (var el in value)
            {
                Console.Write(el);
                Console.Write(separator);
            }
            Console.Write(end);

            Console.ResetColor();
        }

        public static void Out(char[] value, char separator = ' ', ConsoleColor color = ConsoleColor.White, string end = "\n")
        {
            //Метод вывода в консоль массива > перегрузка для char.
            //Выводит в строку все элементы массива через разделитель.
            //
            //value: Массив для вывода | char[].
            //separator: Разделитель для вывода | char.
            //color: Цвет вывода | ConsoleColor .
            //end: Завершение строки (\n по умолчанию) | string.

            Console.ForegroundColor = color;

            foreach (var el in value)
            {
                Console.Write(el);
                Console.Write(separator);
            }
            Console.Write(end);

            Console.ResetColor();
        }

        public static void Out(dynamic value, ConsoleColor color = ConsoleColor.White, string end = "\n")
        {
            //Метод вывода в консоль элемента.
            //Выводит в строку элемент.
            //
            //value: Элемент для вывода | dynamic.
            //color: Цвет вывода | ConsoleColor .
            //end: Завершение строки (\n по умолчанию) | string.

            Console.ForegroundColor = color;

            Console.Write(value);
            Console.Write(end);

            Console.ResetColor();
        }

        public static void Out(string formatKey, dynamic value, ConsoleColor color = ConsoleColor.White, string end = "\n")
        {
            //Метод вывода в консоль форматированной строки.
            //Выводит в строку элемены 1,2 в соответствии с форматом formatKeys[formatKey].
            //
            //formatKey: Ключ строки формата из formatKeys | string.
            //value: Элемент для вывода | dynamic.
            //color: Цвет вывода | ConsoleColor .
            //                       !! Переопределяет настройки цвета formatKey !!.
            //end: Завершение строки (\n по умолчанию) | string.

            ConsoleColor defaultColor = color;
            string stringToPrint = formatKeys[formatKey]; //Начало цветной части строки.
            int startColorizedPrintPosition = 0; //Начало цветной части строки.
            int endColorizedPrintPosition = 0; //Конец цветной части строки.

            if (formatKey.Contains("Error") || formatKey.Contains("Error")) //Если в ключе форматированного вывода есть 'Error' выводим красным цветом.
            {
                defaultColor = ConsoleColor.Red;
            }

            if (formatKey.Contains("Win") || formatKey.Contains("win")) //Если в ключе форматированного вывода есть 'Win' выводим зеленым цветом.
            {
                defaultColor = ConsoleColor.Green;
            }

            if (formatKey.Contains("Warning") || formatKey.Contains("warning")) //Если в ключе форматированного вывода есть 'Warning' выводим желтым цветом.
            {
                defaultColor = ConsoleColor.Yellow;
            }

            defaultColor = color == ConsoleColor.White ? defaultColor : color; //Если задано переопределение цвета вывода - установить его.

            while (GetStringColorEntry(stringToPrint).Item1 != -1) //Пока в строке найдены цветные части.
            {
                endColorizedPrintPosition = stringToPrint.IndexOf(">");

                Console.ForegroundColor = defaultColor; //Устанавливаем цвет вывода как переданный параметр color или в соответствии с ключом формата.

                Console.Write(stringToPrint.Substring(0, GetStringColorEntry(stringToPrint).Item1)); //Выводим всё до цветной части.

                Console.ForegroundColor = colors[GetStringColorEntry(stringToPrint).Item2]; //Устанавливаем необходимый цвет.

                startColorizedPrintPosition = //Начало цветной части равняется.
                    GetStringColorEntry(stringToPrint).Item1 //Началом цветной части.
                    + GetStringColorEntry(stringToPrint).Item2.Length //Плюс длиной определения цвета части.
                    + 1; //Плюс символ <.

                Talk.tryFormatOut( //Вывести цветную подстроку.
                        stringToPrint.Substring(
                                startColorizedPrintPosition,
                                endColorizedPrintPosition - startColorizedPrintPosition
                            ),
                        value
                    );

                stringToPrint = stringToPrint.Length > endColorizedPrintPosition + 1 ?
                     stringToPrint.Substring(endColorizedPrintPosition + 1) : ""; //Сокращаем выводимую строку на уже выведенную часть;.

            }

            Console.ForegroundColor = defaultColor; //Устанавливаем цвет вывода как переданный параметр color или в соответствии с ключом формата.

            Talk.tryFormatOut(stringToPrint, value);
            Console.Write(end);

            Console.ResetColor(); //Возвращаем default настройку цветов консоли.
        }

        public static void Out(string formatKey, dynamic value1, dynamic value2, ConsoleColor color = ConsoleColor.White, string end = "\n")
        {
            //Метод вывода в консоль форматированной строки.
            //Выводит в строку элемены 1,2 в соответствии с форматом formatKeys[formatKey].
            //
            //formatKey: Ключ строки формата из formatKeys | string.
            //value1: Элемент для вывода | dynamic.
            //value2: Элемент для вывода | dynamic.
            //color: Цвет вывода | ConsoleColor .
            //                       !! Переопределяет настройки цвета formatKey !!.
            //end: Завершение строки (\n по умолчанию) | string.

            ConsoleColor defaultColor = color;
            string stringToPrint = formatKeys[formatKey]; //Начало цветной части строки.
            int startColorizedPrintPosition = 0; //Начало цветной части строки.
            int endColorizedPrintPosition = 0; //Конец цветной части строки.

            if (formatKey.Contains("Error") || formatKey.Contains("Error")) //Если в ключе форматированного вывода есть 'Error' выводим красным цветом.
            {
                defaultColor = ConsoleColor.Red;
            }

            if (formatKey.Contains("Win") || formatKey.Contains("win")) //Если в ключе форматированного вывода есть 'Win' выводим зеленым цветом.
            {
                defaultColor = ConsoleColor.Green;
            }

            if (formatKey.Contains("Warning") || formatKey.Contains("warning")) //Если в ключе форматированного вывода есть 'Warning' выводим желтым цветом.
            {
                defaultColor = ConsoleColor.Yellow;
            }

            defaultColor = color == ConsoleColor.White ? defaultColor : color; //Если задано переопределение цвета вывода - установить его.

            while (GetStringColorEntry(stringToPrint).Item1 != -1) //Пока в строке найдены цветные части.
            {
                endColorizedPrintPosition = stringToPrint.IndexOf(">");

                Console.ForegroundColor = defaultColor; //Устанавливаем цвет вывода как переданный параметр color или в соответствии с ключом формата.

                Console.Write(stringToPrint.Substring(0, GetStringColorEntry(stringToPrint).Item1)); //Выводим всё до цветной части.

                Console.ForegroundColor = colors[GetStringColorEntry(stringToPrint).Item2]; //Устанавливаем необходимый цвет.

                startColorizedPrintPosition = //Начало цветной части равняется.
                    GetStringColorEntry(stringToPrint).Item1 //Началом цветной части.
                    + GetStringColorEntry(stringToPrint).Item2.Length //Плюс длиной определения цвета части.
                    + 1; //Плюс символ [.

                Talk.tryFormatOut( //Вывести цветную подстроку.
                        stringToPrint.Substring(
                                startColorizedPrintPosition,
                                endColorizedPrintPosition - startColorizedPrintPosition
                            ),
                        value1, value2
                    );

                stringToPrint = stringToPrint.Length > endColorizedPrintPosition + 1 ?
                     stringToPrint.Substring(endColorizedPrintPosition + 1) : ""; //Сокращаем выводимую строку на уже выведенную часть;.

            }

            Console.ForegroundColor = defaultColor; //Устанавливаем цвет вывода как переданный параметр color или в соответствии с ключом формата.

            Talk.tryFormatOut(stringToPrint, value1, value2);
            Console.Write(end);

            Console.ResetColor(); //Возвращаем default настройку цветов консоли.
        }

    }

    class Manager
    {

        public static bool isLinuxBased;

        /// <summary>.
        /// Make path to file in window looks like linux path.
        /// </summary>.
        /// <param name="path">Input path</param>.
        /// <returns>New path</returns>.
        static public string MakePathLikeLinux(string path)
        {
            return path.Replace("\\", "/"); //Заменяем обратные слеши на слеши.
        }

        /// <summary>.
        /// Add slash to disc name to normal work like path.
        /// </summary>.
        /// <param name="path">Input path</param>.
        static public void MakePurePathToDisc(ref string path)
        {
            if (path.EndsWith(":")) //Если заканчивается на двоеточие - значит путь не полный, дополняем его слешем.
            {
                if (isLinuxBased)
                {
                    path += "/";
                }
                else
                {
                    path += "\\";
                }
            }
        }

        /// <summary>.
        /// Return all files in specific folder.
        /// </summary>.
        /// <param name="folderName">Folder to get files from</param>.
        /// <returns>String contains all files in folder</returns>.
        static public string GetFilesInFolder(string folderName)
        {
            string answer = "";

            List<string> fileNames = Directory.GetFiles(folderName).Select(el => Path.GetFileName(el)).ToList(); //Список файлов.
            List<string> dirNames = Directory.GetDirectories(folderName).Select(el => "#" + Path.GetFileName(el)).ToList(); //Список папок.

            answer += String.Join("\n", fileNames); //Добавляем в ответ названия файлов.
            if (fileNames.Count > 0 && dirNames.Count > 0) //Если в папке есть и файлы и папки, разделяем их \n.
            {
                answer += "\n";
            }
            answer += String.Join("\n", dirNames); //Добавляем в ответ названия папок.

            return answer;
        }

        /// <summary>.
        /// Return all available drives in system.
        /// </summary>.
        /// <returns>String contains information about drives</returns>.
        static public string GetDrives()
        {

            int currentDriveNum = 1;

            string answer =
                "#".PadRight(3) +
                "Name:".PadRight(7) +
                "Free memory:".PadRight(14) +
                "Space used:".PadRight(13) +
                "Space remaining:".PadRight(18) +
                "Type:".PadRight(5) + "\n";

            foreach (var drive in DriveInfo.GetDrives()) //Выводим информацию о диске.
            {
                double freeSpace = drive.TotalFreeSpace;
                double allSpace = drive.TotalSize;

                answer += (currentDriveNum++).ToString().PadRight(3);
                answer += drive.Name.PadRight(7);
                answer += (((int)(freeSpace / allSpace * 100)).ToString() + "%").PadRight(14);
                answer += (((int)(drive.TotalSize / Math.Pow(2, 30))).ToString() + "gb").PadRight(13);
                answer += (((int)(drive.AvailableFreeSpace / Math.Pow(2, 30))).ToString() + "gb").PadRight(18);
                answer += drive.DriveType.ToString().PadRight(5);

                answer += "\n";
            }
            return answer;
        }

        /// <summary>.
        /// Change the current drive.
        /// </summary>.
        /// <param name="currentFolder">Path to current folder</param>.
        /// <param name="num">Number of drive to change to</param>.
        /// <returns>True if changing accepted and False otherwise</returns>.
        static public bool ChangeDrive(ref string currentFolder, string num)
        {
            try //Пытаемся сменить диск.
            {
                int numInt = Int32.Parse(num);

                currentFolder = DriveInfo.GetDrives()[numInt - 1].Name;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>.
        /// Move file to specific folder.
        /// </summary>.
        /// <param name="currentFolder">Path to current folder</param>.
        /// <param name="fileName">Name of file to move</param>.
        /// <param name="pathToMove">Path of folder to move file to</param>.
        /// <returns>True if changing drive acceptef and False otherwise</returns>.
        static public bool MoveFile(string currentFolder, string fileName, string pathToMove)
        {
            string pathToFile = Path.Combine(currentFolder, fileName);

            if (!File.Exists(pathToFile))
            { //Если файла не существует - выходим.
                return false;
            }

            if (!Manager.ChangeFolder(ref currentFolder, pathToMove)) //Получаем целевую директорию.
            {
                return false;
            }
            else
            {
                File.Move(pathToFile, Path.Combine(currentFolder, fileName)); //Перемещаем файл.
                return true;
            }
        }

        /// <summary>.
        /// Copy file to specific folder.
        /// </summary>.
        /// <param name="currentFolder">Path to current folder</param>.
        /// <param name="fileName">Name of file to copy</param>.
        /// <param name="pathToMove">Path of folder to copy file to</param>.
        /// <returns>True if copying accepted and False otherwise</returns>.
        static public bool CopyFile(string currentFolder, string fileName, string pathToMove)
        {
            string pathToFile = Path.Combine(currentFolder, fileName);

            if (!File.Exists(pathToFile)) //Если файла не существует - выходим.
            {
                return false;
            }

            if (!Manager.ChangeFolder(ref currentFolder, pathToMove)) //Получаем целевую директорию.
            {
                return false;
            }
            else
            {
                File.Copy(pathToFile, Path.Combine(currentFolder, fileName)); //Копируем файл.
                return true;
            }
        }

        /// <summary>.
        /// Concatinate text files and return the summary of them.
        /// </summary>.
        /// <param name="currentFolder">Path to current folder</param>.
        /// <param name="param">All parameters in request (containing names of files from index 1)</param>.
        /// <returns>Summary text of all files</returns>.
        static public string Concat(string currentFolder, List<string> param)
        {
            List<string> pathToFiles = new List<string>();

            for (int i = 1; i < param.Count; i++)
            {
                pathToFiles.Add(Path.Combine(currentFolder, param[i])); //Путь к файлу - текущая папка + его имя.
            }

            string result = "";

            foreach (string path in pathToFiles)
            {

                using (StreamReader sw = new StreamReader(File.Open(path, FileMode.Open))) //Обьединяем содержимое файлов.
                {
                    result += sw.ReadToEnd();
                }
            }

            return result;
        }

        /// <summary>.
        /// Removing file from current folder.
        /// </summary>.
        /// <param name="currentFolder">Path to current folder</param>.
        /// <param name="fileName">Name of file to remove</param>.
        /// <returns>True if deleting accepted and False otherwise</returns>.
        static public bool RemoveFile(string currentFolder, string fileName)
        {
            string pathToFile = Path.Combine(currentFolder, fileName);

            if (!File.Exists(pathToFile)) //Если файла не существует - выходим.
            {
                return false;
            }
            else
            {
                File.Delete(pathToFile); //Удаляем файл.
                return true;
            }
        }

        static public bool RenameFile(string currentFolder, string fileName, string newFileName)
        {
            string pathToFile = Path.Combine(currentFolder, fileName);

            if (!File.Exists(pathToFile)) //Если файла не существует - выходим.
            {
                return false;
            }

            File.Move(pathToFile, Path.Combine(currentFolder, newFileName)); //Перемещаем файл.
            return true;
        }

        /// <summary>.
        /// Create new file with text in specific encoding.
        /// </summary>.
        /// <param name="currentFolder">Path to current folder</param>.
        /// <param name="fileName">Name of file to create</param>.
        /// <param name="text">Text to write in file</param>.
        /// <param name="fileEncoding">Encoding to write with</param>.
        /// <returns>True if creating accepted and False otherwise</returns>.
        static public bool CreateFile(string currentFolder, string fileName, string text, Encoding fileEncoding)
        {
            string pathToFile = Path.Combine(currentFolder, fileName);

            var file = File.Create(pathToFile);
            file.Close();

            using (StreamWriter sw = new StreamWriter(File.Open(pathToFile, FileMode.Create), fileEncoding))
            {
                sw.WriteLine(text); //Пишем в файл текст в необходимой кодировке.
            }

            return true;
        }

        /// <summary>.
        /// Return text from file.
        /// </summary>.
        /// <param name="currentFolder">Path to current folder</param>.
        /// <param name="fileName">Name of file to get text from</param>.
        /// <param name="fileEncoding">Encoding to get text with</param>.
        /// <returns>Text from file</returns>.
        static public string GetTextFile(string currentFolder, string fileName, Encoding fileEncoding)
        {
            string pathToFile = Path.Combine(currentFolder, fileName); //Путь к файлу - текущая папка + имя файла.

            using (StreamReader sw = new StreamReader(File.Open(pathToFile, FileMode.Open), fileEncoding))
            {
                return sw.ReadToEnd();
            }
        }

        /// <summary>.
        /// Change using folder.
        /// </summary>.
        /// <param name="currentFolder">Path to current folder</param>.
        /// <param name="command">Relative path to change current path with</param>.
        /// <returns>True if changing accepted and False otherwise</returns>.
        static public bool ChangeFolder(ref string currentFolder, string command)
        {
            string currentFolderDublicate = currentFolder;

            while (command.StartsWith("../") || command.StartsWith("..")) //Пока переход на уровень вверх - переходим.
            {
                command = command.StartsWith("../") ? command.Substring(3) : command.Substring(2);

                if (isLinuxBased)
                {
                    currentFolderDublicate =
                        currentFolderDublicate.LastIndexOf("/") == -1 ?
                            currentFolderDublicate : currentFolderDublicate.Substring(0, currentFolderDublicate.LastIndexOf("/"));
                }
                else
                {
                    currentFolderDublicate =
                        currentFolderDublicate.LastIndexOf("\\") == -1 ?
                            currentFolderDublicate : currentFolderDublicate.Substring(0, currentFolderDublicate.LastIndexOf("\\"));
                }

            }

            if (command.EndsWith(".")) //Если заканчивается точкой => обращение к той же папке => режем её.
            {
                command = command.Substring(0, command.Length - 1);
            }

            if (!Directory.Exists(Path.Combine(currentFolderDublicate, command))) //Если такой директории нет.
            {
                string wrongPath = Path.Combine(currentFolderDublicate, command);


                string parentDir = "";

                if (isLinuxBased)
                {
                    parentDir = wrongPath.Substring(0, wrongPath.LastIndexOf("/"));
                }
                else
                {
                    parentDir = wrongPath.Substring(0, wrongPath.LastIndexOf("\\"));
                }

                Manager.MakePurePathToDisc(ref parentDir);

                string ending = "";

                if (isLinuxBased)
                {
                    ending = wrongPath.Substring(wrongPath.LastIndexOf("/"));
                    ending = ending.Substring("/".Length);
                }
                else
                {
                    ending = wrongPath.Substring(wrongPath.LastIndexOf("\\"));
                    ending = ending.Substring("\\".Length);
                }

                if (!Directory.Exists(parentDir))
                { //Если корня для директории нет.
                    return false;
                }
                else
                {
                    int occurrences = 0;
                    string occurenceName = "";
                    string pureDirName = "";

                    foreach (var dirName in Directory.GetDirectories(parentDir)) //Проверяем, есть ли строго одно совпадение по префиксу.
                    {

                        if (isLinuxBased)
                        {
                            pureDirName = dirName.Substring(dirName.LastIndexOf("/") + 1);
                        }
                        else
                        {
                            pureDirName = dirName.Substring(dirName.LastIndexOf("\\") + 1);
                        }

                        if (pureDirName.StartsWith(ending))
                        {
                            occurrences++;
                            occurenceName = pureDirName;
                        }
                    }

                    if (occurrences == 1) //Если подходящая папка только одна.
                    {
                        currentFolder = Path.Combine(Path.Combine(parentDir, occurenceName));
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

            }
            currentFolder = Path.Combine(currentFolderDublicate, command);
            return true;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //Initialize format - print dictionary.

            Talk.Initialize();

            //Adding format keys associated with text to print.

            Talk.AddFormatKey("help", "\nДоступные команды: \n" +
                "green<ls> - Выводит список файлов и папок в текущей директории \n" +
                "green<cd> - Переходит в директорию относительно текущей директории \n" +
                "       red<example/ex1/ex2> green<cd ../>    = red<example/ex1> \n" +
                "       red<example/ex1>     green<cd ex2>    = red<example/ex1/ex2> \n" +
                "       red<example/ex1/ex2> green<cd ../ex3> = red<example/ex1/ex3> \n" +
                "       Возможен переход в папку (Указанную последнюю в пути) при указании лишь начала названия папки\n" +
                "       (При отсутсвии конфликта имён)\n" +
                "       red<example/ex1/someFolder1> green<cd ../someF> = red<example/ex1/someFolder1> \n" +
                "       red<example/ex1> green<cd someFol> = red<example/ex1/someFolder1> \n" +
                "green<dr> - Показывает список всех доступных дисков с их номерами\n" +
                "green<dr 2> - Переходит к диску под номером 2\n" +
                "green<mv someFile.txt ../someFolder> - Переносит файл someFile.txt в папку someFolder\n" +
                "green<cp someFile.txt ../someFolder> - Копирует файл someFile.txt в папку someFolder\n" +
                "green<rm test.txt> - Удаляет файл test.txt из текущей папки\n" +
                "green<rn test.txt test2.txt> - Переименовывает файл test.txt в test2.txt\n" +
                "green<nano test.txt> - Выводит содержимое файла test.txt в консоль в кодировке utf8\n" +
                "green<nano test.txt ascii> - Выводит содержимое файла test.txt в консоль в кодировке ascii\n" +
                "                      yellow<Возможные названия кодировок: {utf8, utf7, utf32, ascii, unicode}>\n" +
                "green<touch test.txt> - Создаёт пустой файл test.txt в кодировке utf8\n" +
                "green<touch test.txt Some_File_for_u> - Создаёт файл test.txt в кодировке utf8 и пишет туда строку Some_File_for_u, \n" +
                "                                        где нижнее подчёркивание заменяется на пробел \n" +
                "green<touch test.txt Some_File_for_u ascii> - Создаёт файл test.txt в кодировке ascii и пишет туда строку Some_File_for_u, \n" +
                "                                       где нижнее подчёркивание заменяется на пробел \n" +
                "                                       yellow<Возможные названия кодировок: {utf8, utf7, utf32, ascii, unicode}>\n" +
                "green<concat file1.txt file2.txt file1.txt> - Обьединяет текстовые файлы и выводит в консоль их обьединение \n\n"
                );

            Talk.AddFormatKey("welcomeMessage", "Команды взяты из linux, для списка всех команд введите green<help>\n");

            ///////////////////////////////////////////////////////////////////////////////
            //                         Start of working                                  //
            ///////////////////////////////////////////////////////////////////////////////

            //Код состоит из компонентов, написанных в разное время и на разных ide//
            //Возможна смесь из xml и обычных комментариев                         //

            Talk.Out("welcomeMessage", "");

            string currentPath = Directory.GetCurrentDirectory(); //Путь к текущей директории.

            if (currentPath.Contains("\\")) //Если обратный слеш в строке - значит windows based system.
            {
                Manager.isLinuxBased = false;
            }
            else //Иначе - linux based system.
            {
                Manager.isLinuxBased = true;
            }

            string answer = "";
            while (true)
            {
                try
                {
                    Manager.MakePurePathToDisc(ref currentPath);

                    Talk.Out(Manager.MakePathLikeLinux(currentPath) + ":", ConsoleColor.Green, ""); //Выводим текущий путь.
                    Talk.Out("~$ ", ConsoleColor.White, "");
                    answer = "";

                    List<string> command = Talk.Get("array", "", "string", ' ').Value; //Получаем название команды.

                    if (command.Count < 1)
                    {
                        throw new Exception("Command incorrect");
                    }

                    switch (command[0])
                    {
                        case "ls":
                            answer = Manager.GetFilesInFolder(currentPath);
                            break;
                        case "cd":
                            if (command.Count != 2)
                            {
                                throw new Exception("Command incorrect");
                            }
                            if (!Manager.ChangeFolder(ref currentPath, command[1]))
                            {
                                throw new Exception("Command incorrect");
                            }
                            break;
                        case "dr":
                            if (command.Count == 1)
                            {
                                answer = Manager.GetDrives();

                            }
                            else
                            {
                                if (command.Count == 2)
                                {
                                    if (!Manager.ChangeDrive(ref currentPath, command[1]))
                                    {
                                        throw new Exception("Command incorrect");
                                    }
                                }
                            }
                            break;
                        case "mv":
                            if (command.Count != 3)
                            {
                                throw new Exception("Command incorrect");
                            }
                            if (!Manager.MoveFile(currentPath, command[1], command[2]))
                            {
                                throw new Exception("Command incorrect");
                            }
                            break;
                        case "rn":
                            if (command.Count != 3)
                            {
                                throw new Exception("Command incorrect");
                            }
                            if (!Manager.RenameFile(currentPath, command[1], command[2]))
                            {
                                throw new Exception("Command incorrect");
                            }
                            break;
                        case "cp":
                            if (command.Count != 3)
                            {
                                throw new Exception("Command incorrect");
                            }
                            if (!Manager.CopyFile(currentPath, command[1], command[2]))
                            {
                                throw new Exception("Command incorrect");
                            }
                            break;
                        case "rm":
                            if (command.Count != 2)
                            {
                                throw new Exception("Command incorrect");
                            }
                            if (!Manager.RemoveFile(currentPath, command[1]))
                            {
                                throw new Exception("Command incorrect");
                            }
                            break;
                        case "nano":
                            if (command.Count != 2 && command.Count != 3)
                            {
                                throw new Exception("Command incorrect");
                            }

                            if (command.Count == 2)
                            {
                                Talk.Out(Manager.GetTextFile(currentPath, command[1], Encoding.UTF8));
                            }
                            if (command.Count == 3)
                            {
                                switch (command[2])
                                {
                                    case "utf8":
                                        Talk.Out(Manager.GetTextFile(currentPath, command[1], Encoding.UTF8));
                                        break;
                                    case "ascii":
                                        Talk.Out(Manager.GetTextFile(currentPath, command[1], Encoding.ASCII));
                                        break;
                                    case "unicode":
                                        Talk.Out(Manager.GetTextFile(currentPath, command[1], Encoding.Unicode));
                                        break;
                                    case "utf32":
                                        Talk.Out(Manager.GetTextFile(currentPath, command[1], Encoding.UTF32));
                                        break;
                                    case "utf7":
                                        Talk.Out(Manager.GetTextFile(currentPath, command[1], Encoding.UTF7));
                                        break;
                                    default:
                                        throw new Exception("Command incorrect");
                                }
                            }

                            break;

                        case "concat":

                            if (command.Count < 3)
                            {
                                throw new Exception("Command incorrect");
                            }

                            Talk.Out(Manager.Concat(currentPath, command));
                            break;

                        case "touch":
                            if (command.Count != 2 && command.Count != 3 && command.Count != 4)
                            {
                                throw new Exception("Command incorrect");
                            }



                            if (command.Count == 2)
                            {
                                Manager.CreateFile(currentPath, command[1], "", Encoding.UTF8);
                            }

                            if (command.Count == 3)
                            {
                                command[2] = command[2].Replace("_", " ");
                                Manager.CreateFile(currentPath, command[1], command[2], Encoding.UTF8);
                            }

                            if (command.Count == 4)
                            {
                                command[2] = command[2].Replace("_", " ");
                                switch (command[3])
                                {
                                    case "utf8":
                                        Manager.CreateFile(currentPath, command[1], command[2], Encoding.UTF8);
                                        break;
                                    case "ascii":
                                        Manager.CreateFile(currentPath, command[1], command[2], Encoding.ASCII);
                                        break;
                                    case "unicode":
                                        Manager.CreateFile(currentPath, command[1], command[2], Encoding.Unicode);
                                        break;
                                    case "utf32":
                                        Manager.CreateFile(currentPath, command[1], command[2], Encoding.UTF32);
                                        break;
                                    case "utf7":
                                        Manager.CreateFile(currentPath, command[1], command[2], Encoding.UTF7);
                                        break;
                                    default:
                                        throw new Exception("Command incorrect");
                                }
                            }

                            break;

                        case "help":

                            Talk.Out("help", "");
                            break;
                    }

                    Talk.Out(answer, ConsoleColor.Blue, "\n");
                }
                catch (Exception e)
                {
                    Talk.Out(e.Message, ConsoleColor.Red, "\n");
                }
            }
        }
    }
}
