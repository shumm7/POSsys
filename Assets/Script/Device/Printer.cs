using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

public class Printer : MonoBehaviour
{
    //印刷内容
    private string printingText;
    private System.Drawing.Font printFontDefault;
    //プリンター設定
    public string PrinterName;
    //印刷設定
    public Brush brush = new SolidBrush(System.Drawing.Color.Black);
    public float sx = 0; //印刷開始位置
    public float sy = 0;
    public float allignWidth = -5;
    public float allignHeight = 5;

    public void Print(string FilePath, string FontFamilyName, float FontSize)
    {
        printingText = new StreamReader(@FilePath, false).ReadToEnd();
        printingText = "<l15>";
        printingText = printingText.Replace(System.Environment.NewLine, "");
        printFontDefault = new System.Drawing.Font(FontFamilyName, FontSize);

        System.Drawing.Printing.PrintDocument pd = new System.Drawing.Printing.PrintDocument();
        pd.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(pd_PrintPage);
        if(PrinterName!="")
            pd.PrinterSettings.PrinterName = PrinterName;

        pd.Print();
    }

    private void pd_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
    {
        System.Drawing.Font printFont = printFontDefault;
        //印刷中の場所
        float cx = sx;
        float cy = sy;
        float maxHeight = printFont.Size;
        Debug.Log(maxHeight);
        bool flag = false;

        string tag = "";
        for(int i=0; i<printingText.Length; i++) {
            if (i + 2 < printingText.Length)
            {
                tag = printingText.Substring(i, 2);
            }
            else
            {
                e.Graphics.DrawString(printingText.Substring(i, 1), printFont, brush, cx, cy);
                cx += e.Graphics.MeasureString(printingText.Substring(i, 1), printFont).Width + allignWidth;
                e.Graphics.DrawString(printingText.Substring(i+1, 1), printFont, brush, cx, cy);
                cx += e.Graphics.MeasureString(printingText.Substring(i, 1), printFont).Width + allignWidth;
                break;
            }

            switch (tag)
            {
                case "<s":
                    if(GetTag(printingText, i, "number") == "")
                    {
                        printFont = new System.Drawing.Font(printFont.FontFamily, printFontDefault.Size, printFont.Style);
                        i += 2;
                        flag = true;
                    }
                    else
                    {
                        printFont = new System.Drawing.Font(printFont.FontFamily, int.Parse(GetTag(printingText, i, "number")), printFont.Style);
                        i+=(2+GetTag(printingText, i, "number").Length);
                        flag = true;
                    }
                    break;
                case "<n":
                    if (GetTag(printingText, i, "string") == "")
                    {
                        cy += maxHeight + allignHeight;
                        cx = sx;
                        maxHeight = printFont.Size;
                        i += 2;
                        flag = true;
                    }
                    break;
                case "<f":
                    if (GetTag(printingText, i, "string") == "")
                    {
                        printFont = new System.Drawing.Font(printFontDefault.FontFamily, printFont.Size, printFont.Style);
                        i += 2;
                        flag = true;
                    }
                    else
                    {
                        printFont = new System.Drawing.Font(GetTag(printingText, i, "string"), printFont.Size, printFont.Style);
                        i += (2 + GetTag(printingText, i, "string").Length);
                        flag = true;
                    }
                    break;
                case "<x":
                    if (GetTag(printingText, i, "number") == "")
                    {
                        cx = sx;
                        i += 2;
                        flag = true;
                    }
                    else
                    {
                        cx = int.Parse(GetTag(printingText, i, "number"));
                        i += (2 + GetTag(printingText, i, "number").Length);
                        flag = true;
                    }
                    break;
                case "<y":
                    if (GetTag(printingText, i, "number") == "")
                    {
                        cy = sy;
                        i += 2;
                        flag = true;
                    }
                    else
                    {
                        cy = int.Parse(GetTag(printingText, i, "number"));
                        i += (2 + GetTag(printingText, i, "number").Length);
                        flag = true;
                    }
                    break;
                case "<b":
                    if (GetTag(printingText, i, "string") == "")
                    {
                        if (printFont.Bold)
                        {
                            printFont = new System.Drawing.Font(printFont.FontFamily, printFont.Size, printFont.Style & ~System.Drawing.FontStyle.Bold);
                        }
                        else
                        {
                            printFont = new System.Drawing.Font(printFont.FontFamily, printFont.Size, printFont.Style | System.Drawing.FontStyle.Bold);
                        }
                        i += 2;
                        flag = true;
                    }
                    break;
                case "<i":
                    if (GetTag(printingText, i, "string") == "")
                    {
                        if (printFont.Italic)
                        {
                            printFont = new System.Drawing.Font(printFont.FontFamily, printFont.Size, printFont.Style & ~System.Drawing.FontStyle.Italic);
                        }
                        else
                        {
                            printFont = new System.Drawing.Font(printFont.FontFamily, printFont.Size, printFont.Style | System.Drawing.FontStyle.Italic);
                        }
                        i += 2;
                        flag = true;
                    }
                    break;
                case "<r":
                    if (GetTag(printingText, i, "string") == "")
                    {
                        printFont = new System.Drawing.Font(printFont.FontFamily, printFont.Size, System.Drawing.FontStyle.Regular);
                        i += 2;
                        flag = true;
                    }
                    break;
                case "<o":
                    if (GetTag(printingText, i, "string") == "")
                    {
                        if (printFont.Strikeout)
                        {
                            printFont = new System.Drawing.Font(printFont.FontFamily, printFont.Size, printFont.Style & ~System.Drawing.FontStyle.Strikeout);
                        }
                        else
                        {
                            printFont = new System.Drawing.Font(printFont.FontFamily, printFont.Size, printFont.Style | System.Drawing.FontStyle.Strikeout);
                        }
                        i += 2;
                        flag = true;
                    }
                    break;
                case "<u":
                    if (GetTag(printingText, i, "string") == "")
                    {
                        if (printFont.Underline)
                        {
                            printFont = new System.Drawing.Font(printFont.FontFamily, printFont.Size, printFont.Style & ~System.Drawing.FontStyle.Underline);
                        }
                        else
                        {
                            printFont = new System.Drawing.Font(printFont.FontFamily, printFont.Size, printFont.Style | System.Drawing.FontStyle.Underline);
                        }
                        i += 2;
                        flag = true;
                    }
                    break;
                case "<g":
                    if (GetTag(printingText, i, "string") != "")
                    {
                        Image img = Image.FromFile(@GetTag(printingText, i, "string"));
                        cy += allignHeight;
                        e.Graphics.DrawImage(img, cx, cy);
                        cy += allignHeight + img.Height;
                        img.Dispose();
                        i += (2 + GetTag(printingText, i, "string").Length);
                        flag = true;
                    }
                    break;
                case "<l":
                    if (GetTag(printingText, i, "number") == "")
                    {
                        cx = sx;
                        e.Graphics.DrawLine(Pens.Black, sx, cy + allignHeight/2, e.PageSettings.PaperSize.Width, cy + allignHeight/2);
                        cy += allignHeight + allignHeight;
                        i += 2;
                        flag = true;
                    }
                    else if (GetTag(printingText, i, "number") != "")
                    {
                        cx = sx;
                        for (int j = 0; j < int.Parse(GetTag(printingText, i, "number")); j++)
                        {
                            e.Graphics.DrawLine(Pens.Black, sx, cy + allignHeight / 2, e.PageSettings.PaperSize.Width, cy + allignHeight / 2);
                            cy++;
                        }
                            cy += allignHeight + allignHeight;
                        i += 2 + GetTag(printingText, i, "number").Length;
                        flag = true;
                    }
                    break;

            }
            
            if(printFont.Size> maxHeight)
            {
                maxHeight = printFont.Size;
            }
            if (!flag)
            {
                e.Graphics.DrawString(printingText.Substring(i, 1), printFont, brush, cx, cy);
                cx += e.Graphics.MeasureString(printingText.Substring(i, 1), printFont).Width + allignWidth;
            }
            flag = false;
        }
    }

    private string GetTag(string Text, int StartTag, string mode)
    {
        if(Text.Substring(StartTag+2, 1) == ">")
        {
            return "";
        }
        else
        {
            for(int i=StartTag+2; i<Text.Length; i++)
            {
                if(Text.Substring(i, 1) == ">")
                {
                    if (mode == "string")
                    {
                        return Text.Substring(StartTag + 2, i - StartTag - 2);
                    }
                    else if(mode == "number")
                    {
                        try
                        {
                            int num = int.Parse(Text.Substring(StartTag + 2, i - StartTag - 2));
                            return num.ToString();
                        }
                        catch(System.Exception)
                        {
                            return "";
                        }
                    }
                }
            }
            return "";
        }
    }
}
