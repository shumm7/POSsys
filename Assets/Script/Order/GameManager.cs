using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public Text TimeUI;
    public GameObject Tabs;
    public GameObject OrderButton;
    public Text PriceUI;
    public GameObject OrderListUI;
    public GameObject OrderListPrefab;
    public static DataLoader.List productList;
    public OrderList orderList;
    public int TabMode = 0;
    private int CurrentPrice = 0;

    public class OrderList
    {
        public int[] TabMode = new int[24];
        public int[] Number = new int[24];
        public int[] Amount = new int[24];
        public int[] CurrentStock = new int[24];
        public int Price = 0;

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
                        Price += list.Price[_tabmode][_num-1];
                        if (CurrentStock[i] != -1)
                        {
                            CurrentStock[i]--;
                        }
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
                    TabMode[i] = _tabmode;
                    Number[i] = _num;
                    Amount[i] = 1;
                    CurrentStock[i] = list.Stock[_tabmode][_num-1] - 1;
                    Price += list.Price[_tabmode][_num-1];
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
        orderList.Add(productList, _TabMode, num);
        GetComponent<OrderListComponent>().UpdateList(orderList);
        PriceUI.text = MarkDecimal(orderList.Price) + " 円";
    }

    public void RemoveOrder(int _TabMode, int num)
    {
        orderList.Remove(productList, _TabMode, num);
        GetComponent<OrderListComponent>().UpdateList(orderList);
        PriceUI.text = MarkDecimal(orderList.Price) + " 円";
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
}
