using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SystemController : MonoBehaviour
{
    public GameObject SettingsForm1;
    public GameObject SettingsForm2;
    public GameObject SettingsForm3;
    int MaxPage = 3;
    public int pageNumber;
    public Text pageNumberText;

    void Start()
    {
        pageNumber = 1;

        setSettingsData();
        setPage();

    }

    public void ButtonClicked(int num)
    {
        if(num == 1) //次ページ
        {
            pageNumber++;
            pageNumber = Number.Range(pageNumber, 1, MaxPage);
        }
        else if(num == -1)
        {
            pageNumber--;
            pageNumber = Number.Range(pageNumber, 1, MaxPage);
        }
        setPage();

    }

    private void setPage()
    {
        switch (pageNumber)
        {
            case 1:
                SettingsForm1.SetActive(true);
                SettingsForm2.SetActive(false);
                SettingsForm3.SetActive(false);
                break;
            case 2:
                SettingsForm1.SetActive(false);
                SettingsForm2.SetActive(true);
                SettingsForm3.SetActive(false);
                break;
            case 3:
                SettingsForm1.SetActive(false);
                SettingsForm2.SetActive(false);
                SettingsForm3.SetActive(true);
                break;
        }
        pageNumberText.text = pageNumber.ToString() + " / " + MaxPage.ToString();
    }

    public void setSettingsData()
    {
        DataLoader.Config temp = GetComponent<DataLoader>().LoadConfig();
        SettingsForm1.transform.Find("StoreName").GetComponent<InputField>().text = temp.StoreName;
        SettingsForm1.transform.Find("Tax").GetComponent<InputField>().text = temp.Tax.ToString();
        SettingsForm1.transform.Find("Encoding").GetComponent<InputField>().text = temp.Encoding;
        SettingsForm1.transform.Find("ScreenResolutionWidth").GetComponent<InputField>().text = temp.ScreenResolutionWidth.ToString();
        SettingsForm1.transform.Find("ScreenResolutionHeight").GetComponent<InputField>().text = temp.ScreenResolutionHeight.ToString();
        SettingsForm1.transform.Find("Fullscreen").GetComponent<Toggle>().isOn = temp.FullScreen;

        SettingsForm2.transform.Find("EnableLINENotify").GetComponent<Toggle>().isOn = temp.EnableLINENotify;
        SettingsForm2.transform.Find("LINENotifyToken").GetComponent<InputField>().text = temp.LINENotifyToken;
        SettingsForm2.transform.Find("LINENotifyPurchaseNotice").GetComponent<Toggle>().isOn = temp.LINENotifyPurchaseNotice;
        SettingsForm2.transform.Find("BarcodeReader").GetComponent<Toggle>().isOn = temp.BarcodeReader;
        SettingsForm2.transform.Find("BarcodeReaderTimeOut").GetComponent<InputField>().text = temp.BarcodeReaderTimeOut.ToString();
        SettingsForm2.transform.Find("Printer").GetComponent<Toggle>().isOn = temp.Printer;
        SettingsForm2.transform.Find("PrinterName").GetComponent<InputField>().text = temp.PrinterName;
        SettingsForm2.transform.Find("SubPrinterName").GetComponent<InputField>().text = temp.SubPrinterName;

        SettingsForm3.transform.Find("PrinterFontFamily").GetComponent<InputField>().text = temp.PrinterFontFamily;
        SettingsForm3.transform.Find("PrinterFontSize").GetComponent<InputField>().text = temp.PrinterFontSize.ToString();
        SettingsForm3.transform.Find("FormatDate").GetComponent<InputField>().text = temp.FormatDate;
        SettingsForm3.transform.Find("FormatTime").GetComponent<InputField>().text = temp.FormatTime;
        SettingsForm3.transform.Find("NumberedTicket").GetComponent<Toggle>().isOn = temp.NumberedTicket;
        SettingsForm3.transform.Find("MaximumTicketNumber").GetComponent<InputField>().text = temp.MaximumTicketNumber.ToString();
        SettingsForm3.transform.Find("NumberedTicketPrintOnMain").GetComponent<Toggle>().isOn = temp.NumberedTicketPrintOnMain;
        SettingsForm3.transform.Find("OrderListReceipt").GetComponent<Toggle>().isOn = temp.OrderListReceipt;
        SettingsForm3.transform.Find("OrderListPrintOnMain").GetComponent<Toggle>().isOn = temp.OrderListPrintOnMain;
    }

    public void saveSettingsData()
    {
        DataLoader.Config temp = GetComponent<DataLoader>().LoadConfig();
        temp.StoreName = SettingsForm1.transform.Find("StoreName").GetComponent<InputField>().text;
        if (SettingsForm1.transform.Find("Tax").GetComponent<InputField>().text != "")
        {
            temp.Tax = Number.ToNumber(SettingsForm1.transform.Find("Tax").GetComponent<InputField>().text);
        }
        else
        {
            temp.Tax = 10;
        }

        if (SettingsForm1.transform.Find("Encoding").GetComponent<InputField>().text!="")
        {
            temp.Encoding = SettingsForm1.transform.Find("Encoding").GetComponent<InputField>().text;
        }
        else
        {
            temp.Encoding = "Shift_JIS";
        }
        if (SettingsForm1.transform.Find("ScreenResolutionWidth").GetComponent<InputField>().text != "") {
            temp.ScreenResolutionWidth = Number.ToNumber(SettingsForm1.transform.Find("ScreenResolutionWidth").GetComponent<InputField>().text);
        }
        else
        {
            temp.ScreenResolutionWidth = 1920;
            temp.ScreenResolutionHeight = 1280;
        }
        if (SettingsForm1.transform.Find("ScreenResolutionHeight").GetComponent<InputField>().text != "")
        {
            temp.ScreenResolutionHeight =Number.ToNumber(SettingsForm1.transform.Find("ScreenResolutionHeight").GetComponent<InputField>().text);
        }
        else
        {
            temp.ScreenResolutionWidth = 1920;
            temp.ScreenResolutionHeight = 1280;
        }
        temp.FullScreen = SettingsForm1.transform.Find("Fullscreen").GetComponent<Toggle>().isOn;


        temp.EnableLINENotify = SettingsForm2.transform.Find("EnableLINENotify").GetComponent<Toggle>().isOn;
        temp.LINENotifyToken = SettingsForm2.transform.Find("LINENotifyToken").GetComponent<InputField>().text;
        temp.LINENotifyPurchaseNotice = SettingsForm2.transform.Find("LINENotifyPurchaseNotice").GetComponent<Toggle>().isOn;
        temp.BarcodeReader = SettingsForm2.transform.Find("BarcodeReader").GetComponent<Toggle>().isOn;
        if (SettingsForm2.transform.Find("BarcodeReaderTimeOut").GetComponent<InputField>().text != "")
        {
            temp.BarcodeReaderTimeOut = double.Parse(SettingsForm2.transform.Find("BarcodeReaderTimeOut").GetComponent<InputField>().text);
        }
        else
        {
            temp.BarcodeReaderTimeOut = 0.1;
        }
        temp.Printer = SettingsForm2.transform.Find("Printer").GetComponent<Toggle>().isOn;
        temp.PrinterName = SettingsForm2.transform.Find("PrinterName").GetComponent<InputField>().text;
        temp.SubPrinterName = SettingsForm2.transform.Find("SubPrinterName").GetComponent<InputField>().text;


        temp.PrinterFontFamily = SettingsForm3.transform.Find("PrinterFontFamily").GetComponent<InputField>().text;
        if (SettingsForm3.transform.Find("PrinterFontSize").GetComponent<InputField>().text != "")
        {
            temp.PrinterFontSize = Number.ToNumber(SettingsForm3.transform.Find("PrinterFontSize").GetComponent<InputField>().text);
        }
        else
        {
            temp.PrinterFontSize = 11;
        }
        temp.FormatDate = SettingsForm3.transform.Find("FormatDate").GetComponent<InputField>().text;
        temp.FormatTime = SettingsForm3.transform.Find("FormatTime").GetComponent<InputField>().text;

        temp.NumberedTicket = SettingsForm3.transform.Find("NumberedTicket").GetComponent<Toggle>().isOn;
        if (SettingsForm3.transform.Find("PrinterFontSize").GetComponent<InputField>().text != "")
        {
            temp.MaximumTicketNumber = Number.ToNumber(SettingsForm3.transform.Find("MaximumTicketNumber").GetComponent<InputField>().text);
        }
        else
        {
            temp.MaximumTicketNumber = 99;
        }
        temp.NumberedTicketPrintOnMain = SettingsForm3.transform.Find("NumberedTicketPrintOnMain").GetComponent<Toggle>().isOn;
        temp.OrderListReceipt = SettingsForm3.transform.Find("OrderListReceipt").GetComponent<Toggle>().isOn;
        temp.OrderListPrintOnMain = SettingsForm3.transform.Find("OrderListPrintOnMain").GetComponent<Toggle>().isOn;

        GetComponent<DataLoader>().SaveConfig(temp);
        setSettingsData();
    }

}
