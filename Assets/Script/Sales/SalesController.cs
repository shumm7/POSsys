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
    List<OrderDataCSVList> 日付リスト;
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
    public GameObject Graph;

    public class OrderDataCSVList
    {
        public int Year;
        public int Month;
        public int Day;
    }
    public class OrderDataCSVListMapper : CsvHelper.Configuration.ClassMap<OrderDataCSVList>
    {
        public OrderDataCSVListMapper()
        {
            Map(x => x.Year).Index(0);
            Map(x => x.Month).Index(1);
            Map(x => x.Day).Index(2);
        }
    }

    public class OrderList
    {
        public string Name = "";
        public int Category = -1;
        public int Number = -1;
        public int Amount = 0;
        public int Price = 0;
    }

    public class OrderListMapper : CsvHelper.Configuration.ClassMap<OrderList>
    {
        public OrderListMapper()
        {
            Map(x => x.Name).Index(0);
            Map(x => x.Category).Index(1);
            Map(x => x.Number).Index(2);
            Map(x => x.Amount).Index(3);
            Map(x => x.Price).Index(4);
        }
    }



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

    public void Clicked小ボタン(int num){
        SetUI(num);
    }

    public void Clicked当日売上番号ボタン(int num)
    {
        if (num == -1)
        {
            当日売上日付番号 = 日付データ数 - 1;
            当日売上注文ID番号 = 注文データ数(日付リスト[当日売上日付番号]) - 1;
        }
        else if(num == 0)
        {
            当日売上日付番号 = Number.Range(当日売上日付番号 - 1, 0, 日付データ数 - 1);
            当日売上注文ID番号 = 注文データ数(日付リスト[当日売上日付番号]) - 1;
        }
        else if (num == 1)
        {
            当日売上日付番号 = Number.Range(当日売上日付番号 + 1, 0, 日付データ数 - 1);
            当日売上注文ID番号 = 注文データ数(日付リスト[当日売上日付番号]) - 1;
        }
        else if (num == 2)
        {
            当日売上注文ID番号 = Number.Range(当日売上注文ID番号 - 1, 0, 注文データ数(日付リスト[当日売上日付番号]) - 1);
        }
        else if (num == 3)
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

    private void SetUI(int num)
    {
        if (num == 0) //当日売上
        {
            UI[0].SetActive(true);
            UI[1].SetActive(false);
            UI[2].SetActive(false);
            UI[3].SetActive(false);
            Clicked当日売上番号ボタン(-1);
        }
        else if (num == 1) //時間帯別売上
        {
            UI[0].SetActive(false);
            UI[1].SetActive(true);
            UI[2].SetActive(false);
            UI[3].SetActive(false);
            List<Graph.GraphData> data = new List<Graph.GraphData>();
            data.Add(new Graph.GraphData {Value=3});
            data.Add(new Graph.GraphData { Value = 1 });
            data.Add(new Graph.GraphData { Value = 6 });

            Graph.GetComponent<Graph>().GenerateGraph(data);
        }
        else if (num == 2) //期間内売上
        {
            UI[0].SetActive(false);
            UI[1].SetActive(false);
            UI[2].SetActive(true);
            UI[3].SetActive(false);
        }
        else if (num == 3) //商品別売上
        {
            UI[0].SetActive(false);
            UI[1].SetActive(false);
            UI[2].SetActive(false);
            UI[3].SetActive(true);
        }
    }

    private List<OrderDataCSVList> 日付データ取得(){
        var r = new List<OrderDataCSVList>();
            if (GetComponent<DataLoader>().checkExist("data/order/order.csv"))
            {
                var result = new List<OrderDataCSVList>();
                {
                    using (TextReader fileReader = File.OpenText(@"data/order/order.csv"))
                    {
                        using (var csv = new CsvReader(fileReader, System.Globalization.CultureInfo.InvariantCulture))
                        {
                            csv.Configuration.HasHeaderRecord = true;
                            csv.Configuration.RegisterClassMap<OrderDataCSVListMapper>();
                            csv.Read();
                            csv.ReadHeader();
                            while (csv.Read())
                            {
                                var record = new OrderDataCSVList
                                {
                                    Year = int.Parse(csv.GetField("Year")),
                                    Month = int.Parse(csv.GetField("Month")),
                                    Day = int.Parse(csv.GetField("Day"))
                                };
                                
                                if(注文データ数(record) != 0)
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

    private OrderDataCSVList 最新日付データ取得(){
        return 日付データ取得()[0];
    }

    private int 注文データ数(OrderDataCSVList Date)
    {
        string dir = Number.FormatDate(Date.Year, Date.Month, Date.Day);
        if(File.Exists(@"data/order/" + dir + "/0.csv"))
        {
            int cnt = 0;
            while (true)
            {
                if(!File.Exists(@"data/order/" + dir + "/" + cnt +".csv"))
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

    private List<OrderList> 注文データ取得(OrderDataCSVList Date, int num)
    {
        string path = "data/order/" + Number.FormatDate(Date.Year, Date.Month, Date.Day) + "/" + num.ToString() + ".csv";
        var result = new List<OrderList>();
        if (GetComponent<DataLoader>().checkExist(path))
        {
            using (TextReader fileReader = File.OpenText(@path))
            {
                using (var csv = new CsvReader(fileReader, System.Globalization.CultureInfo.InvariantCulture))
                {
                    csv.Configuration.HasHeaderRecord = true;
                    csv.Configuration.RegisterClassMap<OrderListMapper>();
                    csv.Read();
                    csv.ReadHeader();
                    while (csv.Read())
                    {
                        var record = new OrderList
                        {
                            Name = csv.GetField("Name"),
                            Category = int.Parse(csv.GetField("Category")),
                            Number = int.Parse(csv.GetField("Number")),
                            Amount = int.Parse(csv.GetField("Amount")),
                            Price = int.Parse(csv.GetField("Price"))
                        };

                        result.Add(record);
                    }
                    fileReader.Close();
                }
            }
        }

        return result;
    }

    private int 注文総額取得(List<OrderList> orderList)
    {
        int price = 0;
        foreach(var record in orderList)
        {
            price += record.Price;
        }
        return price;
    }

    private void 当日売上商品リストUI設定(List<OrderList> list)
    {
        int cnt = 0;
        while (true)
        {
            if (当日売上商品リスト.transform.Find(cnt.ToString())!=null)
            {
                Destroy(当日売上商品リスト.transform.Find(cnt.ToString()).gameObject);
            }
            else
            {
                break;
            }
            cnt++;
        }

        cnt = 0;
        foreach(var record in list)
        {
            var child = Instantiate(当日売上商品, 当日売上商品.transform);
            child.name = cnt.ToString();
            child.transform.Find("Name").GetComponent<Text>().text = record.Name;
            child.transform.Find("Amount").GetComponent<Text>().text = record.Amount.ToString() + " 点";
            child.transform.Find("Price").GetComponent<Text>().text = Number.MarkDecimal(record.Price) + " 円";
            child.transform.SetParent(当日売上商品リスト.transform);
            child.SetActive(true);

            cnt++;
        }
    }
}
