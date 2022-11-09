using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace cghw
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Добро пожаловать в приложение для обработки данных.");
            while (true)
            {
                Start();
            }
        }
        //Метод, который отвечает за связь интерфейса и программы.
        static void Start()
        {
            MainMenu();
            int n=ReadNumber(0,5);
            switch (n)
            {
                case 0:
                    Console.WriteLine("Завершение программы");
                    Environment.Exit(0);
                    break;
                case 1:
                    AddNewDataset();
                    break;
                case 2:
                    InformationWhereMaxis();
                    break;
                case 3:
                    SortedInformation();
                    break;
                case 4:
                    YearAboveAverage();
                    break;
                case 5:
                    SummaryStatistics();
                    break;
            }
        }
        //Объявление основых массивов, которые отвечают за хранение всех данных.
        public static string[,] procdata = null;
        public static string[] data = null;
        //Функция, которая добавляет и читает датасет.
        public static string [,] AddNewDataset()
        {
            string datasetname;
            try
            {
                datasetname = CheckDataExist();
                try
                {
                    data = File.ReadAllLines(datasetname);
                    if (data[0].Split(',')[0] == "Name" && data[0].Split(',')[1] == "Developer" &&
                        data[0].Split(',')[2] == "Producer" && data[0].Split(',')[3] == "Genre" &&
                        data[0].Split(',')[4] == "Operating System" && data[0].Split(',')[5] == "Date Released")
                    {
                        procdata = new string[data.Length, (data[0].Split(',')).Length];
                        for (int i = 0; i < data.Length; i++)
                        {
                            string temp = null;
                            string[] splitted = data[i].Split('"');
                            for (int j = 0; j < splitted.Length; j++)
                            {
                                if (j % 2 != 0)
                                {
                                    splitted[j] = splitted[j].Replace(',', '*');
                                    temp = temp + '"' + splitted[j] + '"';
                                }
                                else
                                {
                                    temp += splitted[j];
                                }
                            }

                            string[] finalsplit = temp.Split(',');
                            for (int j = 0; j < finalsplit.Length; j++)
                            {
                                finalsplit[j] = finalsplit[j].Replace('*', ',');
                                procdata[i, j] = finalsplit[j];
                            }
                        }
                    }
                }
                catch (IOException)
                {
                    Console.WriteLine("Ошибка чтения данных.");
                }
                catch (IndexOutOfRangeException)
                {
                    Console.WriteLine("Ошибка");
                }
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Такого датасета не существует или указан неверный формат файла.");
            }

            return procdata;

        }
        //Метод, где идет вывод данных,в которых разработчиком является Maxis.
         static void InformationWhereMaxis()
         {
             try
             {
                 File.AppendAllText("Developer_Maxis.csv","Name,Developer,Producer,Genre,Operating System,Date Released");
                 for (int i = 1; i < procdata.GetLength(0); i++)
                 {
                     if (procdata[i, 1].Contains("Maxis"))
                     {
                         //Вывод данных и добавление в файл.
                         Console.WriteLine(data[i]);
                         File.AppendAllText("Developer_Maxis.csv",data[i]+"\n");
                     }
                 }
             }
             catch (NullReferenceException)
             {
                 Console.WriteLine("Нет данных для работы!");
             }
             
         }
        //Метод отвечающий за сортировку данных.
        static void SortedInformation()
        {
            try
            {
                List<string> micWin = new List<string>();
                List<int> micWinyear = new List<int>();
                List<string> appleWin = new List<string>();
                List<int> appleWinyear = new List<int>();
                for (int i = 1; i < data.Length; i++)
                {
                    if (procdata[i, 4] == "Microsoft Windows")
                    {
                        string[] time = StringToData(i);
                        for (int j = 0; j < time.Length; j++)
                        {
                            if (int.TryParse(time[j], out int year) && year > 1000)
                            {
                                micWinyear.Add(year);
                                micWin.Add(data[i]);
                            }
                            
                        }
                    }

                    if (procdata[i, 4] == "\"Microsoft Windows, macOS\"")
                    {
                        string[] time = StringToData(i);
                        for (int j = 0; j < time.Length; j++)
                        {
                            if (int.TryParse(time[j], out int year) && year > 1000)
                            {
                                appleWinyear.Add(year);
                                appleWin.Add(data[i]);
                                
                            }
                        }
                    }
                    //Добавление всех данных в массивы.
                    
                }
                //Сортировка и вывод данных.
                Sort(appleWin,appleWinyear);
                Sort(micWin,micWinyear);
                int min = 0;
                int max = 0;
                if (micWinyear.Max() > appleWinyear.Max())
                {
                     max = micWinyear.Max();
                }
                else
                {
                     max = appleWinyear.Max();
                }
                if (appleWinyear.Min() > micWinyear.Min())
                {
                     min = micWinyear.Min();
                }
                else
                {
                     min = appleWinyear.Min();
                }
                int delta = max-min;
                Console.WriteLine(delta);
                MakeSpace();
                for (int k = 0; k < micWin.Count; k++)
                {
                    File.AppendAllText("Microsoft_Windows_Sort.csv", micWin[k]+"\n");
                    Console.WriteLine(micWin[k]);
                }
                MakeSpace();
                for (int k = 0; k < appleWin.Count; k++)
                {
                    File.AppendAllText("Microsoft_Windows_Mac_Sort.csv", appleWin[k]+"\n");
                    Console.WriteLine(appleWin[k]);
                }
                MakeSpace();

            }
            catch (NullReferenceException)
            {
                Console.WriteLine("Нет данных для работы!");
            }
            
        }
        //Функция сортировки пузырьком.
        static void Sort(List<string> appleWin, List<int> appleWinyear)
        {
            int tempyear = 0;
            string tempdata = null;
            for (int j = 0; j < appleWin.Count; j++)
            {
                for (int sort = 0; sort < appleWin.Count - 1; sort++)
                {
                    if (appleWinyear[sort] > appleWinyear[sort + 1])
                    {
                        tempyear = appleWinyear[sort+1];
                        appleWinyear[sort + 1] = appleWinyear[sort];
                        appleWinyear[sort] = tempyear;
                        tempdata = appleWin[sort+1];
                        appleWin[sort + 1] = appleWin[sort];
                        appleWin[sort] = tempdata;
                    }
                }
            }     
        }
        //Задание 3.
        static void YearAboveAverage()
        {
            try
            {
                Console.WriteLine("Введите название для записи в файл.");
                string filename = Console.ReadLine();
                if (filename.Length > 4 && filename.Substring(filename.Length - 4) == ".csv")
                {
                    File.AppendAllText(filename,"Name,Developer,Producer,Genre,Operating System,Date Released"+"\n");
                }
                else
                {
                    Console.WriteLine("Неверное имя файла!");
                }

                long averageyear = 0;
                long k = 0;
                for (int i = 1; i < data.Length; i++)
                {
                    //Поиск среднего года.
                    string[] time = StringToData(i);
                    for (int j = 0; j < time.Length; j++)
                    {
                        if (int.TryParse(time[j], out int year) && year > 1000)
                        {
                            averageyear += year;
                            k++;
                        }
                    }
                }

                averageyear = averageyear / k;
                for (int i = 1; i < data.Length; i++)
                {
                    string[] time = StringToData(i);
                    for (int j = 0; j < time.Length; j++)
                    {
                        if (int.TryParse(time[j], out int year) && year > 1000)
                        {
                            //Нахождение года, больше среднего.
                            if (year > averageyear)
                            {
                                Console.WriteLine(data[i]);
                                if (File.Exists(filename))
                                {
                                    File.AppendAllText(filename, data[i]+"\n");
                                }
                            }
                        }
                    }
                }

            }
            catch (NullReferenceException)
            {
                Console.WriteLine("Нет данных для работы!");
            }
            catch (IOException)
            {
                Console.WriteLine("Проблема записи в файл.");
            }
            catch (DivideByZeroException)
            {
                Console.WriteLine("Ошибка!");
            }
        }
            //Метод обработки ячейки массива, чтобы можно было взять дату.
        static string[] StringToData(int i)
        {
            string[] time = procdata[i, 5].Replace(" (Early Acess", "").Replace("\"", "").Replace(",", "")
                .Split(' ');
            return time;
        }
        //Метод для вывода общей статистики
        static void SummaryStatistics()
        {
            try
            {
                var dict = new Dictionary<string, int>();
                for (int i = 1; i < data.Length; i++)
                {
                    if (!dict.ContainsKey(procdata[i, 2]))
                    {
                        dict[procdata[i, 2]] = 1;
                    }
                    else
                    {
                        int k = dict[procdata[i, 2]];
                        dict[procdata[i, 2]] = k + 1;
                    }
                }
                Console.WriteLine("Общее количество игр по каждому производителю.");
                int minvalue = 10000;
                foreach (var producer in dict)
                {
                    Console.WriteLine($"Название  производителя: {producer.Key}. Количество игр: {producer.Value}");
                    if (producer.Value < minvalue)
                    {
                        minvalue = producer.Value;
                    }
                }
                MakeSpace();
                int minyear = 10000;
                int minindex = 0;
                for (int i = 1; i < data.Length; i++)
                {
                    if (procdata[i, 4] == "Microsoft Windows" && procdata[i, 3] == "First-person shooter")
                    {
                        string[] time = StringToData(i);
                        for (int j = 0; j < time.Length; j++)
                        {
                            if (int.TryParse(time[j], out int year) && year > 1000)
                            {
                                if (year < minyear)
                                {
                                    minyear = year;
                                    minindex = i;
                                }
                            }
                        }
                    }
                }
                Console.WriteLine("Данные о самой ранней выпущенной игре, где OS=MW, жанр= First-person shooter");
                Console.WriteLine(data[minindex]);
                MakeSpace();
                var genredata = new Dictionary<string, int>();
                var genrecount = new Dictionary<string, int>();
                for (int i = 1; i < data.Length; i++)
                {
                    string[] time = StringToData(i);
                    for (int j = 0; j < time.Length; j++)
                    {
                        if (int.TryParse(time[j], out int year) && year > 1000)
                        {
                            if (!genredata.ContainsKey(procdata[i, 3]))
                            {
                                genredata[procdata[i, 3]] = year;
                                genrecount[procdata[i, 3]] = 1;
                            }
                            else
                            {
                                int k = genredata[procdata[i, 3]];
                                int z = genrecount[procdata[i, 3]];
                                genredata[procdata[i, 3]] = k + year;
                                genrecount[procdata[i, 3]] = z + 1;
                            }
                        }
                    }
                }
                Console.WriteLine("Информация о среднем значении года выпуска игры по жанрам.");
                MakeSpace();
                foreach (var data in genredata)
                {
                    Console.WriteLine($"Название  Жанра: {data.Key}. Средний год жанра: { data.Value/genrecount[data.Key] }");
                }
                MakeSpace();
                Console.WriteLine("Продюсеры, который связан с минимальным количеством компьютерных игр.");
                MakeSpace();
                foreach (var producer in dict)
                {
                    if (producer.Value == minvalue)
                    {
                        Console.WriteLine(producer.Key);
                    }
                }
                MakeSpace();
                
                
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("Нет данных для работы!");
            }

        }

        static void MakeSpace()
        {
            Console.WriteLine();
            Console.WriteLine();
        }
        //Проверка файла на существование.
        static string CheckDataExist()
        {
            Console.WriteLine("Введите название датасета: ");
            string datasetname = Console.ReadLine();
            if (!File.Exists(datasetname) || datasetname.Length<5 && datasetname.Substring(datasetname.Length-4)!=".csv")
            {
                throw new ArgumentException();
            }

            return datasetname;

        }
        //Вывод интерфейса
        static void MainMenu()
        {
            Console.WriteLine(" Выберите номер пункта, что вы хотите сделать:\n" +
                              " 1. Добавить файл с данными. \n" +
                              " 2. Вывод данных об играх, где разработчиком является Maxis. \n" +
                              " 3. Вывод переупорядоченного набора данных. \n" +
                              " 4. Вывод выборки игр. \n" +
                              " 5. Вывод сводной статистики. \n "+
                              "Для выхода введите 0");
        }
        //Метод, для чтения цифры, которая передается для вызова определенного метода.
        static int ReadNumber(int firstnumber, int secondnumber)
        {
            while (true)
            {
                bool check = int.TryParse(Console.ReadLine(),out int n);
                if (check)
                {
                    if (n >= firstnumber && n <= secondnumber)
                    {
                        return n;
                    }
                    Console.WriteLine("Невозможное значение пункта.");
                }
                if (!check)
                {
                    Console.WriteLine("Неверный ввод, попробуйте снова."); 
                }
            }
        }
    }
}
