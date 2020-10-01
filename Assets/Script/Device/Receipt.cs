using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;


public class Receipt : MonoBehaviour
{
    public string FormatDataDirectory = @"data/template/";

    public void GenerateReceipt(List<string> Text, bool useLINENotify)
    {
        GenerateReceipt(GetComponent<DataLoader>().LoadConfig().PrinterName, Text, useLINENotify);
    }

    public void GenerateReceipt(string PrinterName, List<string> Text, bool useLINENotify)
    {
        DataLoader c = GetComponent<DataLoader>();
        DateTime time = DateTime.Now;
        Directory.CreateDirectory("data/receipt/");

        string Filename = "data/receipt/" + time.ToString("yyyyMMdd_HHmmss") + ".txt";


        if (GetComponent<DataLoader>().LoadConfig().EnableLINENotify && useLINENotify)
        {
            //Todo LINENotify にタグが入らないようにする
            string LINENotifyText = "";
            string Token = c.LoadConfig().LINENotifyToken;

            foreach (string line in Text)
            {
                LINENotifyText += (line + Environment.NewLine) ;
            }

            LINENotifyText = RemoveTags(LINENotifyText);

            LINENotify.SendMessage(LINENotifyText, Token);
        }

            if (c.LoadConfig().Printer)
            {
                string PrinterText = "";
                foreach (string line in Text)
                {
                    PrinterText += (line + "<n>");
                }
                GetComponent<Printer>().Print(PrinterText, c.LoadConfig().PrinterFontFamily, c.LoadConfig().PrinterFontSize, PrinterName);
            }

        string FileText = "";
        foreach (string line in Text)
        {
            FileText += (line + Environment.NewLine);
        }

        FileText = RemoveTags(FileText);
        c.saveFile(@Filename, FileText);
    }

    public void 注文完了レシート(List<GameManager.OrderList> _list, GameManager.OrderList 注文情報, int 割引額, int 現金, int Count)
    {
        注文完了レシート("order.txt", GetComponent<DataLoader>().LoadConfig().PrinterName, _list, 注文情報, 割引額, 現金, Count);
    }

    public void 注文完了レシート(string templateDir, string PrinterName, List<GameManager.OrderList> _list, GameManager.OrderList 注文情報, int 割引額, int 現金, int Count)
    {
        DateTime time = DateTime.Now;
        DataLoader c = GetComponent<DataLoader>();
        int Tax = GetComponent<DataLoader>().LoadConfig().Tax;
        string StoreName = GetComponent<DataLoader>().LoadConfig().StoreName;

        try
        {
            string[] del = { "\r\n" };
            List<string> Text = new List<string>();
            var t = GetComponent<DataLoader>().loadFile(FormatDataDirectory + templateDir);

            //タグ （STORE_NAME, DATE, TIME）
            t = t.Replace(Environment.NewLine, "<n>");

            t = t.Replace("<tSTORE_NAME>", GetComponent<DataLoader>().LoadConfig().StoreName);
            t = t.Replace("<tDATE>", DateTime.Now.ToString(GetComponent<DataLoader>().LoadConfig().FormatDate));
            t = t.Replace("<tTIME>", DateTime.Now.ToString(GetComponent<DataLoader>().LoadConfig().FormatTime));

            t = t.Replace("<tAMOUNT_TOTAL>", Number.MarkDecimal(注文情報.Amount)); //商品数
            t = t.Replace("<tPRICE_TOTAL>", Number.MarkDecimal(注文情報.Price)); //小計（割引額を含まない総額）
            t = t.Replace("<tDISCOUNT>", Number.MarkDecimal(割引額)); //割引額
            t = t.Replace("<tTOTAL>", Number.MarkDecimal(注文情報.Price - 割引額)); //合計金額
            t = t.Replace("<tINSIDE_TAX>", Number.MarkDecimal(Number.InsideTax(注文情報.Price - 割引額, Tax))); //内税
            t = t.Replace("<tPRICE_KEEP>", Number.MarkDecimal(現金)); //お預かり
            t = t.Replace("<tPRICE_RETURN>", Number.MarkDecimal(現金 - (注文情報.Price - 割引額))); //お釣り
            t = t.Replace("<tNUMBER>", Count.ToString()); //レシート番号

            string Tag;
            bool flag = false;
            for (int i = 0; i < t.Length; i++)
            {
                string loopText;
                if (i + 2 < t.Length)
                {
                    Tag = t.Substring(i, 2);
                }
                else
                {
                    break;
                }

                switch (Tag)
                {
                    case "<l":
                        if (GetComponent<Printer>().GetTag(t, i, "string") == "GOODS")
                        {
                            string TempTag;
                            for (int j = i; j < t.Length; j++)
                            {
                                if (j + 2 < t.Length)
                                {
                                    TempTag = t.Substring(j, 3);
                                }
                                else
                                {
                                    break;
                                }

                                switch (TempTag)
                                {
                                    case "<l>":
                                        int dif = GetComponent<Printer>().GetTag(t, i, "string").Length + 3;
                                        loopText = t.Substring(i + dif, j - i - dif);

                                        string tempText = "";
                                        foreach (var record in _list)
                                        {
                                            string m = loopText;
                                            m = m.Replace("<mGOODS_NAME>", record.Name);
                                            m = m.Replace("<mPRICE>", Number.MarkDecimal(record.Price));
                                            m = m.Replace("<mGOODS_PRICE>", Number.MarkDecimal(record.Price / record.Amount));
                                            m = m.Replace("<mAMOUNT>", Number.MarkDecimal(record.Amount));

                                            tempText += m;
                                        }

                                        t = t.Replace("<l" + GetComponent<Printer>().GetTag(t, i, "string") + ">" + loopText + "<l>", tempText);
                                        flag = true;
                                        break;
                                }
                            }
                            if (flag)
                            {
                                break;
                            }
                        }
                        break;
                }
            }

            foreach (var record in t.Split(del, StringSplitOptions.None))
            {
                Text.Add(record);
            }

            GenerateReceipt(PrinterName, Text, GetComponent<DataLoader>().LoadConfig().LINENotifyPurchaseNotice);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to print! :" + e);
        }
    }

    public void 注文伝票レシート(string PrinterName, List<GameManager.OrderList> _list, GameManager.OrderList 注文情報, int 割引額, int 現金, int Number)
    {
        注文完了レシート("order_list.txt", PrinterName, _list, 注文情報, 割引額, 現金, Number);
    }

    public void レジ金設定レシート(int money, int BeforeCash, GameObject OpeningUI, int[] note)
    {
        string[] del = { "\r\n" };
        List<string> Text = new List<string>();
        var t = GetComponent<DataLoader>().loadFile(FormatDataDirectory + "set_register.txt");

        //タグ （STORE_NAME, DATE, TIME）
        t = t.Replace(Environment.NewLine, "<n>");
        t = t.Replace("<tSTORE_NAME>", GetComponent<DataLoader>().LoadConfig().StoreName);
        t = t.Replace("<tDATE>", DateTime.Now.ToString(GetComponent<DataLoader>().LoadConfig().FormatDate));
        t = t.Replace("<tTIME>", DateTime.Now.ToString(GetComponent<DataLoader>().LoadConfig().FormatTime));

        t = t.Replace("<tAFTER_CASH>", Number.MarkDecimal(money)); //商品数
        t = t.Replace("<tBEFORE_CASH>", Number.MarkDecimal(BeforeCash)); //小計（割引額を含まない総額）

        string Tag;
        bool flag = false;
        for (int i = 0; i < t.Length; i++)
        {
            string loopText;
            if (i + 2 < t.Length)
            {
                Tag = t.Substring(i, 2);
            }
            else
            {
                break;
            }

            switch (Tag)
            {
                case "<l":
                    if (GetComponent<Printer>().GetTag(t, i, "string") == "CASH_DETAIL")
                    {
                        string TempTag;
                        for (int j = i; j < t.Length; j++)
                        {
                            if (j + 2 < t.Length)
                            {
                                TempTag = t.Substring(j, 3);
                            }
                            else
                            {
                                break;
                            }

                            switch (TempTag)
                            {
                                case "<l>":
                                    int dif = GetComponent<Printer>().GetTag(t, i, "string").Length + 3;
                                    loopText = t.Substring(i + dif, j - i - dif);

                                    string tempText = "";
                                    for (int l = 0; l < 10; l++)
                                    {
                                        string temp = OpeningUI.transform.Find(l.ToString()).GetComponent<InputField>().text;
                                        string m = loopText;
                                        if (temp != "")
                                        {
                                            m = m.Replace("<mCASH_NAME>", note[l].ToString());
                                            m = m.Replace("<mCASH_AMOUNT>", Number.MarkDecimal(temp));
                                            m = m.Replace("<mCASH_TOTAL>", Number.MarkDecimal(Number.ToNumber(temp) * note[l]));
                                        }
                                        else
                                        {
                                            m = m.Replace("<mCASH_NAME>", note[l].ToString());
                                            m = m.Replace("<mCASH_AMOUNT>", Number.MarkDecimal(0));
                                            m = m.Replace("<mCASH_TOTAL>", Number.MarkDecimal(0));
                                        }

                                        tempText += m;
                                    }

                                    t = t.Replace("<l" + GetComponent<Printer>().GetTag(t, i, "string") + ">" + loopText + "<l>", tempText);
                                    flag = true;
                                    break;
                            }
                        }
                        if (flag)
                        {
                            break;
                        }
                    }
                    break;
            }
        }

        foreach (var record in t.Split(del, StringSplitOptions.None))
        {
            Text.Add(record);
        }

        GenerateReceipt(Text, GetComponent<DataLoader>().LoadConfig().LINENotifyPurchaseNotice);

    }

    public void レジ金チェックレシート(int money, GameObject CheckUI, int[] note)
    {
        string[] del = { "\r\n" };
        List<string> Text = new List<string>();
        var t = GetComponent<DataLoader>().loadFile(FormatDataDirectory + "check_register.txt");

        //タグ （STORE_NAME, DATE, TIME）
        t = t.Replace(Environment.NewLine, "<n>");
        t = t.Replace("<tSTORE_NAME>", GetComponent<DataLoader>().LoadConfig().StoreName);
        t = t.Replace("<tDATE>", DateTime.Now.ToString(GetComponent<DataLoader>().LoadConfig().FormatDate));
        t = t.Replace("<tTIME>", DateTime.Now.ToString(GetComponent<DataLoader>().LoadConfig().FormatTime));

        t = t.Replace("<tREGISTER_CASH>", Number.MarkDecimal(GetComponent<DataLoader>().LoadLeastPayment().After));
        t = t.Replace("<tINPUT_CASH>", Number.MarkDecimal(money));
        t = t.Replace("<tDIFFERENCE>", Number.MarkDecimal(money - GetComponent<DataLoader>().LoadLeastPayment().After));

        string Tag;
        bool flag = false;
        for (int i = 0; i < t.Length; i++)
        {
            string loopText;
            if (i + 2 < t.Length)
            {
                Tag = t.Substring(i, 2);
            }
            else
            {
                break;
            }

            switch (Tag)
            {
                case "<l":
                    if (GetComponent<Printer>().GetTag(t, i, "string") == "CASH_DETAIL")
                    {
                        string TempTag;
                        for (int j = i; j < t.Length; j++)
                        {
                            if (j + 2 < t.Length)
                            {
                                TempTag = t.Substring(j, 3);
                            }
                            else
                            {
                                break;
                            }

                            switch (TempTag)
                            {
                                case "<l>":
                                    int dif = GetComponent<Printer>().GetTag(t, i, "string").Length + 3;
                                    loopText = t.Substring(i + dif, j - i - dif);

                                    string tempText = "";
                                    for (int l = 0; l < 10; l++)
                                    {
                                        string m = loopText;
                                        string temp = CheckUI.transform.Find(l.ToString()).GetComponent<InputField>().text;
                                        if (temp != "")
                                        {
                                            m = m.Replace("<mCASH_NAME>", note[l].ToString());
                                            m = m.Replace("<mCASH_AMOUNT>", Number.MarkDecimal(temp));
                                            m = m.Replace("<mCASH_TOTAL>", Number.MarkDecimal(Number.ToNumber(temp) * note[l]));
                                        }
                                        else
                                        {
                                            m = m.Replace("<mCASH_NAME>", note[l].ToString());
                                            m = m.Replace("<mCASH_AMOUNT>", Number.MarkDecimal(0));
                                            m = m.Replace("<mCASH_TOTAL>", Number.MarkDecimal(0));
                                        }

                                        tempText += m;
                                    }

                                    t = t.Replace("<l" + GetComponent<Printer>().GetTag(t, i, "string") + ">" + loopText + "<l>", tempText);
                                    flag = true;
                                    break;
                            }
                        }
                        if (flag)
                        {
                            break;
                        }
                    }
                    break;
            }
        }

        foreach (var record in t.Split(del, StringSplitOptions.None))
        {
            Text.Add(record);
        }

        GenerateReceipt(Text, GetComponent<DataLoader>().LoadConfig().LINENotifyPurchaseNotice);
    }

    public void 注文会計取り消しレシート(DataLoader.Payment Payment, List<GameManager.OrderList> 購入履歴)
    {
        string[] del = { "\r\n" };
        List<string> Text = new List<string>();
        var t = GetComponent<DataLoader>().loadFile(FormatDataDirectory + "cancel_order_payment.txt");

        //タグ （STORE_NAME, DATE, TIME）
        t = t.Replace(Environment.NewLine, "<n>");

        t = t.Replace("<tSTORE_NAME>", GetComponent<DataLoader>().LoadConfig().StoreName);
        t = t.Replace("<tDATE>", DateTime.Now.ToString(GetComponent<DataLoader>().LoadConfig().FormatDate));
        t = t.Replace("<tTIME>", DateTime.Now.ToString(GetComponent<DataLoader>().LoadConfig().FormatTime));

        t = t.Replace("<tPAYMENT_RETURN>", Number.MarkDecimal(Payment.Increase));
        t = t.Replace("<tPAYMENT_DESCRIPTION>", Payment.Description);
        t = t.Replace("<tREGISTER_CASH>", Number.MarkDecimal(GetComponent<DataLoader>().LoadLeastPayment().After));

        string Tag;
        bool flag = false;
        for (int i = 0; i < t.Length; i++)
        {
            string loopText;
            if (i + 2 < t.Length)
            {
                Tag = t.Substring(i, 2);
            }
            else
            {
                break;
            }

            switch (Tag)
            {
                case "<l":
                    if (GetComponent<Printer>().GetTag(t, i, "string") == "GOODS")
                    {
                        string TempTag;
                        for (int j = i; j < t.Length; j++)
                        {
                            if (j + 2 < t.Length)
                            {
                                TempTag = t.Substring(j, 3);
                            }
                            else
                            {
                                break;
                            }

                            switch (TempTag)
                            {
                                case "<l>":
                                    int dif = GetComponent<Printer>().GetTag(t, i, "string").Length + 3;
                                    loopText = t.Substring(i + dif, j - i - dif);

                                    string tempText = "";
                                    foreach (var record in 購入履歴)
                                    {
                                        string m = loopText;
                                        m = m.Replace("<mGOODS_NAME>", record.Name);
                                        m = m.Replace("<mPRICE>", Number.MarkDecimal(record.Price));
                                        m = m.Replace("<mGOODS_PRICE>", Number.MarkDecimal(record.Price / record.Amount));
                                        m = m.Replace("<mAMOUNT>", Number.MarkDecimal(record.Amount));

                                        tempText += m;
                                    }

                                    t = t.Replace("<l" + GetComponent<Printer>().GetTag(t, i, "string") + ">" + loopText + "<l>", tempText);
                                    flag = true;
                                    break;
                            }
                        }
                        if (flag)
                        {
                            break;
                        }
                    }
                    break;
            }
        }

        foreach (var record in t.Split(del, StringSplitOptions.None))
        {
            Text.Add(record);
        }

        GenerateReceipt(Text, GetComponent<DataLoader>().LoadConfig().LINENotifyPurchaseNotice);
    }

    public void 会計取り消しレシート(DataLoader.Payment Payment)
    {
        string[] del = { "\r\n" };
        List<string> Text = new List<string>();
        var t = GetComponent<DataLoader>().loadFile(FormatDataDirectory + "cancel_payment.txt");

        //タグ （STORE_NAME, DATE, TIME）
        t = t.Replace(Environment.NewLine, "<n>");

        t = t.Replace("<tSTORE_NAME>", GetComponent<DataLoader>().LoadConfig().StoreName);
        t = t.Replace("<tDATE>", DateTime.Now.ToString(GetComponent<DataLoader>().LoadConfig().FormatDate));
        t = t.Replace("<tTIME>", DateTime.Now.ToString(GetComponent<DataLoader>().LoadConfig().FormatTime));

        t = t.Replace("<tPAYMENT_RETURN>", Number.MarkDecimal(Payment.Increase));
        t = t.Replace("<tPAYMENT_DESCRIPTION>", Payment.Description);
        t = t.Replace("<tREGISTER_CASH>", Number.MarkDecimal(GetComponent<DataLoader>().LoadLeastPayment().After));

        foreach (var record in t.Split(del, StringSplitOptions.None))
        {
            Text.Add(record);
        }

        GenerateReceipt(Text, GetComponent<DataLoader>().LoadConfig().LINENotifyPurchaseNotice);
    }

    public void レジ入金レシート(int money, string description)
    {
        string[] del = { "\r\n" };
        List<string> Text = new List<string>();
        var t = GetComponent<DataLoader>().loadFile(FormatDataDirectory + "payment.txt");

        //タグ （STORE_NAME, DATE, TIME）
        t = t.Replace(Environment.NewLine, "<n>");

        t = t.Replace("<tSTORE_NAME>", GetComponent<DataLoader>().LoadConfig().StoreName);
        t = t.Replace("<tDATE>", DateTime.Now.ToString(GetComponent<DataLoader>().LoadConfig().FormatDate));
        t = t.Replace("<tTIME>", DateTime.Now.ToString(GetComponent<DataLoader>().LoadConfig().FormatTime));

        t = t.Replace("<tPAYMENT>", Number.MarkDecimal(money));
        t = t.Replace("<tPAYMENT_DESCRIPTION>", description);
        t = t.Replace("<tREGISTER_CASH>", Number.MarkDecimal(GetComponent<DataLoader>().LoadLeastPayment().After));

        foreach (var record in t.Split(del, StringSplitOptions.None))
        {
            Text.Add(record);
        }

        GenerateReceipt(Text, GetComponent<DataLoader>().LoadConfig().LINENotifyPurchaseNotice);
    }

    public void レジ出金レシート(int money, string description)
    {
        string[] del = { "\r\n" };
        List<string> Text = new List<string>();
        var t = GetComponent<DataLoader>().loadFile(FormatDataDirectory + "withdrawal.txt");

        //タグ （STORE_NAME, DATE, TIME）
        t = t.Replace(Environment.NewLine, "<n>");

        t = t.Replace("<tSTORE_NAME>", GetComponent<DataLoader>().LoadConfig().StoreName);
        t = t.Replace("<tDATE>", DateTime.Now.ToString(GetComponent<DataLoader>().LoadConfig().FormatDate));
        t = t.Replace("<tTIME>", DateTime.Now.ToString(GetComponent<DataLoader>().LoadConfig().FormatTime));

        t = t.Replace("<tPAYMENT>", Number.MarkDecimal(money));
        t = t.Replace("<tPAYMENT_DESCRIPTION>", description);
        t = t.Replace("<tREGISTER_CASH>", Number.MarkDecimal(GetComponent<DataLoader>().LoadLeastPayment().After));

        foreach (var record in t.Split(del, StringSplitOptions.None))
        {
            Text.Add(record);
        }

        GenerateReceipt(Text, GetComponent<DataLoader>().LoadConfig().LINENotifyPurchaseNotice);
    }

    public void その他レシート(string filename)
    {
        string[] del = { "\r\n" };
        List<string> Text = new List<string>();
        var t = GetComponent<DataLoader>().loadFile(FormatDataDirectory + filename);

        //タグ （STORE_NAME, DATE, TIME）
        t = t.Replace(Environment.NewLine, "<n>");

        t = t.Replace("<tSTORE_NAME>", GetComponent<DataLoader>().LoadConfig().StoreName);
        t = t.Replace("<tDATE>", DateTime.Now.ToString(GetComponent<DataLoader>().LoadConfig().FormatDate));
        t = t.Replace("<tTIME>", DateTime.Now.ToString(GetComponent<DataLoader>().LoadConfig().FormatTime));

        foreach (var record in t.Split(del, StringSplitOptions.None))
        {
            Text.Add(record);
        }

        GenerateReceipt(Text, GetComponent<DataLoader>().LoadConfig().LINENotifyPurchaseNotice);
    }

    public void 整理券レシート(string PrinterName, int Number)
    {
        string[] del = { "\r\n" };
        List<string> Text = new List<string>();
        var t = GetComponent<DataLoader>().loadFile(FormatDataDirectory + "number_ticket.txt");

        //タグ （STORE_NAME, DATE, TIME）
        t = t.Replace(Environment.NewLine, "<n>");

        t = t.Replace("<tSTORE_NAME>", GetComponent<DataLoader>().LoadConfig().StoreName);
        t = t.Replace("<tDATE>", DateTime.Now.ToString(GetComponent<DataLoader>().LoadConfig().FormatDate));
        t = t.Replace("<tTIME>", DateTime.Now.ToString(GetComponent<DataLoader>().LoadConfig().FormatTime));
        t = t.Replace("<tNUMBER>", Number.ToString());

        foreach (var record in t.Split(del, StringSplitOptions.None))
        {
            Text.Add(record);
        }

        GenerateReceipt(Text, GetComponent<DataLoader>().LoadConfig().LINENotifyPurchaseNotice);
    }

    private string RemoveTags(string text)
    {
        Printer p = GetComponent<Printer>();
        string Tag;

        for (int i = 0; i < text.Length; i++)
        {
            if (i + 2 < text.Length)
            {
                Tag = text.Substring(i, 2);
            }
            else
            {
                break;
            }

            switch (Tag)
            {
                case "<s":
                    if (p.GetTag(text, i, "number") == "")
                    {
                        text = text.Replace("<s>", "");
                        i--;
                    }
                    else
                    {
                        text = text.Replace("<s" + p.GetTag(text, i, "number") + ">", "");
                        i--;
                    }
                    break;
                case "<x": //文字サイズ（引数：サイズ）
                    if (p.GetTag(text, i, "number") == "")
                    {
                        text = text.Replace("<x>", "");
                        i--;
                    }
                    else
                    {
                        text = text.Replace("<x" + p.GetTag(text, i, "number") + ">", "");
                        i--;
                    }
                    break;
                case "<y": //文字サイズ（引数：サイズ）
                    if (p.GetTag(text, i, "number") == "")
                    {
                        text = text.Replace("<y>", "");
                        i--;
                    }
                    else
                    {
                        text = text.Replace("<y" + p.GetTag(text, i, "number") + ">", "");
                        i--;
                    }
                    break;
                case "<X": //文字サイズ（引数：サイズ）
                    if (p.GetTag(text, i, "number") == "")
                    {
                        text = text.Replace("<X>", "");
                        i--;
                    }
                    else
                    {
                        text = text.Replace("<X" + p.GetTag(text, i, "number") + ">", "");
                        i--;
                    }
                    break;
                case "<Y": //文字サイズ（引数：サイズ）
                    if (p.GetTag(text, i, "number") == "")
                    {
                        text = text.Replace("<Y>", "");
                        i--;
                    }
                    else
                    {
                        text = text.Replace("<Y" + p.GetTag(text, i, "number") + ">", "");
                        i--;
                    }
                    break;
                case "<n":
                    if (p.GetTag(text, i, "string") == "")
                    {
                        text = text.Replace("<n>", "\r\n");
                        i--;
                    }
                    break;
                case "<f":
                    if (p.GetTag(text, i, "string") == "")
                    {
                        text = text.Replace("<f>", "");
                        i--;
                    }
                    else
                    {
                        text = text.Replace("<f" + p.GetTag(text, i, "string") + ">", "");
                        i--;
                    }
                    break;
                case "<b":
                    if (p.GetTag(text, i, "string") == "")
                    {
                        text = text.Replace("<b>", "");
                        i--;
                    }
                    break;
                case "<i":
                    if (p.GetTag(text, i, "string") == "")
                    {
                        text = text.Replace("<i>", "");
                        i--;
                    }
                    break;
                case "<r":
                    if (p.GetTag(text, i, "string") == "")
                    {
                        text = text.Replace("<r>", "");
                        i--;
                    }
                    break;
                case "<o":
                    if (p.GetTag(text, i, "string") == "")
                    {
                        text = text.Replace("<o>", "");
                        i--;
                    }
                    break;
                case "<u":
                    if (p.GetTag(text, i, "string") == "")
                    {
                        text = text.Replace("<u>", "");
                        i--;
                    }
                    break;
                case "<g":
                    if (p.GetTag(text, i, "string") != "")
                    {
                        text = text.Replace("<g" + p.GetTag(text, i, "string") + ">", "");
                        i--;
                    }
                    break;
                case "<p":
                    if (p.GetTag(text, i, "number") == "")
                    {
                        text = text.Replace("<p>", "");
                        i--;
                    }
                    else
                    {
                        text = text.Replace("<p" + p.GetTag(text, i, "number") + ">", "");
                        i--;
                    }
                    break;
            }
        }

        return text;
    }

}
