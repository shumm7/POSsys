using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;
using LitJson;
using CsvHelper;

public class DataLoader : MonoBehaviour
{
    //設定ファイル
    public class Config
    {
        public string StoreName  = "テストストア";
        public string[] TabName = new string[4] { "Tab1", "Tab2", "Tab3", "Tab4"};
        public int Tax = 10;
        public string Encoding = "Shift_JIS";
        public int ScreenResolutionWidth = 1920;
        public int ScreenResolutionHeight = 1280;
        public bool FullScreen = true;
        public bool EnableLINENotify = false;
        public string LINENotifyToken = "";
        public bool LINENotifyPurchaseNotice = false;
        public bool BarcodeReader = false;
        public double BarcodeReaderTimeOut = 0.1;
    }
    public bool SaveConfig(Config _config)
    {
        return generateConfig(@"config.json", JsonUtility.ToJson(_config));
    }
    public Config LoadConfig()
    {
        string json = loadFile(@"config.json");
        Config temp;
        try
        {
            temp = JsonUtility.FromJson<Config>(json);
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
            return null;
        }
        return temp;
    }

    //商品リスト
    public class List
    {
        public int Category { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int Stock { get; set; }
        public bool Available { get; set; }
        public string ImagePath { get; set; }
        public string ID { get; set; }
    }

    public class ListMapper : CsvHelper.Configuration.ClassMap<DataLoader.List>
    {
        public ListMapper()
        {
            Map(x => x.Category).Index(0);
            Map(x => x.Name).Index(1);
            Map(x => x.Price).Index(2);
            Map(x => x.Stock).Index(3);
            Map(x => x.Available).Index(4);
            Map(x => x.ImagePath).Index(5);
            Map(x => x.ID).Index(6);
        }
    }

    public static void SaveList(List<List> _list)
    {
        var path = @"list.csv";

        using (TextWriter fileWriter = new StreamWriter(path, false))
        using (var csv = new CsvHelper.CsvWriter(fileWriter, System.Globalization.CultureInfo.InvariantCulture))
        {
            csv.Configuration.HasHeaderRecord = true;
            csv.Configuration.RegisterClassMap<ListMapper>();
            csv.WriteRecords(_list);
        }
    }

    public static List<List> LoadList()
    {
        var path = @"list.csv";
        var result = new List<List>();
        {
            using (TextReader fileReader = File.OpenText(path))
            {
                using (var csv = new CsvReader(fileReader, System.Globalization.CultureInfo.InvariantCulture))
                {
                    csv.Configuration.HasHeaderRecord = true;
                    csv.Configuration.RegisterClassMap<ListMapper>();
                    csv.Read();
                    csv.ReadHeader();
                    while (csv.Read())
                    {
                        var record = new List
                        {
                            Category = int.Parse(csv.GetField("Category")),
                            Name = csv.GetField("Name"),
                            Price = int.Parse(csv.GetField("Price")),
                            Stock = int.Parse(csv.GetField("Stock")),
                            Available = bool.Parse(csv.GetField("Available")),
                            ImagePath = csv.GetField("ImagePath"),
                            ID = csv.GetField("ID")
                        };
                        result.Add(record);
                    }
                }
            }
            return result;
        }
    }

    public class Payment
    {
        public int RegisterCash;
        public int Before;
        public int After;
        public int Increase;
        public string Description;
        public DateTime Time;
    }

    public Payment LoadPayment(int num)
    {
        if (checkExist("data/payment/" + num.ToString() + ".json"))
        {
            string json = loadFile("data/payment/" + num.ToString() + ".json");
            Payment temp;
            Directory.CreateDirectory("data/payment");
            try
            {
                temp = JsonMapper.ToObject<Payment>(json);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
                return null;
            }
            return temp;
        }
        else
        {
            Payment temp = new Payment();
            temp.RegisterCash = 0;
            temp.Before = 0;
            temp.After = 0;
            temp.Increase = 0;
            temp.Description = "";
            return temp;
        }
    }

    public Payment LoadLeastPayment()
    {
        int count = 0;
        Directory.CreateDirectory("data/payment");
        while (true)
        {
            if (!checkExist("data/payment/" + count.ToString() + ".json"))
            {
                break;
            }
            count++;
        }
        if (count != 0)
        {
            return LoadPayment(count - 1);
        }
        else
        {
            Payment temp = new Payment();
            temp.RegisterCash = 0;
            temp.Before = 0;
            temp.After = 0;
            temp.Increase = 0;
            temp.Description = "";
            return temp;
        }
    }

    public void AddPayment(int money, string description, bool RegisterSettings)
    {
        int count = 0;
        Directory.CreateDirectory("data/payment");
        while (true)
        {
            if (!checkExist("data/payment/" + count.ToString() + ".json"))
            {
                break;
            }
            count++;
        }

        Payment temp = new Payment();

        if (!RegisterSettings)
        {
            temp.Before = LoadLeastPayment().After;
            temp.After = temp.Before + money;
            temp.RegisterCash = LoadLeastPayment().RegisterCash;
            temp.Increase = money;
            temp.Description = description;
            temp.Time = DateTime.Now;
        }
        else
        {
            temp.Before = LoadLeastPayment().After;
            temp.After = money;
            temp.RegisterCash = money;
            temp.Increase = money - temp.Before;
            temp.Description = description;
            temp.Time = DateTime.Now;
        }

        saveFile("data/payment/" + count.ToString() + ".json", JsonMapper.ToJson(temp));
    }

    public void SaveOrderList(string path, List<GameManager.OrderList> orderList)
    {
        if (path.Substring(path.Length - 5) == "0.csv")
        {
            var r = new List<GameManager.OrderDataCSVList>();
            if (GetComponent<DataLoader>().checkExist("data/order/order.csv"))
            {
                var result = new List<GameManager.OrderDataCSVList>();
                {
                    using (TextReader fileReader = File.OpenText(@"data/order/order.csv"))
                    {
                        using (var csv = new CsvReader(fileReader, System.Globalization.CultureInfo.InvariantCulture))
                        {
                            csv.Configuration.HasHeaderRecord = true;
                            csv.Configuration.RegisterClassMap<GameManager.OrderDataCSVListMapper>();
                            csv.Read();
                            csv.ReadHeader();
                            while (csv.Read())
                            {
                                var record = new GameManager.OrderDataCSVList
                                {
                                    Year = int.Parse(csv.GetField("Year")),
                                    Month = int.Parse(csv.GetField("Month")),
                                    Day = int.Parse(csv.GetField("Day"))
                                };
                                result.Add(record);
                            }
                        }
                    }
                }
                r = result;
            }
            r.Add(new GameManager.OrderDataCSVList { Year = DateTime.Now.Year, Month = DateTime.Now.Month, Day = DateTime.Now.Day });

            using (TextWriter fileWriter = new StreamWriter(@"data/order/order.csv", false))
            using (var csv = new CsvWriter(fileWriter, System.Globalization.CultureInfo.InvariantCulture))
            {
                csv.Configuration.HasHeaderRecord = true;
                csv.Configuration.RegisterClassMap<GameManager.OrderDataCSVListMapper>();
                csv.WriteRecords(r);
            }
        }

        using (TextWriter fileWriter = new StreamWriter(path, true))
        using (var csv = new CsvWriter(fileWriter, System.Globalization.CultureInfo.InvariantCulture))
        {
            csv.Configuration.HasHeaderRecord = true;
            csv.Configuration.RegisterClassMap<GameManager.OrderListMapper>();
            csv.WriteRecords(orderList);
        }
    }

    public List<GameManager.OrderList> LoadOrderList(string path)
    {
        var result = new List<GameManager.OrderList>();
        if (GetComponent<DataLoader>().checkExist(path))
        {
            using (TextReader fileReader = File.OpenText(@path))
            {
                using (var csv = new CsvReader(fileReader, System.Globalization.CultureInfo.InvariantCulture))
                {
                    csv.Configuration.HasHeaderRecord = true;
                    csv.Configuration.RegisterClassMap<GameManager.OrderListMapper>();
                    csv.Read();
                    csv.ReadHeader();
                    while (csv.Read())
                    {
                        var record = new GameManager.OrderList
                        {
                            Name = csv.GetField("Name"),
                            Category = int.Parse(csv.GetField("Category")),
                            Number = int.Parse(csv.GetField("Number")),
                            Amount = int.Parse(csv.GetField("Amount")),
                            Price = int.Parse(csv.GetField("Price")),
                        };

                        result.Add(record);
                    }
                    fileReader.Close();
                }
            }
        }

        return result;
    }

    public bool saveFile(string Filename)
    {
        return saveFile(Filename, null);
    }

    public bool saveFile(string Filename, string text)
    {
        StreamWriter sw = null;
        bool res;

        try
        {
            sw = new StreamWriter(@Filename, false, Encoding.GetEncoding(LoadConfig().Encoding));
            sw.WriteLine(text);
            sw.Flush();
            res = true;
        }
        catch (Exception e)
        {
            Debug.LogWarningFormat("Cannot open {0}", @Filename);
            Debug.LogWarning(e.Message);
            res = false;
        }
        finally
        {
            if (sw != null)
                sw.Close();
        }
        return res;
    }

    private bool generateConfig(string Filename, string text)
    {
        StreamWriter sw = null;
        bool res;

        try
        {
            sw = new StreamWriter(@Filename, false);
            sw.WriteLine(text);
            sw.Flush();
            res = true;
        }
        catch (Exception e)
        {
            Debug.LogWarningFormat("Cannot open {0}", @Filename);
            Debug.LogWarning(e.Message);
            res = false;
        }
        finally
        {
            if (sw != null)
                sw.Close();
        }
        return res;
    }

    public bool AddLine(string Filename, string text)
    {
        StreamWriter sw = null;
        bool res;

        try
        {
            sw = new StreamWriter(@Filename, true, Encoding.GetEncoding(LoadConfig().Encoding));
            sw.WriteLine(text);
            sw.Flush();
            res = true;
        }
        catch (Exception e)
        {
            Debug.LogWarningFormat("Cannot open {0}", @Filename);
            Debug.LogWarning(e.Message);
            res = false;
        }
        finally
        {
            if (sw != null)
                sw.Close();
        }
        return res;
    }

    public string loadFile(string Filename)
    {
        StreamReader sr = null;
        string text;

        try
        {
            sr = new StreamReader(@Filename, false);
            text = sr.ReadToEnd();
        }
        catch (Exception e)
        {
            Debug.LogWarningFormat("Cannot open {0}", @Filename);
            Debug.LogWarning(e.Message);
            text = null;
        }
        finally
        {
            if (sr != null)
                sr.Close();
        }
        return text;
    }

    public bool checkExist(string Filename)
    {
        return File.Exists(@Filename);
    }

}
