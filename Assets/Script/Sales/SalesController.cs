using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using CsvHelper;

public class SalesController : MonoBehaviour
{
    //GameObject
    public GameObject[] UI = new GameObject[4];

    // 内部変数
    int UImode = 0;
    List<GameManager.OrderDataCSVList> 日付リスト;
    int 日付データ数;

    //当日売り上げ
    public Text 当日売上日付;
    private int 当日売上日付番号;
    public Text 当日売上注文ID;
    private int 当日売上注文ID番号;
    public Text 当日売上売上額;
    public Text 当日売上総売上額;
    public Text 当日売上日時;
    public GameObject 当日売上商品リスト;
    public GameObject 当日売上商品;

    //時間帯別売上
    public Text 時間帯別売上日付;
    private int 時間帯別売上日付番号;
    private int 時間帯別売上時間番号;
    public Text 時間帯別売上時間;
    public Text 時間帯別売上個数;
    public Text 時間帯別売上総売上額;
    public GameObject 時間帯別売上商品リスト;
    public GameObject 時間帯別売上商品;

    //商品別売上
    public Text 商品別売上日付;
    private int 商品別売上日付番号;
    public Text 商品別売上商品名;
    private int 商品別売上商品番号;
    public Text 商品別売上売上額;
    public Text 商品別売上個数;
    public GameObject 商品別売上商品リスト;
    public GameObject 商品別売上商品;

    void Start()
    {
        UImode = 0;
        日付リスト = 日付データ取得();
        日付データ数 = 日付リスト.Count;
        SetUI(UImode);
    }

    void Update()
    {

    }

    public void Clicked小ボタン(int num)
    {
        SetUI(num);
    }

    public void Clicked当日売上番号ボタン(int num)
    {
        if (日付データ数 > 0)
        {
            if (num == -1) //リセット
            {
                当日売上日付番号 = 日付データ数 - 1;
                当日売上注文ID番号 = 注文データ数(日付リスト[当日売上日付番号]) - 1;
            }
            else if (num == 0) //日付 前へ
            {
                当日売上日付番号 = Number.Range(当日売上日付番号 - 1, 0, 日付データ数 - 1);
                当日売上注文ID番号 = 注文データ数(日付リスト[当日売上日付番号]) - 1;
            }
            else if (num == 1) //日付 次へ
            {
                当日売上日付番号 = Number.Range(当日売上日付番号 + 1, 0, 日付データ数 - 1);
                当日売上注文ID番号 = 注文データ数(日付リスト[当日売上日付番号]) - 1;
            }
            else if (num == 2) //ID 前へ
            {
                当日売上注文ID番号 = Number.Range(当日売上注文ID番号 - 1, 0, 注文データ数(日付リスト[当日売上日付番号]) - 1);
            }
            else if (num == 3) //ID 次へ
            {
                当日売上注文ID番号 = Number.Range(当日売上注文ID番号 + 1, 0, 注文データ数(日付リスト[当日売上日付番号]) - 1);
            }
            当日売上日付.text = 日付リスト[当日売上日付番号].Year.ToString() + "年 " + 日付リスト[当日売上日付番号].Month.ToString() + "月 " + 日付リスト[当日売上日付番号].Day.ToString() + "日";
            当日売上注文ID.text = 当日売上注文ID番号.ToString();
            当日売上日時.text = File.GetCreationTime(@"data/order/" + Number.FormatDate(日付リスト[当日売上日付番号].Year, 日付リスト[当日売上日付番号].Month, 日付リスト[当日売上日付番号].Day) + "/" + 当日売上注文ID番号.ToString() + ".csv").ToString("yyyy年MM月dd日 HH:mm:ss");
            当日売上売上額.text = Number.MarkDecimal(注文総額取得(注文データ取得(日付リスト[当日売上日付番号], 当日売上注文ID番号))) + " 円";
            当日売上商品リストUI設定(注文データ取得(日付リスト[当日売上日付番号], 当日売上注文ID番号));
            int allPrice = 0;
            for (int i = 0; i < 注文データ数(日付リスト[当日売上日付番号]); i++)
            {
                allPrice += 注文総額取得(注文データ取得(日付リスト[当日売上日付番号], i));
            }
            当日売上総売上額.text = Number.MarkDecimal(allPrice) + " 円";
        }
    }

    private void SetUI(int num)
    {
        if (num == 0) //当日売上
        {
            UI[0].SetActive(true);
            UI[1].SetActive(false);
            UI[2].SetActive(false);
            //UI[3].SetActive(false);
            Clicked当日売上番号ボタン(-1);
        }
        else if (num == 1) //時間帯別売上
        {
            UI[0].SetActive(false);
            UI[1].SetActive(true);
            UI[2].SetActive(false);
            //UI[3].SetActive(false);
            Clicked時間帯別売上日付ボタン(-1);
        }
        else if (num == 2) //期間内売上
        {
            UI[0].SetActive(false);
            UI[1].SetActive(false);
            UI[2].SetActive(true);
            //UI[3].SetActive(false);
            Clicked商品別売上日付ボタン(-1);
        }
        else if (num == 3) //商品別売上
        {
            UI[0].SetActive(false);
            UI[1].SetActive(false);
            //UI[2].SetActive(false);
            //UI[3].SetActive(true);
        }
    }

    private List<GameManager.OrderDataCSVList> 日付データ取得()
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

                            if (注文データ数(record) != 0)
                                result.Add(record);
                        }
                    }
                    fileReader.Close();
                }
            }
            r = result;
        }

        return r;
    }

    private GameManager.OrderDataCSVList 最新日付データ取得()
    {
        return 日付データ取得()[0];
    }

    private int 注文データ数(GameManager.OrderDataCSVList Date)
    {
        string dir = Number.FormatDate(Date.Year, Date.Month, Date.Day);
        if (File.Exists(@"data/order/" + dir + "/0.csv"))
        {
            int cnt = 0;
            while (true)
            {
                if (!File.Exists(@"data/order/" + dir + "/" + cnt + ".csv"))
                {
                    return cnt;
                }
                cnt++;
            }
        }
        else
        {
            return 0;
        }
    }

    private List<GameManager.OrderList> 注文データ取得(GameManager.OrderDataCSVList Date, int num)
    {
        string path = "data/order/" + Number.FormatDate(Date.Year, Date.Month, Date.Day) + "/" + num.ToString() + ".csv";
        return GetComponent<DataLoader>().LoadOrderList(path);
    }

    private List<GameManager.OrderList> 時間帯別注文データ取得(GameManager.OrderDataCSVList Date, int Hour)
    {
        List<GameManager.OrderList> result = new List<GameManager.OrderList>();
        for (int i = 0; i < 注文データ数(Date); i++)
        {
            var data = 注文データ取得(Date, i);
            var Time = File.GetCreationTime(@"data/order/" + Number.FormatDate(日付リスト[時間帯別売上日付番号].Year, 日付リスト[時間帯別売上日付番号].Month, 日付リスト[時間帯別売上日付番号].Day) + "/" + i.ToString() + ".csv");

            if (int.Parse(Time.ToString("HH")) == Hour)
            {
                foreach (var r in data)
                {
                    if (result.Count == 0)
                    {
                        result.Add(r);
                    }
                    else
                    {
                        bool flag = false;
                        for (int cnt = 0; cnt < result.Count; cnt++)
                        {
                            if (result[cnt].Number == r.Number)
                            {
                                result[cnt].Amount += r.Amount;
                                result[cnt].Price += r.Price;
                                flag = true;
                                break;
                            }
                        }
                        if (!flag) result.Add(r);
                    }
                }
            }
        }
        return result;
    }

    private GameManager.OrderList 商品別注文データ取得(GameManager.OrderDataCSVList Date, int Category, string Name)
    {
        int result = 0;
        for (int i = 0; i < 注文データ数(Date); i++)
        {
            var data = 注文データ取得(Date, i);
            foreach(var record in data)
            {
                if(record.Category==Category && record.Name == Name)
                {
                    result += record.Amount;
                } 
            }
        }

        var list = DataLoader.LoadList();
        list.Add(new DataLoader.List { Name = "割引", Category = -1 });
        int cnt = 0;
        int Price = 0;
        int Number = 0;
        foreach (var record in list)
        {
            if(record.Category == Category && record.Name == Name)
            {
                Number = cnt;
                Price = record.Price * result;
                break;
            }
            cnt++;
        }

        return new GameManager.OrderList { Amount = result, Category = Category, Number = Number, Name =  Name, Price = Price};
    }

    private int 注文総額取得(List<GameManager.OrderList> orderList)
    {
        int price = 0;
        foreach (var record in orderList)
        {
            price += record.Price;
        }
        return price;
    }

    private void 当日売上商品リストUI設定(List<GameManager.OrderList> list)
    {
        if (日付データ数 > 0)
            SetScrollView(list, 当日売上商品リスト.transform, 当日売上商品.transform);
    }

    private void 時間帯別売上商品リストUI設定(List<GameManager.OrderList> list)
    {
        if (日付データ数 > 0)
            SetScrollView(list, 時間帯別売上商品リスト.transform, 時間帯別売上商品.transform);
    }

    private void SetScrollView(List<GameManager.OrderList> list, Transform Scroll, Transform Prefabs)
    {
        int cnt = 0;
        while (true)
        {
            if (Scroll.Find(cnt.ToString()) != null)
            {
                Destroy(Scroll.Find(cnt.ToString()).gameObject);
            }
            else
            {
                break;
            }
            cnt++;
        }

        cnt = 0;
        foreach (var record in list)
        {
            var child = Instantiate(Prefabs.gameObject, Prefabs);
            child.name = cnt.ToString();
            child.transform.Find("Name").GetComponent<Text>().text = record.Name;
            child.transform.Find("Amount").GetComponent<Text>().text = record.Amount.ToString() + " 点";
            child.transform.Find("Price").GetComponent<Text>().text = Number.MarkDecimal(record.Price) + " 円";
            child.transform.SetParent(Scroll);
            child.SetActive(true);

            cnt++;
        }
    }

    public void Clicked時間帯別売上日付ボタン(int num)
    {
        if (日付データ数 > 0)
        {
            if (num == -1) //リセット
            {
                時間帯別売上日付番号 = 日付データ数 - 1;
                DateTime t = DateTime.Now;
                時間帯別売上時間番号 = int.Parse(t.ToString("HH"));
            }
            else if (num == 0) //日付 前へ
            {
                時間帯別売上日付番号 = Number.Range(時間帯別売上日付番号 - 1, 0, 日付データ数 - 1);
            }
            else if (num == 1) //日付 次へ
            {
                時間帯別売上日付番号 = Number.Range(時間帯別売上日付番号 + 1, 0, 日付データ数 - 1);
            }
            else if (num == 2) //時間 前へ
            {
                時間帯別売上時間番号 = Number.Range(時間帯別売上時間番号 - 1, 0, 23);
            }
            else if (num == 3) //時間 次へ
            {
                時間帯別売上時間番号 = Number.Range(時間帯別売上時間番号 + 1, 0, 23);
            }

            時間帯別売上日付.text = 日付リスト[時間帯別売上日付番号].Year.ToString() + "年 " + 日付リスト[時間帯別売上日付番号].Month.ToString() + "月 " + 日付リスト[時間帯別売上日付番号].Day.ToString() + "日";
            時間帯別売上時間.text = 時間帯別売上時間番号.ToString() + ":00";
            var list = 時間帯別注文データ取得(日付リスト[時間帯別売上日付番号], 時間帯別売上時間番号);
            int 売上総額 = 0;
            int 売上個数 = 0;
            時間帯別売上商品リストUI設定(list);
            foreach (var record in list)
            {
                売上総額 += record.Price;
                売上個数 += record.Amount;
            }
            時間帯別売上総売上額.text = Number.MarkDecimal(売上総額) + " 円";
            時間帯別売上個数.text = 売上個数.ToString() + " 個";
        }
    }

    public void Clicked商品別売上日付ボタン(int num)
    {
        if (日付データ数 > 0)
        {
            if (num == -2) //画面更新
            {

            }
            else if (num == -1) //リセット
            {
                商品別売上日付番号 = 日付データ数 - 1;
                DateTime t = DateTime.Now;
                商品別売上商品番号 = 0;
            }
            else if (num == 0) //日付 前へ
            {
                商品別売上日付番号 = Number.Range(商品別売上日付番号 - 1, 0, 日付データ数 - 1);
            }
            else if (num == 1) //日付 次へ
            {
                商品別売上日付番号 = Number.Range(商品別売上日付番号 + 1, 0, 日付データ数 - 1);
            }

            商品別売上日付.text = 日付リスト[商品別売上日付番号].Year.ToString() + "年 " + 日付リスト[商品別売上日付番号].Month.ToString() + "月 " + 日付リスト[商品別売上日付番号].Day.ToString() + "日";
            var list = 時間帯別注文データ取得(日付リスト[時間帯別売上日付番号], 時間帯別売上時間番号);
            var GoodsList = DataLoader.LoadList();
            GoodsList.Add(new DataLoader.List { Name = "割引", Category = -1 });
            商品別売上商品リストUI設定(GoodsList);

        }
    }

    private void 商品別売上商品リストUI設定(List<DataLoader.List> list)
    {
        int cnt = 0;
        Transform Scroll = 商品別売上商品リスト.transform;
        Transform Prefabs = 商品別売上商品.transform;

        while (true)
        {
            if (Scroll.Find(cnt.ToString()) != null)
            {
                Destroy(Scroll.Find(cnt.ToString()).gameObject);
            }
            else
            {
                break;
            }

            cnt++;
        }

         int 割引総額 = 0;
         cnt = 0;
        foreach (var record in list)
         {
            var child = Instantiate(Prefabs.gameObject, Prefabs);
            child.name = cnt.ToString();
            child.transform.Find("Name").GetComponent<Text>().text = record.Name;

            if (record.Name == "割引" && record.Category == -1)
            {
                child.transform.Find("Price").GetComponent<Text>().text = "";
            }
            else
            {
                child.transform.Find("Price").GetComponent<Text>().text = "単価 " + Number.MarkDecimal(record.Price) + " 円";
            }

            child.transform.SetParent(Scroll);
            child.SetActive(true);
            child.GetComponent<Button>().onClick.AddListener(() => { Clicked商品別売上商品ボタン(int.Parse(child.name)); });
            cnt++;
         }

        商品別売上商品名.text = list[商品別売上商品番号].Name;
        var data = 商品別注文データ取得(日付リスト[商品別売上日付番号], list[商品別売上商品番号].Category, list[商品別売上商品番号].Name);
        if (data.Name == "割引" && data.Category == -1)
        {
            for (int i = 0; i < 注文データ数(日付リスト[商品別売上日付番号]); i++)
            {
                foreach(var r in 注文データ取得(日付リスト[商品別売上日付番号], i))
                {
                    if (r.Name == "割引" && r.Category == -1)
                    {
                        割引総額 += r.Price;
                    }
                }
            }

            商品別売上売上額.text = Number.MarkDecimal(割引総額) + " 円";

        }
        else
        {
            商品別売上売上額.text = Number.MarkDecimal(data.Price) + " 円";
        }
        商品別売上個数.text = Number.MarkDecimal(data.Amount) + " 個";

    }

    public void Clicked商品別売上商品ボタン(int num)
    {
        商品別売上商品番号 = num;
        Clicked商品別売上日付ボタン(-2);
    }
}
