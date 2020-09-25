using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using CsvHelper;

public class GameManager : MonoBehaviour
{
    public static List<DataLoader.List> productList;
    public List<OrderList> orderList;
    string StoreName;
    int TabMode;
    int[] 商品数;
    int Page;
    int[] MaxPage;
    int 現金;
    int 割引額;
    bool isBarcodeReader = true;

    //GameObject
    public GameObject タブ;
    public GameObject 商品ボタン;
    public GameObject 注文履歴;
    public GameObject 注文履歴ボタンPrefab;
    public GameObject 精算画面;
    public Text お釣り;
    public GameObject 割引画面;
    public GameObject 精算終了確認;
    public GameObject Numpad;
    public Text 小計金額;
    public Button 注文ボタン;
    public Button 注文確定ボタン;
    public Button 注文キャンセルボタン;
    public Button ScrollButtonUp;
    public Button ScrollButtonDown;
    public GameObject 終了確認画面;


    public class OrderList
    {
        public string Name = "";
        public int Category = -1;
        public int Number = -1;
        public int Amount = 0;
        public int Price = 0;
    }

    public class OrderDataCSVList
    {
        public int Year;
        public int Month;
        public int Day;
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

    public class OrderDataCSVListMapper : CsvHelper.Configuration.ClassMap<OrderDataCSVList>
    {
        public OrderDataCSVListMapper()
        {
            Map(x => x.Year).Index(0);
            Map(x => x.Month).Index(1);
            Map(x => x.Day).Index(2);
        }
    }

    void Start()
    {
        productList = DataLoader.LoadList();

        orderList = new List<OrderList>();
        isBarcodeReader = GetComponent<DataLoader>().LoadConfig().BarcodeReader;
        GetComponent<BarcodeReader>().TimeOut = GetComponent<DataLoader>().LoadConfig().BarcodeReaderTimeOut;
        TabMode = 0;
        Page = 0;
        商品数 = new int[4];
        MaxPage = new int[4];
        現金 = 0;

        精算画面.SetActive(false);
        割引画面.SetActive(false);

        for (int i =0; i<4; i++)
        {
            商品数[i] = ProductList.各カテゴリーの商品数取得(productList, i);
            if (商品数[i] > 6)
            {
                MaxPage[i] = (int)Math.Ceiling((float)商品数[i] / 3f) - 2;
            }
            else
            {
                MaxPage[i] = 0;
            }
        }

        商品ボタン表示();
    }

    void Update()
    {
        //スクロールボタン
        if (Page == MaxPage[TabMode])
        {
            ScrollButtonDown.interactable = false;
        }
        else
        {
            ScrollButtonDown.interactable = true;
        }
        if (Page == 0)
        {
            ScrollButtonUp.interactable = false;
        }
        else
        {
            ScrollButtonUp.interactable = true;
        }

        //タブ
        for(int i=0; i<4; i++)
        {
            Text Tab = タブ.transform.Find((i + 1).ToString()).Find("Text").GetComponent<Text>();
            Tab.text = GetComponent<DataLoader>().LoadConfig().TabName[i];
        }

        if (精算終了確認.activeSelf == true)
        {
            注文確定ボタン.interactable = false;
            注文キャンセルボタン.interactable = false;
        }
        else
        {
            注文確定ボタン.interactable = true;
            注文キャンセルボタン.interactable = true;
        }

        if (精算画面.activeSelf==false && 精算終了確認.activeSelf==false && 終了確認画面.activeSelf==false) //メイン画面
        {
            //注文数ゼロ時の処理
            if (注文情報取得().Amount == 0)
            {
                注文ボタン.interactable = false;
            }
            else
            {
                注文ボタン.interactable = true;
            }

            if (!isBarcodeReader) {
                if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
                    注文追加(0);
                else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
                    注文追加(1);
                else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
                    注文追加(2);
                else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
                    注文追加(3);
                else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
                    注文追加(4);
                else if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
                    注文追加(5);
                else if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
                    Clicked注文ボタン();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
                終了確認画面.GetComponent<CheckWindow>().WindowAwake();
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                Clickedタブボタン(Number.Range(TabMode + 1, 0, 3));
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
                Clickedタブボタン(Number.Range(TabMode - 1, 0, 3));
            else if (Input.GetKeyDown(KeyCode.DownArrow))
                Clickedスクロールボタン(-1);
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                Clickedスクロールボタン(1);
        }
        else if (精算画面.activeSelf==true && 精算終了確認.activeSelf== false && 終了確認画面.activeSelf == false)//精算画面
        {
            注文ボタン.interactable = false;
            if (!割引画面.activeSelf)
            {
                現金 = NumPadKey.Value;
                精算画面.transform.Find("現金").Find("Text").GetComponent<Text>().text = Number.MarkDecimal(現金) + " 円";
                精算画面.transform.Find("内税").GetComponent<Text>().text = "内税 " + Number.MarkDecimal(Number.InsideTax(注文情報取得().Price - 割引額, GetComponent<DataLoader>().LoadConfig().Tax)) + " 円";
            }

            お釣り.text = Number.MarkDecimal(現金 - (注文情報取得().Price - 割引額)) + " 円";
            if(現金 - (注文情報取得().Price - 割引額) < 0)
            {
                お釣り.color = new Color(1f, 0f, 0f, 1f);
            }
            else
            {
                お釣り.color = new Color(0f, 0f, 0f, 1f);
            }

            if(現金 - (注文情報取得().Price - 割引額) >= 0)
            {
                注文確定ボタン.interactable = true;
            }
            else
            {
                注文確定ボタン.interactable = false;
            }

            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
                Numpad.GetComponent<NumPadKey>().KeyPressed("1");
            else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
                Numpad.GetComponent<NumPadKey>().KeyPressed("2");
            else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
                Numpad.GetComponent<NumPadKey>().KeyPressed("3");
            else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
                Numpad.GetComponent<NumPadKey>().KeyPressed("4");
            else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
                Numpad.GetComponent<NumPadKey>().KeyPressed("5");
            else if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
                Numpad.GetComponent<NumPadKey>().KeyPressed("6");
            else if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7))
                Numpad.GetComponent<NumPadKey>().KeyPressed("7");
            else if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8))
                Numpad.GetComponent<NumPadKey>().KeyPressed("8");
            else if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9))
                Numpad.GetComponent<NumPadKey>().KeyPressed("9");
            else if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0))
                Numpad.GetComponent<NumPadKey>().KeyPressed("0");
            else if (Input.GetKeyDown(KeyCode.Clear))
                Numpad.GetComponent<NumPadKey>().KeyPressed("Clear");
            else if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace))
                Numpad.GetComponent<NumPadKey>().KeyPressed("Del");
            else if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
                Clicked注文確定ボタン();
            else if (Input.GetKeyDown(KeyCode.Escape))
                Clicked注文キャンセルボタン();

        }
        else if (精算終了確認.activeSelf == true)
        {
            if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
                Clicked精算終了確認ボタン();
            else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace))
                Clicked精算終了確認中止ボタン();
        }
    }

    public void Clickedタブボタン(int num)
    {
        TabMode = num;
        Page = 0;
        商品ボタン表示();
    }

    public void Clickedスクロールボタン(int mode)
    {
        if (mode > 0)
        {
            if(ScrollButtonUp.interactable == true)
                Page = Page - 1;
        }
        else if(mode<0)
        {
            if (ScrollButtonDown.interactable == true)
                Page = Page + 1;
        }
        Page = Number.Range(Page, 0, MaxPage[TabMode]);
        商品ボタン表示();
    }

    public void 商品ボタン表示()
    {
        Transform[] Button = new Transform[6];
        
        for(int i = 0; i<6; i++)
        {
            Button[i] = 商品ボタン.transform.Find((i + 1).ToString());
            DataLoader.List[] ListEachCategory = ProductList.各カテゴリーのリスト取得(productList, TabMode).ToArray(); ;

            if (i + Page * 3 < ProductList.各カテゴリーの商品数取得(productList, TabMode))
            {
                Button[i].gameObject.SetActive(true);
                if (ListEachCategory[i + 3 * Page].Available)
                {
                    Button[i].GetComponent<Button>().interactable = true;
                }
                else
                {
                    Button[i].GetComponent<Button>().interactable = false;
                }

                string dir = "image/" + ListEachCategory[i+Page*3].ImagePath;
                if (GetComponent<DataLoader>().checkExist(dir))
                {
                    RawImage img = Button[i].Find("RawImage").GetComponent<RawImage>();
                    byte[] bytes = File.ReadAllBytes(dir);
                    Texture2D texture = new Texture2D(500, 500);
                    texture.LoadImage(bytes);
                    img.texture = texture;
                    img.color = new Color(1f, 1f, 1f, 1f);
                }
                else
                {
                    RawImage img = Button[i].Find("RawImage").GetComponent<RawImage>();
                    img.texture = null;
                    img.color = new Color(100f / 255f, 100f / 255f, 100f / 255f, 1f);
                }

                Button[i].Find("Text").GetComponent<Text>().text = ListEachCategory[i + 3 * Page].Name;
                Button[i].Find("Price").GetComponent<Text>().text = Number.MarkDecimal(ListEachCategory[i + 3 * Page].Price) + " 円";

            }
            else
            {
                Button[i].gameObject.SetActive(false);
            }
        }

        商品ボタン.transform.Find("Place").GetComponent<Text>().text = (Page + 1).ToString() + " / " + (MaxPage[TabMode] + 1).ToString();
    }

    public void 注文履歴一括削除()
    {
        var clones = GameObject.FindGameObjectsWithTag("OrderList");
        orderList.Clear();
        foreach (var clone in clones)
        {
            Destroy(clone);
        }
    }

    public void 注文追加(int ButtonNumber)
    {
        if (商品ボタン.transform.Find((ButtonNumber + 1).ToString()).gameObject.activeSelf) {
            int _tab = TabMode;
            int _num = ButtonNumber + 3 * Page;
            DataLoader.List[] _list = ProductList.各カテゴリーのリスト取得(productList, _tab).ToArray();
            int count = 0;
            bool flag = false;
            OrderList tempRecord = new OrderList();
            foreach (OrderList record in orderList)
            {
                if (record.Category == _tab && record.Number == _num)
                {
                    flag = true;
                    tempRecord = record;
                    break;
                }
                count++;
            }

            if (_list[_num].Available)
            {
                if (flag) //orderList内に存在
                {
                    tempRecord.Amount += 1;
                    tempRecord.Price = _list[_num].Price * tempRecord.Amount;
                    注文履歴.transform.Find(count.ToString()).Find("Amount").GetComponent<Text>().text = "数量 " + tempRecord.Amount.ToString();
                    注文履歴.transform.Find(count.ToString()).Find("Price").GetComponent<Text>().text = "￥ " + Number.MarkDecimal(_list[_num].Price * tempRecord.Amount);
                    orderList[count] = tempRecord;
                    小計金額.text = Number.MarkDecimal(注文情報取得().Price) + " 円";
                }
                else //orderList内に存在しない
                {
                    int i = 0;
                    while (注文履歴.transform.Find(i.ToString()) != false)
                    {
                        i++;
                    }
                    GameObject temp = Instantiate(注文履歴ボタンPrefab, 注文履歴.transform);
                    temp.name = i.ToString();
                    temp.transform.Find("ProductName").GetComponent<Text>().text = _list[_num].Name;
                    temp.transform.Find("Price").GetComponent<Text>().text = "￥ " + Number.MarkDecimal(_list[_num].Price);
                    temp.transform.Find("Amount").GetComponent<Text>().text = "数量 1";
                    orderList.Add(new OrderList { Name = _list[_num].Name, Category = _list[_num].Category, Number = _num, Amount = 1, Price = _list[_num].Price });
                    小計金額.text = Number.MarkDecimal(注文情報取得().Price) + " 円";
                }
            }
        }
    }

    public void 注文削除(int _indexNum)
    {
        if (orderList[_indexNum].Amount == 1)
        {
            Destroy(注文履歴.transform.Find(_indexNum.ToString()).gameObject);
            int i = _indexNum + 1;
            while (注文履歴.transform.Find(i.ToString()) != false)
            {
                注文履歴.transform.Find(i.ToString()).gameObject.name = (i - 1).ToString();
                i++;
            }
            orderList.RemoveAt(_indexNum);
            小計金額.text = Number.MarkDecimal(注文情報取得().Price) + " 円";
        }
        else if (orderList[_indexNum].Amount > 1)
        {
            OrderList tempRecord = orderList[_indexNum];
            tempRecord.Price -= tempRecord.Price / tempRecord.Amount;

            tempRecord.Amount -= 1;
            注文履歴.transform.Find(_indexNum.ToString()).Find("Amount").GetComponent<Text>().text = "数量 " + tempRecord.Amount.ToString();
            注文履歴.transform.Find(_indexNum.ToString()).Find("Price").GetComponent<Text>().text = "￥ " + Number.MarkDecimal(tempRecord.Price);
            orderList[_indexNum] = tempRecord;
            小計金額.text = Number.MarkDecimal(注文情報取得().Price) + " 円";
        }
    }

    public OrderList 注文情報取得()
    {
        OrderList res = new OrderList();
        foreach(OrderList record in orderList)
        {
            res.Price += record.Price;
            res.Amount += record.Amount;
        }
        return res;
    }

    public void Clicked注文ボタン()
    {
        if (注文情報取得().Amount > 0)
        {
            割引画面.SetActive(false);
            精算画面.SetActive(true);
            現金 = 0;
            割引額 = 0;

            精算画面.transform.Find("商品数").Find("Text").GetComponent<Text>().text = "計 " + 注文情報取得().Amount.ToString() + " 点";
            精算画面.transform.Find("現金").Find("Text").GetComponent<Text>().text = Number.MarkDecimal(現金) + " 円";
            精算画面.transform.Find("合計金額").Find("Text").GetComponent<Text>().text = Number.MarkDecimal(注文情報取得().Price) + " 円";
            精算画面.transform.Find("内税").GetComponent<Text>().text = "内税 " + Number.MarkDecimal(Number.InsideTax(注文情報取得().Price - 割引額, GetComponent<DataLoader>().LoadConfig().Tax)) + " 円";
            精算画面.transform.Find("割引").GetComponent<Text>().text = "割引 " + Number.MarkDecimal(割引額) + " 円";
            NumPadKey.数値上限設定(10000000);
            NumPadKey.対象切り替え(精算画面.transform.Find("現金").Find("Text").GetComponent<Text>());
        }
    }

    public void Clicked注文確定ボタン()
    {
        if (現金 - (注文情報取得().Price - 割引額) >= 0)
        {
            精算終了確認.SetActive(true);
            精算終了確認.transform.Find("ウィンドウ").Find("現金").Find("Text").GetComponent<Text>().text = Number.MarkDecimal(現金) + " 円";
            精算終了確認.transform.Find("ウィンドウ").Find("釣銭").Find("Text").GetComponent<Text>().text = Number.MarkDecimal(現金 - (注文情報取得().Price - 割引額)) + " 円";
        }
    }

    public void Clicked注文キャンセルボタン()
    {
        割引画面.SetActive(false);
        精算画面.SetActive(false);
    }

    public void Clicked割引ボタン()
    {
        割引画面.SetActive(true);
        NumPadKey.数値上限設定(注文情報取得().Price + 1);
        NumPadKey.対象切り替え(割引画面.transform.Find("割引額").Find("Text").GetComponent<Text>());
    }

    public void Clicked割引金額ボタン(float 割引率)
    {
        割引画面.transform.Find("割引額").Find("Text").GetComponent<Text>().text = Number.MarkDecimal((int)(注文情報取得().Price * 割引率)) + " 円";
        NumPadKey.対象切り替え(割引画面.transform.Find("割引額").Find("Text").GetComponent<Text>());
    }

    public void Clicked割引決定ボタン()
    {
        割引額 = NumPadKey.Value;
        精算画面.transform.Find("割引").GetComponent<Text>().text = "割引 " + Number.MarkDecimal(割引額) + " 円";
        精算画面.transform.Find("合計金額").Find("Text").GetComponent<Text>().text = Number.MarkDecimal(注文情報取得().Price - 割引額) + " 円";
        NumPadKey.数値上限設定(10000000);
        NumPadKey.対象切り替え(精算画面.transform.Find("現金").Find("Text").GetComponent<Text>());
        割引画面.SetActive(false);
    }

    public void Clicked精算終了確認ボタン()
    {
        DateTime time = DateTime.Now;

        int cnt = 0;
        while (true)
        {
            if (!GetComponent<DataLoader>().checkExist("data/order/" + time.ToString("yyyyMMdd") + "/" + cnt.ToString() + ".csv"))
                break;
            cnt++;
        }
        Directory.CreateDirectory(@"data/order/" + time.ToString("yyyyMMdd"));
        var path = @"data/order/"+ time.ToString("yyyyMMdd") + "/" + cnt.ToString() + ".csv";

        if (割引額 != 0)
        {
            OrderList discount = new OrderList();
            orderList.Add(discount);
            discount.Name = "割引";
            discount.Category = -1;
            discount.Number = -1;
            discount.Amount = 1;
            discount.Price = -割引額;
            SaveOrderList(path);
            orderList.Remove(discount);
        }
        else
        {
            SaveOrderList(path);
        }

        foreach (var record in orderList)
        {
            int num = record.Number;
            if(productList[num].Stock != -1)
            {
                var tempList = productList[num];
                tempList.Stock -= record.Amount;
                productList[num] = tempList;
            }
        }
        DataLoader.SaveList(productList);

        GenerateReceipt(orderList, cnt, time);
        GetComponent<DataLoader>().AddPayment(注文情報取得().Price - 割引額, "商品購入-" + time.ToString("yyyyMMdd") + cnt.ToString(), false);
        orderList.Clear();
        foreach (GameObject record in GameObject.FindGameObjectsWithTag("OrderList"))
        {
            Destroy(record);
        }

        精算終了確認.SetActive(false);
        割引画面.SetActive(false);
        精算画面.SetActive(false);
        小計金額.text = Number.MarkDecimal(注文情報取得().Price) + " 円";
    }

    public void Clicked精算終了確認中止ボタン()
    {
        精算終了確認.SetActive(false);
    }

    public void GenerateReceipt(List<OrderList> _list, int cnt, DateTime time)
    {
        GetComponent<Receipt>().注文完了レシート(_list, 注文情報取得(), 割引額, 現金, cnt);
    }

    public void SaveOrderList(string path)
    {
        GetComponent<DataLoader>().SaveOrderList(path, orderList);
    }

    public void バーコード読み取り()
    {
        string ReadedData = BarcodeReader.Data;
        foreach (var l in productList)
        {
            bool flag = false;
            int count = 0;

            if (l.ID.Replace(" ", "") == ReadedData)
            {
                OrderList tempRecord = new OrderList();
                foreach (var record in orderList)
                {
                    if (record.Number == count)
                    {
                        flag = true;
                        tempRecord = record;
                        break;
                    }
                    count++;
                }

                if (productList[count].Available)
                {
                    if (flag) //orderList内に存在
                    {
                        tempRecord.Amount += 1;
                        tempRecord.Price = productList[count].Price * tempRecord.Amount;
                        注文履歴.transform.Find(count.ToString()).Find("Amount").GetComponent<Text>().text = "数量 " + tempRecord.Amount.ToString();
                        注文履歴.transform.Find(count.ToString()).Find("Price").GetComponent<Text>().text = "￥ " + Number.MarkDecimal(productList[count].Price * tempRecord.Amount);
                        orderList[count] = tempRecord;
                        小計金額.text = Number.MarkDecimal(注文情報取得().Price) + " 円";
                    }
                    else //orderList内に存在しない
                    {
                        int i = 0;
                        while (注文履歴.transform.Find(i.ToString()) != false)
                        {
                            i++;
                        }
                        GameObject temp = Instantiate(注文履歴ボタンPrefab, 注文履歴.transform);
                        temp.name = i.ToString();
                        temp.transform.Find("ProductName").GetComponent<Text>().text = productList[count].Name;
                        temp.transform.Find("Price").GetComponent<Text>().text = "￥ " + Number.MarkDecimal(productList[count].Price);
                        temp.transform.Find("Amount").GetComponent<Text>().text = "数量 1";
                        orderList.Add(new OrderList { Name = productList[count].Name, Category = productList[count].Category, Number = count, Amount = 1, Price = productList[count].Price });
                        小計金額.text = Number.MarkDecimal(注文情報取得().Price) + " 円";
                    }
                }
            }
            count++;
        }
    }
}
