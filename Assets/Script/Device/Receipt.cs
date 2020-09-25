using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;


public class Receipt : MonoBehaviour
{
    public void GenerateReceipt(List<string> Text, bool useLINENotify)
    {
        DataLoader c = GetComponent<DataLoader>();
        DateTime time = DateTime.Now;
        Directory.CreateDirectory("data/receipt/");

        string Filename = "data/receipt/" + time.ToString("yyyyMMdd_HHmmss") + ".txt";

        if (GetComponent<DataLoader>().LoadConfig().EnableLINENotify && useLINENotify)
        {
            string LINENotifyText = "";
            string Token = GetComponent<DataLoader>().LoadConfig().LINENotifyToken;
            foreach (string line in Text)
            {
                LINENotifyText += (line + Environment.NewLine) ;
            }
            LINENotify.SendMessage(LINENotifyText, Token);
        }

            if (GetComponent<DataLoader>().LoadConfig().Printer)
            {
                string PrinterText = "";
                foreach (string line in Text)
                {
                    PrinterText += (line + "<n>");
                }
                GetComponent<Printer>().Print(PrinterText, "ＭＳゴシック", 24, GetComponent<DataLoader>().LoadConfig().PrinterName);
            }

        foreach (string line in Text)
        {
            c.AddLine(@Filename, line);
        }
    }

    public void 注文完了レシート(List<GameManager.OrderList> _list, GameManager.OrderList 注文情報, int 割引額, int 現金, int Count)
    {
        DateTime time = DateTime.Now;
        DataLoader c = GetComponent<DataLoader>();
        int Tax = GetComponent<DataLoader>().LoadConfig().Tax;
        string StoreName = GetComponent<DataLoader>().LoadConfig().StoreName;

        List<string> Text = new List<string>();
        Text.Add(StoreName);
        Text.Add("");
        Text.Add(time.ToString("yyyy年MM月dd日 HH時mm分ss秒"));
        Text.Add("-----------------------------");
        foreach (var record in _list)
        {
            Text.Add(record.Name + new String('　', 12 - record.Name.Length) + Number.MarkDecimal(record.Price) + "　円");
            Text.Add("　　　" + (record.Price / record.Amount) + "　@　" + Number.MarkDecimal(record.Amount) + "　円");
        }
        Text.Add("小　計／" + 注文情報.Amount + "点" + new String('　', 5 + 注文情報.Amount.ToString().Length) + Number.MarkDecimal(注文情報.Price) + " 円");
        Text.Add("");
        Text.Add("-----------------------------");
        if (割引額 != 0)
            Text.Add("割　引   " + Number.MarkDecimal(割引額) + "　円");
        Text.Add("合　計   " + Number.MarkDecimal(注文情報.Price - 割引額) + "　円");
        Text.Add("内　税   " + Number.MarkDecimal(Number.InsideTax(注文情報.Price - 割引額, Tax)) + "　円");
        Text.Add("お預り   " + Number.MarkDecimal(現金) + "　円");
        Text.Add("お釣り   " + Number.MarkDecimal(現金 - (注文情報.Price - 割引額)) + "　円");
        Text.Add("");
        Text.Add("上記正に領収いたしました");
        Text.Add("   伝票番号 " + time.ToString("yyyyMMdd_HHmmss") + "(" + Count.ToString() + ")");
        Text.Add("");
        Text.Add("-----------------------------");
        Text.Add("お買い上げ、ありがとうございました。");
        Text.Add("またのご来店をお待ちしております。");

        GenerateReceipt(Text, GetComponent<DataLoader>().LoadConfig().LINENotifyPurchaseNotice);
    }

    public void レジ金設定レシート(int money, int BeforeCash, GameObject OpeningUI, int[] note)
    {
        DateTime time = DateTime.Now;
        List<string> Text = new List<string>();

        Text.Add(GetComponent<DataLoader>().LoadConfig().StoreName);
        Text.Add("");
        Text.Add(time.ToString("yyyy年MM月dd日 HH時mm分ss秒"));
        Text.Add("-----------------------------");
        Text.Add("レジ金設定");
        Text.Add("設定額　　　" + Number.MarkDecimal(money) + " 円");
        Text.Add("設定前　　　" + Number.MarkDecimal(BeforeCash) + " 円");
        Text.Add("");
        Text.Add("-----------------------------");
        for (int i = 0; i < 10; i++)
        {
            string temp = OpeningUI.transform.Find(i.ToString()).GetComponent<InputField>().text;
            if (temp != "")
            {
                Text.Add(note[i].ToString() + "円 @ " + temp);
            }
            else
            {
                Text.Add(note[i].ToString() + "円 @ " + "0");
            }
        }

        GenerateReceipt(Text, true);
    }

    public void レジ金チェックレシート(int money, GameObject CheckUI, int[] note)
    {
        List<string> Text = new List<string>();
        DateTime time = DateTime.Now;
        Text.Add(GetComponent<DataLoader>().LoadConfig().StoreName);
        Text.Add("");
        Text.Add(time.ToString("yyyy年MM月dd日 HH時mm分ss秒"));
        Text.Add("-----------------------------");
        Text.Add("レジ金確認");
        Text.Add("入力額　　　　" + Number.MarkDecimal(money) + " 円");
        Text.Add("レジ内金額　　" + Number.MarkDecimal(GetComponent<DataLoader>().LoadLeastPayment().After) + " 円");
        Text.Add("");
        Text.Add("誤差額　　　　" + Number.MarkDecimal(money - GetComponent<DataLoader>().LoadLeastPayment().After) + " 円");
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

        GenerateReceipt(Text, true);
    }

    public void 注文会計取り消しレシート(DataLoader.Payment Payment, List<GameManager.OrderList> 購入履歴)
    {
        List<string> Text = new List<string>();
        DateTime time = DateTime.Now;
        Text.Add(GetComponent<DataLoader>().LoadConfig().StoreName);
        Text.Add("");
        Text.Add(time.ToString("yyyy年MM月dd日 HH時mm分ss秒"));
        Text.Add("-----------------------------");
        Text.Add("会計取り消し");
        Text.Add("払い戻し額　　" + Number.MarkDecimal(Payment.Increase) + " 円");
        Text.Add("取引内容　　　" + Payment.Description);
        Text.Add("");
        foreach (var record in 購入履歴)
        {
            Text.Add(record.Name + new String('　', 12 - record.Name.Length) + Number.MarkDecimal(record.Price) + "　円");
            Text.Add("　　　" + (record.Price / record.Amount) + "　@　" + Number.MarkDecimal(record.Amount) + "　円");
        }
        Text.Add("");
        Text.Add("レジ内金額　　" + Number.MarkDecimal(GetComponent<DataLoader>().LoadLeastPayment().After) + " 円");
        Text.Add("");
        Text.Add("-----------------------------");

        GenerateReceipt(Text, true);
    }

    public void 会計取り消しレシート(DataLoader.Payment Payment)
    {
        List<string> Text = new List<string>();
        DateTime time = DateTime.Now;
        Text.Add(GetComponent<DataLoader>().LoadConfig().StoreName);
        Text.Add("");
        Text.Add(time.ToString("yyyy年MM月dd日 HH時mm分ss秒"));
        Text.Add("-----------------------------");
        Text.Add("会計取り消し");
        Text.Add("払い戻し額　　" + Number.MarkDecimal(Payment.Increase) + " 円");
        Text.Add("取引内容　　　" + Payment.Description);
        Text.Add("");
        Text.Add("レジ内金額　　" + Number.MarkDecimal(GetComponent<DataLoader>().LoadLeastPayment().After) + " 円");
        Text.Add("");
        Text.Add("-----------------------------");
        GetComponent<Receipt>().GenerateReceipt(Text, true);
    }

    public void レジ入金レシート(Text DepositMoneyUI, Text DepositDescriptionUI)
    {
        List<string> Text = new List<string>();
        DateTime time = DateTime.Now;
        Text.Add(GetComponent<DataLoader>().LoadConfig().StoreName);
        Text.Add("");
        Text.Add(time.ToString("yyyy年MM月dd日 HH時mm分ss秒"));
        Text.Add("-----------------------------");
        Text.Add("入金");
        Text.Add("入金額　　　" + Number.MarkDecimal(Number.ToNumber(DepositMoneyUI.text.Replace(" 円", "").Replace(",", ""))) + " 円");
        Text.Add("取引内容　　" + DepositDescriptionUI.text);
        Text.Add("");
        Text.Add("レジ内金額　　" + Number.MarkDecimal(GetComponent<DataLoader>().LoadLeastPayment().After) + " 円");
        Text.Add("");
        Text.Add("-----------------------------");
        GetComponent<Receipt>().GenerateReceipt(Text, true);
    }

    public void レジ出金レシート(Text DepositMoneyUI, Text DepositDescriptionUI)
    {
        List<string> Text = new List<string>();
        DateTime time = DateTime.Now;
        Text.Add(GetComponent<DataLoader>().LoadConfig().StoreName);
        Text.Add("");
        Text.Add(time.ToString("yyyy年MM月dd日 HH時mm分ss秒"));
        Text.Add("-----------------------------");
        Text.Add("出金");
        Text.Add("出金額　　　" + Number.MarkDecimal(Number.ToNumber(DepositMoneyUI.text.Replace(" 円", "").Replace(",", ""))) + " 円");
        Text.Add("取引内容　　" + DepositDescriptionUI.text);
        Text.Add("");
        Text.Add("レジ内金額　　" + Number.MarkDecimal(GetComponent<DataLoader>().LoadLeastPayment().After) + " 円");
        Text.Add("");
        Text.Add("-----------------------------");
        GetComponent<Receipt>().GenerateReceipt(Text, true);
    }

}
