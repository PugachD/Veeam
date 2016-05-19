using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

namespace GZipStream
{
    class Program
    {
        #region Приватные поля класса
        private static string operation = "", pattern = "", firstName = "", secondName = "";
        private static Regex regExp;
        private static bool returned;
        private static string stroka = "";
        #endregion

        static int Main(string[] args)
        {
            char repeat = 'A';
            //Thread thread = new Thread(Algorithm);
            stroka = args.ToString();
            do
            {
                ShowHelp();
                returned = true;
                Algorithm();
                Console.WriteLine("Желаете повторить? Y - Да. N - Нет.");
                repeat = Console.ReadKey().KeyChar;
                Console.WriteLine();
                if (repeat == 'Y')
                    stroka = Console.ReadLine();
            } while(repeat != 'N');
            

            Console.WriteLine("Для выхода нажмите любую клавишу");
            Console.ReadLine();
            if (returned)
                return 1;
            else
                return 0;
        }

        #region Вспомогательные методы

        /// <summary>
        /// Основной метод
        /// </summary>
        private static void Algorithm()
        {
            pattern = @"^(\b((c|dec)ompress)\b\s[[][^\[\]]{1,50}[]]\s[[][^\[\]]{1,50}[]])$";
            try
            {
                regExp = new Regex(pattern); //Создаем экземпляр для указанного регулярного выражения
                //Если входная строка соответствует шаблону регулярного выражения, то выполняем
                //Иначе: выводим сообщение 
                if (regExp.IsMatch(stroka))
                {
                    Parser(stroka);

                    //Запускаем метод в зависимости от операции
                    MethodInfo method = Type.GetType("GZipStream.Program").GetMethod(operation);
                    method.Invoke(null, null);
                }
                else
                    throw new Exception("Неверно задана строка параметров!!!\n");
            }
            catch (FileNotFoundException ex)
            {
                returned = false;
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                returned = false;
                Console.WriteLine(ex.Message);
            }
            finally
            {
            }
        }

        /// <summary>
        /// Метод сжатия файла
        /// </summary>
        static public void compress()
        {
            string directory = Directory.GetCurrentDirectory();
            FileInfo fileToCompress = new FileInfo(directory + "\\" + firstName);
            FileInfo archive = new FileInfo(directory + "\\" + secondName);
            //Определяем наличие заданного файла.
            try
            {
                if (fileToCompress.Exists)
                {
                    using (FileStream originalFileStream = fileToCompress.OpenRead())
                    {
                        if ((File.GetAttributes(fileToCompress.FullName) & FileAttributes.Hidden) != FileAttributes.Hidden
                            & fileToCompress.Extension != ".gz")
                        {
                            using (FileStream compressFileStream = File.Create(archive.FullName + ".gz"))
                            {
                                using (System.IO.Compression.GZipStream compressionStream = new System.IO.Compression.GZipStream(compressFileStream, CompressionMode.Compress))
                                {
                                    originalFileStream.CopyTo(compressionStream);
                                }
                            }
                            FileInfo info = new FileInfo(archive.FullName + ".gz");
                            Console.WriteLine("Файл {0} сжат с {1} до {2} байт.",
                                fileToCompress.Name, fileToCompress.Length.ToString(), info.Length.ToString());
                        }
                    }
                }
                else
                    throw new FileNotFoundException(String.Format("Файл с именем {0} в каталоге {1} не найден", firstName, directory));
            }
            catch { };
                    
        }

        /// <summary>
        /// Метод разархивации
        /// </summary>
        static public void decompress()
        {
            string directory = Directory.GetCurrentDirectory();
            FileInfo fileToDeCompress = new FileInfo(directory + "\\" + firstName);
            FileInfo archive = new FileInfo(directory + "\\" + secondName);
            //Определяем наличие заданного файла.
            try
            {
                if (fileToDeCompress.Exists)
                {
                    using (FileStream originalFileStream = fileToDeCompress.OpenRead())
                    {

                        using (FileStream decompressFileStream = File.Create(secondName))
                        {
                            using (System.IO.Compression.GZipStream decompressionStream = new System.IO.Compression.GZipStream(originalFileStream, CompressionMode.Decompress))
                            {
                                decompressionStream.CopyTo(decompressFileStream);
                                Console.WriteLine("Decompressed {0}", fileToDeCompress.Name);
                            }
                        }
                    }
                }
                else
                    throw new FileNotFoundException(String.Format("Архив с именем {0} в каталоге {1} не найден", firstName, directory));
            }
            catch { };
        }

        /// <summary>
        /// Считывание строки
        /// </summary>
        private static void Parser(string inputString)
        {
            pattern = @"\b((c|dec)ompress)\b";
            regExp = new Regex(pattern);
            operation = regExp.Match(inputString).ToString(); //Определяем операцию архивации/разархивации
            inputString = DeConcat(inputString, operation); //Извлечь строку можно несколькими способами.
            //stroka = stroka.Replace(operation + " ", "");

            pattern = @"^([[][^\[\]]{1,50}[]])";
            regExp = new Regex(pattern);
            firstName = regExp.Match(inputString).ToString(); //Запоминаем имя в первых квадратных скобках
            inputString = DeConcat(inputString, firstName); 
            firstName = firstName.Substring(1, firstName.Length - 2); //Убираем из имени квадратные скобки

            secondName = regExp.Match(inputString).ToString(); //Запоминаем имя во вторых квадратных скобках
            secondName = secondName.Substring(1, secondName.Length - 2);
        }

        /// <summary>
        /// Удаление подстроки strSecond из строки strFirst
        /// </summary>
        public static string DeConcat(string strFirst, string strSecond)
        {
            return (string)strFirst.Substring(strFirst.IndexOf(strSecond) + strSecond.Length + 1);
        }


        private static void ShowHelp()
        {
            string helpMsg = string.Format("Для архивации: GZipTest.exe compress [имя исходного файла] [имя архива] \n"
                        + "Для разархивации: GZipTest.exe decompress  [имя архива] [имя распакованного файла]");
            Console.WriteLine(helpMsg);
        }
        #endregion


    }

}
