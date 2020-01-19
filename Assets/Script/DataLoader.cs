using UnityEngine;
using System.IO;
using System;
using LitJson;

public class DataLoader : MonoBehaviour
{
    //設定ファイル
    public class Config
    {
        public string StoreName  = "テストストア";
        public int Tax = 10;
    }
    public bool SaveConfig(Config _config)
    {
        return saveFile(@"config.json", JsonUtility.ToJson(_config));
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


    public bool saveFile(string Filename, string text)
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
