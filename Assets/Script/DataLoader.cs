using UnityEngine;
using System.IO;
using System;
using System.Text;
using LitJson;

public class DataLoader : MonoBehaviour
{
    //設定ファイル
    public class Config
    {
        public string StoreName  = "テストストア";
        public int Tax = 10;
        public string Encoding = "Shift_JIS";
        public int ScreenResolutionWidth = 1920;
        public int ScreenResolutionHeight = 1280;
        public bool FullScreen = true;
        public bool EnableLINENotify = false;
        public string LINENotifyToken = "";
        public bool LINENotifyPurchaseNotice = false;

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
        public List()
        {
            for (int i = 0; i < 4; i++) {
                ProductName[i] = new string[MAX_PRODUCT_AMOUNT];
                Price[i] = new int[MAX_PRODUCT_AMOUNT];
                Stock[i] = new int[MAX_PRODUCT_AMOUNT];
                Sales[i] = new int[MAX_PRODUCT_AMOUNT];
                Available[i] = new bool[MAX_PRODUCT_AMOUNT];
            }
        }

        public int MAX_PRODUCT_AMOUNT = 6;
        public string[] CategoryName = { "TabA", "TabB", "TabC", "TabD"};
        public string[][] ProductName = new string[4][];
        public int[][] Price = new int[4][];
        public int[][] Stock = new int[4][];
        public int[][] Sales = new int[4][];
        public bool[][] Available = new bool[4][];
    }
    public bool SaveList(List _list)
    {
        return saveFile(@"list.json", JsonMapper.ToJson(_list));
    }
    public List LoadList()
    {
        string json = loadFile(@"list.json");
        List temp;
        try
        {
            temp = JsonMapper.ToObject<List>(json);
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
            return null;
        }
        return temp;
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

    public GameManager.OrderList LoadOrderList(string date, int num)
    {
        if (checkExist("data/receipt/logs/" + date + "/" + num.ToString() + ".json"))
        {
            string json = loadFile("data/receipt/logs/" + date + "/"+ num.ToString() + ".json");
            GameManager.OrderList temp;
            try
            {
                temp = JsonMapper.ToObject<GameManager.OrderList>(json);
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
            return new GameManager.OrderList();
        }
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
