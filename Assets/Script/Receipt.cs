using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

        foreach (string line in Text)
        {
            c.AddLine(@Filename, line);
        }
    }
}
