﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.IO;

public class Printer : MonoBehaviour
{

    [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool OpenPrinter(string pPrinterName,
        out IntPtr hPrinter, IntPtr pDefault);

    [DllImport("winspool.drv", SetLastError = true)]
    private static extern bool ClosePrinter(IntPtr hPrinter);

    [DllImport("winspool.drv", SetLastError = true)]
    private static extern bool GetPrinter(IntPtr hPrinter,
        int dwLevel, IntPtr pPrinter, int cbBuf, out int pcbNeeded);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct PRINTER_INFO_2
    {
        public string pServerName;
        public string pPrinterName;
        public string pShareName;
        public string pPortName;
        public string pDriverName;
        public string pComment;
        public string pLocation;
        public IntPtr pDevMode;
        public string pSepFile;
        public string pPrintProcessor;
        public string pDatatype;
        public string pParameters;
        public IntPtr pSecurityDescriptor;
        public uint Attributes;
        public uint Priority;
        public uint DefaultPriority;
        public uint StartTime;
        public uint UntilTime;
        public uint Status;
        public uint cJobs;
        public uint AveragePPM;
    }

    //印刷内容
    private string printingText;
    private System.Drawing.Font printFontDefault;
    //プリンター設定
    public string PrinterName;
    //印刷設定
    public Brush brush = new SolidBrush(System.Drawing.Color.Black);
    public float sx = 0; //印刷開始位置
    public float sy = 0;
    public float allignWidth = 0;
    public float allignHeight = 0;
    public Input TestPrintInput;

    public void OnClicked()
    {
        var ret = Print("test", GetComponent<DataLoader>().LoadConfig().PrinterFontFamily, GetComponent<DataLoader>().LoadConfig().PrinterFontSize, GetComponent<DataLoader>().LoadConfig().PrinterName);

    }

    public bool Print(string Text, string FontFamilyName, float FontSize, string printerName)
    {
        try
        {
            PrinterName = printerName;
            if (!GetPrinterStatus(printerName))
            {
                throw new ArgumentException("Printer " + printerName + " is currently offline.", "original");
            }
            
            printingText = Text;
            printingText = printingText.Replace(Environment.NewLine, "<n>");
            printFontDefault = new System.Drawing.Font(FontFamilyName, FontSize);

            System.Drawing.Printing.PrintDocument pd = new System.Drawing.Printing.PrintDocument();
            pd.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(pd_PrintPage);
            if (PrinterName != "")
                pd.PrinterSettings.PrinterName = PrinterName;

            pd.Print();
            pd.Dispose();
        }
        catch
        {
            Debug.LogWarning("Cannot Print!");
            return false;
        }

        return true;
    }

    private bool GetPrinterStatus(string printerName)
    {
        PRINTER_INFO_2 PrinterInfo = GetPrinterInfo(printerName);
        Debug.Log(PrinterInfo.Status);
        switch (PrinterInfo.Status)
        {
            case 0:
            case 1024:
            case 131072:
                return true;
        }
        return false;
}

    private void pd_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
    {
        try
        {
            System.Drawing.Font printFont = printFontDefault;
            
            //印刷中の場所
            float cx = sx;
            float cy = sy;
            float maxHeight = printFont.Size;

            bool flag = false;
            string tag = "";
            string space = "";
            for (int i = 0; i < printingText.Length; i++) {
                space = printingText.Substring(i, 1);

                if (i + 2 < printingText.Length)
                {
                    tag = printingText.Substring(i, 2);
                }
                else
                {
                    e.Graphics.DrawString(printingText.Substring(i, 1), printFont, brush, cx, cy);
                    cx += e.Graphics.MeasureString(printingText.Substring(i, 1), printFont).Width + allignWidth;
                    e.Graphics.DrawString(printingText.Substring(i + 1, 1), printFont, brush, cx, cy);
                    cx += e.Graphics.MeasureString(printingText.Substring(i+1, 1), printFont).Width + allignWidth;
                    break;
                }

                switch (space)
                {
                    case "　":
                        cx += e.Graphics.MeasureString("　", printFont).Width;
                        break;
                     case " ":
                        cx += e.Graphics.MeasureString(" ", printFont).Width;
                        break;
                }

                switch (tag)
                {
                    case "<s": //文字サイズ（引数：サイズ）
                        if (GetTag(printingText, i, "number") == "")
                        {
                            printFont = new System.Drawing.Font(printFont.FontFamily, printFontDefault.Size, printFont.Style);
                            i += 2;
                            flag = true;
                        }
                        else
                        {
                            printFont = new System.Drawing.Font(printFont.FontFamily, int.Parse(GetTag(printingText, i, "number")), printFont.Style);
                            i += (2 + GetTag(printingText, i, "number").Length);
                            flag = true;
                        }
                        break;
                    case "<n": //改行（引数：なし）
                        if (GetTag(printingText, i, "string") == "")
                        {
                            cy += maxHeight + allignHeight;
                            cx = sx;
                            maxHeight = printFont.Size;
                            i += 2;
                            flag = true;
                        }
                        break;
                    case "<f": //フォント（引数：FontFamily）
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
                    case "<x": //座標X（引数：相対座標）
                        if (GetTag(printingText, i, "number") == "")
                        {
                            cx = sx;
                            i += 2;
                            flag = true;
                        }
                        else
                        {
                            cx += int.Parse(GetTag(printingText, i, "number"));
                            i += (2 + GetTag(printingText, i, "number").Length);
                            flag = true;
                        }
                        break;
                    case "<y": //座標Y（引数：相対座標）
                        if (GetTag(printingText, i, "number") == "")
                        {
                            cy = sy;
                            i += 2;
                            flag = true;
                        }
                        else
                        {
                            cy += int.Parse(GetTag(printingText, i, "number"));
                            i += (2 + GetTag(printingText, i, "number").Length);
                            flag = true;
                        }
                        break;
                    case "<X": //座標X（引数：絶対座標）
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
                    case "<Y": //座標Y（引数：絶対座標）
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
                    case "<b": //太字（引数：なし）
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
                    case "<i"://斜体（引数：なし）
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
                    case "<r": //文字装飾クリア（引数：なし）
                        if (GetTag(printingText, i, "string") == "")
                        {
                            printFont = new System.Drawing.Font(printFont.FontFamily, printFont.Size, System.Drawing.FontStyle.Regular);
                            i += 2;
                            flag = true;
                        }
                        break;
                    case "<o": //打ち消し線（引数：なし）
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
                    case "<u": //下線（引数：なし）
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
                    case "<g": //画像（引数：ディレクトリ）
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
                    case "<p": //直線（引数：太さ）
                        if (GetTag(printingText, i, "number") == "")
                        {
                            cx = sx;
                            e.Graphics.DrawLine(Pens.Black, sx, cy + allignHeight / 2, e.PageSettings.PaperSize.Width, cy + allignHeight / 2);
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

                if (printFont.Size > maxHeight)
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
        catch
        {
            Debug.LogError("Cannot Print!");
        }
    }

    public string GetTag(string Text, int StartTag, string mode)
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

    public static PRINTER_INFO_2 GetPrinterInfo(string printerName)
    {
        IntPtr hPrinter;
        if (!OpenPrinter(printerName, out hPrinter, IntPtr.Zero))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        IntPtr pPrinterInfo = IntPtr.Zero;
        try
        {
            int needed;
            GetPrinter(hPrinter, 2, IntPtr.Zero, 0, out needed);
            if (needed <= 0)
            {
                throw new Exception("失敗しました。");
            }

            pPrinterInfo = Marshal.AllocHGlobal(needed);

            int temp;
            if (!GetPrinter(hPrinter, 2, pPrinterInfo, needed, out temp))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            PRINTER_INFO_2 printerInfo = (PRINTER_INFO_2)Marshal.PtrToStructure(pPrinterInfo, typeof(PRINTER_INFO_2));

            return printerInfo;
        }
        finally
        {
            //後始末をする
            ClosePrinter(hPrinter);
            Marshal.FreeHGlobal(pPrinterInfo);
        }
    }
}
