using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using LitJson;



public class CheckController : MonoBehaviour
{
    private int[] note;

    public int ButtonMode;
    public GameObject CancelUI;
    public GameObject OpeningUI;
    public GameObject CheckUI;
    public GameObject DepositUI;

    //払い戻し
    public Text DateUI;
    public Text DescriptionUI;
    public Text PaymentUI;
    public Text BeforeUI;
    public Text AfterUI;
    public Text IDUI;
    public Button PaymentCancelUI;
    private int ID;
    private int maximumID;

    //開店前入金
    public Text OpeningUIText;

    //レジチェック
    public Text CheckUIError;
    public Text CheckUIInput;
    public Text CheckUIRegisterMoney;

    void Start()
    {
        ButtonMode = 0;
        setUI();

        int count = 0;
        Directory.CreateDirectory("data/payment");
        while (true)
        {
            if (!GetComponent<DataLoader>().checkExist("data/payment/" + count.ToString() + ".json"))
            {
                break;
            }
            count++;
        }
        if(count == 0)
        {
            ID = 0;
            maximumID = 0;
        }
        else
        {
            ID = count - 1;
            maximumID = ID;
        }

        note = new int[10] { 10000, 5000, 2000, 1000, 500, 100, 50, 10, 5, 1 };
    }

    void Update()
    {
        if (ButtonMode == 0)
        {
            if(GetComponent<DataLoader>().checkExist("data/payment/" + ID.ToString() + ".json"))
            {
                DataLoader.Payment temp = GetComponent<DataLoader>().LoadPayment(ID);
                DateUI.text = temp.Time.ToString("yyyy/MM/dd hh:mm:ss");
                DescriptionUI.text = temp.Description;
                PaymentUI.text = temp.Increase.ToString();
                BeforeUI.text = temp.Before.ToString();
                AfterUI.text = temp.After.ToString();
                IDUI.text = ID.ToString();
                if (DescriptionUI.text.Substring(0,4)=="取り消し")
                {
                    PaymentCancelUI.interactable = false;
                }
                else
                {
                    PaymentCancelUI.interactable = true;
                }
            }
            else
            {
                DateUI.text = "";
                DescriptionUI.text = "データがありません";
                PaymentUI.text = "";
                BeforeUI.text = "";
                AfterUI.text = "";
                IDUI.text = ID.ToString();
                PaymentCancelUI.interactable = false;
            }
        }
        else if (ButtonMode == 1)
        {
            int money = 0;

            for (int i = 0; i <= 9; i++)
            {
                if (OpeningUI.transform.Find(i.ToString()).GetComponent<InputField>().text != "")
                {
                    money += (note[i] * int.Parse(OpeningUI.transform.Find(i.ToString()).GetComponent<InputField>().text));
                }
                else
                {
                    money += 0;
                }
            }

            OpeningUIText.text = MarkDecimal(money) + " 円";
        }

        else if (ButtonMode == 2)
        {
            int money = 0;

            for (int i = 0; i <= 9; i++)
            {
                if (CheckUI.transform.Find(i.ToString()).GetComponent<InputField>().text != "")
                {
                    money += (note[i] * int.Parse(CheckUI.transform.Find(i.ToString()).GetComponent<InputField>().text));
                }
                else
                {
                    money += 0;
                }
            }

            CheckUIInput.text = MarkDecimal(money) + " 円";
            CheckUIRegisterMoney.text = MarkDecimal(GetComponent<DataLoader>().LoadLeastPayment().After) + " 円";
            string plus = "";
            if(money - GetComponent<DataLoader>().LoadLeastPayment().After > 0)
            {
                plus = "+";
            }
            CheckUIError.text = plus + MarkDecimal(money - GetComponent<DataLoader>().LoadLeastPayment().After).ToString() + " 円";
        }
    }

    public void setUI()
    {
        CancelUI.SetActive(false);
        OpeningUI.SetActive(false);
        CheckUI.SetActive(false);
        DepositUI.SetActive(false);

        switch (ButtonMode)
        {
            case 0:
                CancelUI.SetActive(true);
                break;
            case 1:
                OpeningUI.SetActive(true);
                for (int i = 0; i <= 9; i++)
                {
                    OpeningUI.transform.Find(i.ToString()).GetComponent<InputField>().text = "";
                }
                break;
            case 2:
                CheckUI.SetActive(true);
                for (int i = 0; i <= 9; i++)
                {
                    CheckUI.transform.Find(i.ToString()).GetComponent<InputField>().text = "";
                }
                int count = 0;
                while (true)
                {
                    if (!GetComponent<DataLoader>().checkExist("data/payment/" + count.ToString() + ".json"))
                    {
                        break;
                    }
                    count++;
                }
                CheckUIInput.text = "0 円";
                CheckUIError.text = "0 円";
                CheckUIRegisterMoney.text = GetComponent<DataLoader>().LoadLeastPayment().After.ToString() + " 円";

                break;
            case 3:
                DepositUI.SetActive(true);
                break;
        }
    }

    public void ButtonPressed(int ButtonNumber)
    {
        ButtonMode = ButtonNumber;
        setUI();
    }

    public void setRegisterMoney()
    {
        int BeforeCash = GetComponent<DataLoader>().LoadLeastPayment().After;
        int money = int.Parse(OpeningUIText.text.Substring(0, OpeningUIText.text.Length - 2).Replace(",", ""));
        GetComponent<DataLoader>().AddPayment(money, "開店前レジ金設定", true);

        List<string> Text = new List<string>();
        DateTime time = DateTime.Now;
        Text.Add(GetComponent<DataLoader>().LoadConfig().StoreName);
        Text.Add("");
        Text.Add(time.ToString("yyyy年MM月dd日 HH時mm分ss秒"));
        Text.Add("-----------------------------");
        Text.Add("レジ金設定");
        Text.Add("設定額　　　" + MarkDecimal(money) + " 円");
        Text.Add("設定前　　　" + MarkDecimal(BeforeCash) + " 円");
        Text.Add("");
        Text.Add("-----------------------------");
        for(int i=0; i<10; i++)
        {
            string temp = OpeningUI.transform.Find(i.ToString()).GetComponent<InputField>().text;
            if (temp != "") {
                Text.Add(note[i].ToString() + "円 @ " + temp);
            }
            else
            {
                Text.Add(note[i].ToString() + "円 @ " + "0");
            }
        }

        GetComponent<Receipt>().GenerateReceipt(Text, true);
    }

    public void checkRegisterMoney()
    {
        int money = int.Parse(CheckUIInput.text.Substring(0, CheckUIInput.text.Length - 2).Replace(",", ""));

        List<string> Text = new List<string>();
        DateTime time = DateTime.Now;
        Text.Add(GetComponent<DataLoader>().LoadConfig().StoreName);
        Text.Add("");
        Text.Add(time.ToString("yyyy年MM月dd日 HH時mm分ss秒"));
        Text.Add("-----------------------------");
        Text.Add("レジ金確認");
        Text.Add("入力額　　　　" + MarkDecimal(money) + " 円");
        Text.Add("レジ内金額　　" + MarkDecimal(GetComponent<DataLoader>().LoadLeastPayment().After) + " 円");
        Text.Add("");
        Text.Add("誤差額　　　　" + MarkDecimal(money - GetComponent<DataLoader>().LoadLeastPayment().After) + " 円");
        Text.Add("");
        Text.Add("-----------------------------");

        for (int i = 0; i < 10; i++)
        {
            string temp = CheckUI.transform.Find(i.ToString()).GetComponent<InputField>().text;
            if (temp != "")
            {
                Text.Add(note[i].ToString() + "円 @ " + temp);
            }
            else
            {
                Text.Add(note[i].ToString() + "円 @ " + "0");
            }
        }

        GetComponent<Receipt>().GenerateReceipt(Text, true);
    }

    public void AddPaymentID(int num)
    {
        ID += num;
        if(ID > maximumID)
        {
            ID = 0;
        }
        else if (ID < 0)
        {
            ID = maximumID;
        }
    }

    public void CancelPayment()
    {
        if (DescriptionUI.text.Substring(0, 4) == "取り消し")
        {
            
        }
        else if(DescriptionUI.text.Substring(0, 4) == "商品購入")
        {
            DataLoader.Payment temp = GetComponent<DataLoader>().LoadPayment(ID);
            GetComponent<DataLoader>().AddPayment(-temp.Increase, "払い戻し-" + temp.Description, false);
            
            List<string> Text = new List<string>();
            DateTime time = DateTime.Now;
            Text.Add(GetComponent<DataLoader>().LoadConfig().StoreName);
            Text.Add("");
            Text.Add(time.ToString("yyyy年MM月dd日 HH時mm分ss秒"));
            Text.Add("-----------------------------");
            Text.Add("会計取り消し");
            Text.Add("払い戻し額　　" + MarkDecimal(temp.Increase) + " 円");
            Text.Add("取引内容　　　" + temp.Description);
            Text.Add("");
            Text.Add("レジ内金額　　" + MarkDecimal(GetComponent<DataLoader>().LoadLeastPayment().After) + " 円");
            Text.Add("");
            Text.Add("-----------------------------");
            GetComponent<Receipt>().GenerateReceipt(Text, true);

            string date = temp.Description.Substring(5, 8);
            string number = temp.Description.Substring(13);

            temp.Description = "取り消し-" + temp.Description;
            Directory.CreateDirectory("data/payment");
            GetComponent<DataLoader>().saveFile("data/payment/" + ID.ToString() + ".json", JsonMapper.ToJson(temp));

            GameManager.OrderList list = GetComponent<DataLoader>().LoadOrderList(date, int.Parse(number));
            DataLoader.List stockList = GetComponent<DataLoader>().LoadList();
            for (int i=0; i<24; i++)
            {
                if(list.TabMode[i]!=-1 && list.Number[i] != -1)
                {
                    stockList.Stock[list.TabMode[i]][list.Number[i] - 1] += list.Amount[i];
                    stockList.Sales[list.TabMode[i]][list.Number[i] - 1] -= list.Amount[i];
                }
            }
            GetComponent<DataLoader>().SaveList(stockList);
        }
        else
        {
            DataLoader.Payment temp = GetComponent<DataLoader>().LoadPayment(ID);
            GetComponent<DataLoader>().AddPayment(-temp.Increase, "払い戻し-" + temp.Description, false);

            List<string> Text = new List<string>();
            DateTime time = DateTime.Now;
            Text.Add(GetComponent<DataLoader>().LoadConfig().StoreName);
            Text.Add("");
            Text.Add(time.ToString("yyyy年MM月dd日 HH時mm分ss秒"));
            Text.Add("-----------------------------");
            Text.Add("会計取り消し");
            Text.Add("払い戻し額　　" + MarkDecimal(temp.Increase) + " 円");
            Text.Add("取引内容　　　" + temp.Description);
            Text.Add("");
            Text.Add("レジ内金額　　" + MarkDecimal(GetComponent<DataLoader>().LoadLeastPayment().After) + " 円");
            Text.Add("");
            Text.Add("-----------------------------");
            GetComponent<Receipt>().GenerateReceipt(Text, true);

            temp.Description = "取り消し-" + temp.Description;
            Directory.CreateDirectory("data/payment");
            GetComponent<DataLoader>().saveFile("data/payment/" + ID.ToString() + ".json", JsonMapper.ToJson(temp));
        }
    }

    public string MarkDecimal(int _price)
    {
        if (_price < 1000 && _price>=0)
        {
            return _price.ToString();
        }
        else
        {
            bool flag = false;
            if (_price < 0)
            {
                _price *= -1;
                flag = true;
            }
            string text = _price.ToString();
            string ret = "";
            int count = 0;

            for (int i = 0; i < text.Length; i++)
            {
                ret = text.Substring(text.Length - i - 1, 1) + ret;
                count++;
                if (count == 3 && i != text.Length - 1)
                {
                    ret = "," + ret;
                    count = 0;
                }
            }

            if (flag)
            {
                return "-" + ret;
            }
            else
            {
                return ret;
            }
        }
    }

}
