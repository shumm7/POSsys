using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

public class GameManager : MonoBehaviour
{
    public Text TimeUI;
    public GameObject Tabs;
    public GameObject OrderButton;
    public Text PriceUI;
    public GameObject BackButton;
    public GameObject OrderListUI;
    public GameObject OrderListPrefab;
    public GameObject OrderSubmitButton;

    public GameObject OrderConfirmUI;
    public GameObject OrderCancelButton;
    public GameObject OrderConfirmButton;
    public GameObject OrderConfirmButtonCover;
    public GameObject OrderCheckWindow;
    public Text ProductAmountUI;
    public Text CashUI;
    public Text InsideTax;
    public Text ChangeUI;

    public static DataLoader.List productList;
    public OrderList orderList;
    public int TabMode = 0;
    private int CurrentPrice = 0;

    private int Tax;
    private string StoreName;

    public class OrderList
    {
        public int[] TabMode = new int[24];
        public int[] Number = new int[24];
        public int[] Amount = new int[24];
        public int[] CurrentStock = new int[24];
        public int Price = 0;
        public int OverallAmount = 0;
        public int Cash = 0;

        public void Reset()
        {
            for (int i = 0; i < 24; i++)
            {
                TabMode[i] = -1;
                Number[i] = -1;
                Amount[i] = -1;
                CurrentStock[i] = -1;
            }
            Price = 0;
            OverallAmount = 0;
            Cash = 0;
        }

        public void Add(DataLoader.List list, int _tabmode, int _num)
        {
            if (list.Available[_tabmode][_num-1] && list.Stock[_tabmode][_num-1] != 0)
            {
                int i;
                bool flag = false;
                for (i = 0; i < 24; i++)
                {
                    if (TabMode[i] == _tabmode && Number[i] == _num)
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                {
                    if (CurrentStock[i] != 0) {
                        Amount[i]++;
                        Price += list.Price[_tabmode][_num - 1];
                        if (CurrentStock[i] != -1)
                        {
                            CurrentStock[i]--;
                        }
                        OverallAmount++;
                    }
                }
                else
                {
                    for (i = 0; i < 24; i++)
                    {
                        if (TabMode[i] == -1)
                        {
                            break;
                        }
                    }

                    if (i >= 15)
                    {
                        OrderListComponent.AddUpperNumber(1);
                    }
                    TabMode[i] = _tabmode;
                    Number[i] = _num;
                    Amount[i] = 1;
                    CurrentStock[i] = list.Stock[_tabmode][_num-1] - 1;
                    Price += list.Price[_tabmode][_num-1];
                    OverallAmount++;
                }
            }
        }

        public void Remove(DataLoader.List list, int _tabmode, int _num)
        {
            if (list.Available[_tabmode][_num-1])
            {
                int i;
                bool flag = false;
                for (i = 0; i < 24; i++)
                {
                    if (TabMode[i] == _tabmode && Number[i] == _num)
                    {
                        flag = true;
                        break;
                    }
                }

                if (flag)
                {
                    Price -= list.Price[_tabmode][_num-1];
                    OverallAmount--;
                    if (CurrentStock[i] != -1)
                    {
                        CurrentStock[i]++;
                    }

                    if (Amount[i] > 1)
                    {
                        Amount[i] -= 1;
                    }
                    else
                    {
                        int[,] temp = new int[4, 24];

                        for(int j=i+1; j<24; j++)
                        {
                            temp[0, j] = TabMode[j];
                            temp[1, j] = Number[j];
                            temp[2, j] = Amount[j];
                            temp[3, j] = CurrentStock[j];
                        }
                        for (int j = i; j < 24 - 1; j++)
                        {
                            TabMode[j] = temp[0, j+1];
                            Number[j] = temp[1, j+1];
                            Amount[j] = temp[2, j+1];
                            CurrentStock[j] = temp[3, j+1];
                        }
                        TabMode[23] = -1;
                        Number[23] = -1;
                        Amount[23] = -1;
                        CurrentStock[23] = -1;
                    }
                }
            }
        }
    }

    void Start()
    {
        CurrentPrice = 0;
        PriceUI.text = MarkDecimal(CurrentPrice) + " 円";
        orderList = new OrderList();
        orderList.Reset();

        DataLoader.Config configulation = GetComponent<DataLoader>().LoadConfig();
        StoreName = configulation.StoreName;
        Tax = configulation.Tax;

        for(int i=1; i<16; i++)
        {
            GameObject temp = Instantiate(OrderListPrefab, Vector3.zero, Quaternion.identity, OrderListUI.transform);
            temp.transform.localPosition = new Vector3(0f, -20f - ( i *30 ), 0f);
            temp.name = i.ToString();
            temp.SetActive(false);
        }
        
        productList = GetComponent<DataLoader>().LoadList();
        for(int i = 1; i<=4; i++)
        {
            Tabs.transform.Find(i.ToString()).transform.Find("Text").GetComponent<Text>().text = productList.CategoryName[i - 1];
            Tabs.transform.Find(i.ToString()).gameObject.GetComponent<Button>().interactable = true;
        }
        Tabs.transform.Find("1").gameObject.GetComponent<Button>().interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        DateTime Time = DateTime.Now;
        TimeUI.text = LintNumber(Time.Month) + "月" + LintNumber(Time.Day) + "日　" + LintNumber(Time.Hour) + "：" + LintNumber(Time.Minute);

        if (orderList.OverallAmount > 0)
        {
            OrderSubmitButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            OrderSubmitButton.GetComponent<Button>().interactable = false;
        }

        if (!OrderConfirmUI.activeSelf && !OrderCheckWindow.activeSelf)
        {

            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
                AddOrder(TabMode, 1);
            else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
                AddOrder(TabMode, 2);
            else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
                AddOrder(TabMode, 3);
            else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
                AddOrder(TabMode, 4);
            else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
                AddOrder(TabMode, 5);
            else if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
                AddOrder(TabMode, 6);
            else if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
                OrderButtonClicked();
            else if (Input.GetKeyDown(KeyCode.Escape))
                BackButton.GetComponent<CheckWindow>().WindowAwake();
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                TabButtonClicked(Range(TabMode+1, 0, 3));
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
                TabButtonClicked(Range(TabMode-1, 0, 3));


        }

        if (OrderConfirmUI.activeSelf && !OrderCheckWindow.activeSelf)
        {
            if(int.Parse(CashUI.text.Substring(0, CashUI.text.Length - 2).Replace(",", "")) - orderList.Price < 0)
            {
                OrderConfirmButtonCover.SetActive(true);
            }
            else
            {
                OrderConfirmButtonCover.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
                GetComponent<NumPadKey>().KeyPressed("1");
            else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
                GetComponent<NumPadKey>().KeyPressed("2");
            else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
                GetComponent<NumPadKey>().KeyPressed("3");
            else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
                GetComponent<NumPadKey>().KeyPressed("4");
            else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
                GetComponent<NumPadKey>().KeyPressed("5");
            else if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
                GetComponent<NumPadKey>().KeyPressed("6");
            else if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7))
                GetComponent<NumPadKey>().KeyPressed("7");
            else if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8))
                GetComponent<NumPadKey>().KeyPressed("8");
            else if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9))
                GetComponent<NumPadKey>().KeyPressed("9");
            else if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0))
                GetComponent<NumPadKey>().KeyPressed("0");
            else if (Input.GetKeyDown(KeyCode.Clear))
                GetComponent<NumPadKey>().KeyPressed("Clear");
            else if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace))
                GetComponent<NumPadKey>().KeyPressed("Del");
            else if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
                OrderConfirmButtonClicked();
            else if (Input.GetKeyDown(KeyCode.Escape))
                OrderCancelButtonClicked();
        }
    }

    private string LintNumber(int num)
    {
        if (num < 10)
        {
            return "0" + num.ToString();
        }
        else
        {
            return num.ToString();
        }
    }

    public void TabButtonClicked(int num)
    {
        TabMode = num;
        for (int i = 0; i < 6; i++)
        {
            OrderButton.transform.Find((i+1).ToString()).gameObject.GetComponent<OrderButton>().TabChanged(TabMode, i + 1);
        }
        for(int i=0; i<4; i++)
        {
            Tabs.transform.Find((i+1).ToString()).gameObject.GetComponent<Button>().interactable = true;
        }
        Tabs.transform.Find((num+1).ToString()).gameObject.GetComponent<Button>().interactable = false;
    }

    public void AddOrder(int _TabMode, int num)
    {
        if (!OrderConfirmUI.activeSelf && !OrderCheckWindow.activeSelf)
        {
            orderList.Add(productList, _TabMode, num);
            GetComponent<OrderListComponent>().UpdateList(orderList);
            PriceUI.text = MarkDecimal(orderList.Price) + " 円";
        }
    }

    public void RemoveOrder(int _TabMode, int num)
    {
        if (!OrderConfirmUI.activeSelf && !OrderCheckWindow.activeSelf)
        {
            orderList.Remove(productList, _TabMode, num);
            GetComponent<OrderListComponent>().UpdateList(orderList);
            PriceUI.text = MarkDecimal(orderList.Price) + " 円";
        }
    }

    public void UpButtonClicked()
    {
        OrderListComponent.UpperNumber -= 1;
        GetComponent<OrderListComponent>().UpdateList(orderList);
    }

    public void DownButtonClicked()
    {
        OrderListComponent.UpperNumber += 1;
        GetComponent<OrderListComponent>().UpdateList(orderList);
    }

    public void OrderButtonClicked()
    {
        OrderConfirmUI.SetActive(true);
        OrderSubmitButton.GetComponent<Button>().interactable = false;
        ProductAmountUI.text = "計 " + orderList.OverallAmount.ToString() +"点";
        InsideTax.text = "内税 " + (orderList.Price * Tax / (Tax+100)).ToString() +" 円";
    }

    public void OrderCancelButtonClicked()
    {
        OrderCancelButton.transform.parent.Find("現金").Find("Text").GetComponent<Text>().text = "0 円";
        OrderCancelButton.transform.parent.Find("釣銭").Find("Text").GetComponent<Text>().text = "0 円";
        OrderConfirmUI.SetActive(false);
        OrderSubmitButton.GetComponent<Button>().interactable = true;

    }

    public void OrderConfirmButtonClicked()
    {
        if ((int.Parse(CashUI.text.Substring(0, CashUI.text.Length - 2).Replace(",", "")) - orderList.Price >= 0)){
            OrderCheckWindow.SetActive(true);
            OrderConfirmButtonCover.SetActive(true);
            OrderSubmitButton.GetComponent<Button>().interactable = true;
            OrderCheckWindow.transform.Find("ウィンドウ").Find("現金").Find("Text").GetComponent<Text>().text = CashUI.text;
            OrderCheckWindow.transform.Find("ウィンドウ").Find("釣銭").Find("Text").GetComponent<Text>().text = ChangeUI.text;

        }
        else
        {
            OrderConfirmButtonCover.SetActive(true);
        }
    }

    public void CompleteOrder()
    {
        
        orderList.Cash = int.Parse(CashUI.text.Substring(0, CashUI.text.Length - 2).Replace(",", ""));
        DateTime time = DateTime.Now;
        Directory.CreateDirectory("data/receipt/logs/" + time.ToString("yyyyMMdd"));
        int cnt = 1;
        while (true)
        {
            if (!GetComponent<DataLoader>().checkExist(@"data/receipt/logs/" + time.ToString("yyyyMMdd") + "/" + cnt.ToString() + ".json"))
            {
                break;
            }
            cnt++;
        }
        GenerateReceipt(orderList, cnt, time);

        for (int i = 0; i < 24; i++)
        {
            if (orderList.Number[i] != -1)
            {
                int _tabmode = orderList.TabMode[i];
                int _num = orderList.Number[i] - 1;
                productList.Sales[_tabmode][_num] += orderList.Amount[i];
                if (productList.Stock[_tabmode][_num] != -1)
                {
                    productList.Stock[_tabmode][_num] -= orderList.Amount[i];
                }
                GetComponent<DataLoader>().SaveList(productList);
            }
        }
        GetComponent<DataLoader>().AddPayment(orderList.Price, "商品購入-" + time.ToString("yyyyMMdd")+cnt, false);

        orderList.Reset();
        GetComponent<OrderListComponent>().UpdateList(orderList);
        PriceUI.text = MarkDecimal(orderList.Price) + " 円";

        CashUI.text = "0 円";
        ChangeUI.text = "0 円";
        OrderCheckWindow.SetActive(false);
        OrderConfirmUI.SetActive(false);
    }

    public void GenerateReceipt(OrderList _list, int cnt, DateTime time)
    {
        DataLoader c = GetComponent<DataLoader>();
        
        string FilenameJson = "data/receipt/logs/" + time.ToString("yyyyMMdd") +"/"+cnt.ToString()+ ".json";

        c.saveFile(@FilenameJson, JsonMapper.ToJson(_list));

        List<string> Text = new List<string>();
        Text.Add(StoreName);
        Text.Add("");
        Text.Add(time.ToString("yyyy年MM月dd日 HH時mm分ss秒"));
        Text.Add("-----------------------------");
        for(int i=0; i<24; i++)
        {
            if (_list.Number[i] != -1 )
            {
                int _tabmode = _list.TabMode[i];
                int _num = _list.Number[i] - 1;
                Text.Add(productList.ProductName[_tabmode][_num]+ new String('　', 12- productList.ProductName[_tabmode][_num].Length)+"￥"+ BiggerNumberStr(MarkDecimal(productList.Price[_tabmode][_num]*orderList.Amount[i])));
                Text.Add("　　　￥" +productList.Price[_tabmode][_num] + "　　　@" + BiggerNumberStr(MarkDecimal(orderList.Amount[i])));
            }
        }
        Text.Add("小　計／" + BiggerNumber(orderList.OverallAmount) + "点"+ new String('　', 5+ orderList.OverallAmount.ToString().Length) + "￥" + BiggerNumberStr(MarkDecimal(orderList.Price)));
        Text.Add("");
        Text.Add("-----------------------------");
        Text.Add("合　計   ￥" + BiggerNumberStr(MarkDecimal(orderList.Price)));
        Text.Add("内　税   ￥" + BiggerNumberStr(MarkDecimal(orderList.Price * Tax / (100 + Tax))));
        Text.Add("お預り   ￥" + BiggerNumberStr(CashUI.text.Substring(0, CashUI.text.Length - 2)));
        Text.Add("お釣り   ￥" + BiggerNumberStr(ChangeUI.text.Substring(0, ChangeUI.text.Length - 2)));
        Text.Add("");
        Text.Add("上記正に領収いたしました");
        Text.Add("   伝票番号 " + time.ToString("yyyyMMdd_HHmmss")+"(" + cnt.ToString() + ")");
        Text.Add("");
        Text.Add("-----------------------------");
        Text.Add("お買い上げ、ありがとうございました。");
        Text.Add("またのご来店をお待ちしております。");

        GetComponent<Receipt>().GenerateReceipt(Text, GetComponent<DataLoader>().LoadConfig().LINENotifyPurchaseNotice);
    }

    public string MarkDecimal(int _price)
    {
        if (_price < 1000)
        {
            return _price.ToString();
        }
        else
        {
            string text = _price.ToString();
            string ret = "";
            int count = 0;
            for(int i=0; i<text.Length; i++)
            {
                ret = text.Substring(text.Length - i - 1, 1) + ret;
                count++;
                if (count == 3 && i!=text.Length -1)
                {
                    ret = "," + ret;
                    count = 0;
                }
            }

            return ret;
        }
    }

    public string BiggerNumber(int num)
    {
        /*
        string ret = "";
        for(int i=0; i<num.ToString().Length; i++)
        {
            switch(num.ToString().Substring(i, 1))
            {
                case "0":
                    ret += "０";
                    break;
                case "1":
                    ret += "１";
                    break;
                case "2":
                    ret += "２";
                    break;
                case "3":
                    ret += "３";
                    break;
                case "4":
                    ret += "４";
                    break;
                case "5":
                    ret += "５";
                    break;
                case "6":
                    ret += "６";
                    break;
                case "7":
                    ret += "７";
                    break;
                case "8":
                    ret += "８";
                    break;
                case "9":
                    ret += "９";
                    break;
            }
        }
        return ret;
        */
        return num.ToString();
    }

    public string BiggerNumberStr(string num)
    {
        /*
        string ret = "";
        for (int i = 0; i < num.Length; i++)
        {
            switch (num.Substring(i, 1))
            {
                case "0":
                    ret += "０";
                    break;
                case "1":
                    ret += "１";
                    break;
                case "2":
                    ret += "２";
                    break;
                case "3":
                    ret += "３";
                    break;
                case "4":
                    ret += "４";
                    break;
                case "5":
                    ret += "５";
                    break;
                case "6":
                    ret += "６";
                    break;
                case "7":
                    ret += "７";
                    break;
                case "8":
                    ret += "８";
                    break;
                case "9":
                    ret += "９";
                    break;
                case ",":
                    ret += "，";
                    break;
                default:
                    ret += num.Substring(i, 1);
                    break;
            }
        }
        return ret;
        */
        return num;
    }

    private int Range(int num, int min, int max)
    {
        if (num > max)
        {
            num = min;
        }else if (num<min)
        {
            num = max;
        }
        return num;
    }
}
