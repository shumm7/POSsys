using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

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

        public void Reset()
        {
            for (int i = 0; i < 24; i++)
            {
                TabMode[i] = -1;
                Number[i] = -1;
                Amount[i] = -1;
                CurrentStock[i] = -1;
            }
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
            Vector3 absolutePos = OrderListUI.transform.position;
            absolutePos.y = absolutePos.y - 15 - (25 * i);
            GameObject temp = Instantiate(OrderListPrefab, absolutePos, Quaternion.identity, OrderListUI.transform);
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
                OrderConfirmButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                OrderConfirmButton.GetComponent<Button>().interactable = true;
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
            OrderConfirmButton.GetComponent<Button>().interactable = false;
            OrderSubmitButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            OrderConfirmButton.GetComponent<Button>().interactable = false;
        }
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
