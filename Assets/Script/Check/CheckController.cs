using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using LitJson;
using CsvHelper;



public class CheckController : MonoBehaviour
{
    private int[] note;
    private InputField[,] MoneyFields;

    public int ButtonMode;
    public GameObject CancelUI;
    public GameObject OpeningUI;
    public GameObject CheckUI;
    public GameObject DepositUI;
    public GameObject Numpad;

    //払い戻し
    public Text DateUI;
    public Text DescriptionUI;
    public Text PaymentUI;
    public Text BeforeUI;
    public Text AfterUI;
    public Text IDUI;
    public Button PaymentCancelUI;
    public InputField 入出金金額;
    public InputField 入出金内容;
    public Button 入金ボタン;
    public Button 出金ボタン;

    private int ID;
    private int maximumID;

    //開店前入金
    public Text OpeningUIText;

    //レジチェック
    public Text CheckUIError;
    public Text CheckUIInput;
    public Text CheckUIRegisterMoney;

    //入出金
    public Text DepositDescriptionUI;
    public Text DepositMoneyUI;

    void Start()
    {
        ButtonMode = 0;
        setUI();

        MoneyFields = new InputField[2, 10];
        for(int i=0; i<10; i++)
        {
            MoneyFields[0, i] = OpeningUI.transform.Find(i.ToString()).GetComponent<InputField>();
            MoneyFields[1, i] = CheckUI.transform.Find(i.ToString()).GetComponent<InputField>();
        }

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
            Numpad.SetActive(false);

            if(GetComponent<DataLoader>().checkExist("data/payment/" + ID.ToString() + ".json"))
            {
                DataLoader.Payment temp = GetComponent<DataLoader>().LoadPayment(ID);
                DateUI.text = temp.Time.ToString("yyyy/MM/dd hh:mm:ss");
                DescriptionUI.text = temp.Description;
                PaymentUI.text = Number.MarkDecimal(temp.Increase) + " 円";
                BeforeUI.text = Number.MarkDecimal(temp.Before) + " 円";
                AfterUI.text = Number.MarkDecimal(temp.After) + " 円";
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
            Numpad.SetActive(true);
            for(int i=0; i<10; i++)
            {
                if (MoneyFields[0,i].isFocused)
                {
                    金額入力(MoneyFields[0, i], 1000);
                }
            }

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

            OpeningUIText.text = Number.MarkDecimal(money) + " 円";
        }

        else if (ButtonMode == 2)
        {
            int money = 0;
            Numpad.SetActive(true);
            for (int i = 0; i < 10; i++)
            {
                if (MoneyFields[1, i].isFocused)
                {
                    金額入力(MoneyFields[1, i], 1000);
                }
            }

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

            CheckUIInput.text = Number.MarkDecimal(money) + " 円";
            CheckUIRegisterMoney.text = Number.MarkDecimal(GetComponent<DataLoader>().LoadLeastPayment().After) + " 円";
            string plus = "";
            if(money - GetComponent<DataLoader>().LoadLeastPayment().After > 0)
            {
                plus = "+";
            }
            CheckUIError.text = plus + Number.MarkDecimal(money - GetComponent<DataLoader>().LoadLeastPayment().After).ToString() + " 円";
        }
        else if (ButtonMode == 3)
        {
            Numpad.SetActive(true);
            if (入出金金額.isFocused)
            {
                金額入力(入出金金額, 10000000);
            }
            else
            {
                if(!Number.CheckNumber(入出金金額.text.Replace(" 円", "")))
                   入出金金額.text = Number.MarkDecimal(入出金金額.text) + " 円";
            }

            if(入出金金額.text.Replace(" 円", "") == "0" || 入出金金額.text == "" || 入出金内容.text == "")
            {
                入金ボタン.interactable = false;
                出金ボタン.interactable = false;
            }
            else
            {
                入金ボタン.interactable = true;
                出金ボタン.interactable = true;
            }
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
                int cnt = 0;
                Directory.CreateDirectory("data/payment");
                while (true)
                {
                    if (!GetComponent<DataLoader>().checkExist("data/payment/" + cnt.ToString() + ".json"))
                    {
                        break;
                    }
                    cnt++;
                }
                if (cnt == 0)
                {
                    ID = 0;
                    maximumID = 0;
                }
                else
                {
                    ID = cnt - 1;
                    maximumID = ID;
                }
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
                入出金金額.text = "";
                入出金内容.text = "";
                入金ボタン.interactable = false;
                出金ボタン.interactable = false;
                break;
        }
    }

    public void ButtonPressed(int ButtonNumber)
    {
        ButtonMode = ButtonNumber;
        setUI();
    }

    public void レジ金設定()
    {
        int BeforeCash = GetComponent<DataLoader>().LoadLeastPayment().After;
        int money = int.Parse(OpeningUIText.text.Substring(0, OpeningUIText.text.Length - 2).Replace(",", ""));
        GetComponent<DataLoader>().AddPayment(money, "開店前レジ金設定", true);

        DateTime time = DateTime.Now;
        GetComponent<Receipt>().レジ金設定レシート(money, BeforeCash, OpeningUI, note);
    }

    public void レジ金チェック()
    {
        int money = int.Parse(CheckUIInput.text.Substring(0, CheckUIInput.text.Length - 2).Replace(",", ""));

        GetComponent<Receipt>().レジ金チェックレシート(money, CheckUI, note);
    }

    public void 取引ID増減(int num)
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

    public void 注文取消()
    {
        if (DescriptionUI.text.Substring(0, 4) == "取り消し")
        {
            
        }
        else if(DescriptionUI.text.Substring(0, 4) == "商品購入")
        {
            DataLoader.Payment temp = GetComponent<DataLoader>().LoadPayment(ID);
            GetComponent<DataLoader>().AddPayment(-temp.Increase, "払い戻し-" + temp.Description, false);

            maximumID++;

            string date = temp.Description.Substring(5, 8);
            string number = temp.Description.Substring(13);
            var path = "data/order/" + date + "/" + number + ".csv";

            if (GetComponent<DataLoader>().checkExist(path))
            {
                var list = DataLoader.LoadList();
                var 購入履歴 = GetComponent<DataLoader>().LoadOrderList(path);
                var orderList = GetComponent<DataLoader>().LoadOrderList(path);

                for(int i=0; i<orderList.Count; i++)
                {
                    orderList[i].Amount *= -1;
                    orderList[i].Price *= -1;
                }
                int cnt = 0;
                while (true)
                {
                    if (!GetComponent<DataLoader>().checkExist("data/order/" + date + "/" + cnt.ToString() + ".csv"))
                        break;
                    cnt++;
                }
                GetComponent<DataLoader>().SaveOrderList(@"data/order/" + date + "/" + cnt.ToString() + ".csv", orderList);

                GetComponent<Receipt>().注文会計取り消しレシート(temp, 購入履歴);

                temp.Description = "取り消し-" + temp.Description;
                Directory.CreateDirectory("data/payment");
                GetComponent<DataLoader>().saveFile("data/payment/" + ID.ToString() + ".json", JsonMapper.ToJson(temp));

                foreach (var record in 購入履歴)
                {
                    var tempList = list[record.Number];
                    tempList.Stock += record.Amount;
                    list[record.Number] = tempList;
                }
                DataLoader.SaveList(list);
            }
            else
            {
                GetComponent<Receipt>().会計取り消しレシート(temp);

                temp.Description = "取り消し-" + temp.Description;
                Directory.CreateDirectory("data/payment");
                GetComponent<DataLoader>().saveFile("data/payment/" + ID.ToString() + ".json", JsonMapper.ToJson(temp));

            }
        }
        else
        {
            DataLoader.Payment temp = GetComponent<DataLoader>().LoadPayment(ID);
            GetComponent<DataLoader>().AddPayment(-temp.Increase, "払い戻し-" + temp.Description, false);

            GetComponent<Receipt>().会計取り消しレシート(temp);

            temp.Description = "取り消し-" + temp.Description;
            Directory.CreateDirectory("data/payment");
            GetComponent<DataLoader>().saveFile("data/payment/" + ID.ToString() + ".json", JsonMapper.ToJson(temp));
            maximumID++;
        }
    }

    public void AddDeposit(int num)
    {
        NumPadKey.Value = 0;

        if (num == 1)
        {
            GetComponent<DataLoader>().AddPayment(Number.ToNumber(DepositMoneyUI.text.Replace(" 円", "").Replace(",", "")), "入金-" + DepositDescriptionUI.text, false);
            int money = Number.ToNumber(DepositMoneyUI.text.Replace("円", "").Replace(",", "").Replace(" ", ""));
            string description = DepositDescriptionUI.text;

            DepositMoneyUI.transform.parent.GetComponent<InputField>().text = "0";
            DepositDescriptionUI.transform.parent.GetComponent<InputField>().text = "";

            GetComponent<Receipt>().レジ入金レシート(money, description);
        }
        else if (num == -1)
        {
            GetComponent<DataLoader>().AddPayment(-Number.ToNumber(DepositMoneyUI.text.Replace(" 円", "").Replace(",", "")), "出金-" + DepositDescriptionUI.text, false);
            int money = Number.ToNumber(DepositMoneyUI.text.Replace("円", "").Replace(",", "").Replace(" ", ""));
            string description = DepositDescriptionUI.text;

            DepositMoneyUI.transform.parent.GetComponent<InputField>().text = "0";
            DepositDescriptionUI.transform.parent.GetComponent<InputField>().text = "";

            GetComponent<Receipt>().レジ出金レシート(money, description);
        }
    }

    private void 金額入力(InputField UI, int limit)
    {
        NumPadKey.対象切り替え(UI);
        NumPadKey.数値上限設定(limit);
    }

}
