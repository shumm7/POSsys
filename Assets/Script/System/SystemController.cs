using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SystemController : MonoBehaviour
{
    public GameObject SettingsForm;

    void Start()
    {
        setSettingsData();
    }

    public void setSettingsData()
    {
        DataLoader.Config temp = GetComponent<DataLoader>().LoadConfig();
        SettingsForm.transform.Find("StoreName").GetComponent<InputField>().text = temp.StoreName;
        SettingsForm.transform.Find("Tax").GetComponent<InputField>().text = temp.Tax.ToString();
        SettingsForm.transform.Find("Encoding").GetComponent<InputField>().text = temp.Encoding;
        SettingsForm.transform.Find("ScreenResolutionWidth").GetComponent<InputField>().text = temp.ScreenResolutionWidth.ToString();
        SettingsForm.transform.Find("ScreenResolutionHeight").GetComponent<InputField>().text = temp.ScreenResolutionHeight.ToString();
        SettingsForm.transform.Find("Fullscreen").GetComponent<Toggle>().isOn = temp.FullScreen;
        SettingsForm.transform.Find("EnableLINENotify").GetComponent<Toggle>().isOn = temp.EnableLINENotify;
        SettingsForm.transform.Find("LINENotifyToken").GetComponent<InputField>().text = temp.LINENotifyToken;
        SettingsForm.transform.Find("LINENotifyPurchaseNotice").GetComponent<Toggle>().isOn = temp.LINENotifyPurchaseNotice;
        SettingsForm.transform.Find("BarcodeReader").GetComponent<Toggle>().isOn = temp.BarcodeReader;
        SettingsForm.transform.Find("BarcodeReaderTimeOut").GetComponent<InputField>().text = temp.BarcodeReaderTimeOut.ToString();
        SettingsForm.transform.Find("Printer").GetComponent<Toggle>().isOn = temp.Printer;
        SettingsForm.transform.Find("PrinterName").GetComponent<InputField>().text = temp.PrinterName;
    }

    public void saveSettingsData()
    {
        DataLoader.Config temp = GetComponent<DataLoader>().LoadConfig();
        temp.StoreName = SettingsForm.transform.Find("StoreName").GetComponent<InputField>().text;
        if (SettingsForm.transform.Find("Tax").GetComponent<InputField>().text != "")
        {
            temp.Tax = int.Parse(SettingsForm.transform.Find("Tax").GetComponent<InputField>().text);
        }
        else
        {
            temp.Tax = 10;
        }

        if (SettingsForm.transform.Find("Encoding").GetComponent<InputField>().text!="")
        {
            temp.Encoding = SettingsForm.transform.Find("Encoding").GetComponent<InputField>().text;
        }
        else
        {
            temp.Encoding = "Shift_JIS";
        }
        if (SettingsForm.transform.Find("ScreenResolutionWidth").GetComponent<InputField>().text != "") {
            temp.ScreenResolutionWidth = int.Parse(SettingsForm.transform.Find("ScreenResolutionWidth").GetComponent<InputField>().text);
        }
        else
        {
            temp.ScreenResolutionWidth = 1920;
            temp.ScreenResolutionHeight = 1280;
        }
        if (SettingsForm.transform.Find("ScreenResolutionHeight").GetComponent<InputField>().text != "")
        {
            temp.ScreenResolutionHeight = int.Parse(SettingsForm.transform.Find("ScreenResolutionHeight").GetComponent<InputField>().text);
        }
        else
        {
            temp.ScreenResolutionWidth = 1920;
            temp.ScreenResolutionHeight = 1280;
        }
        temp.FullScreen = SettingsForm.transform.Find("Fullscreen").GetComponent<Toggle>().isOn;
        temp.EnableLINENotify = SettingsForm.transform.Find("EnableLINENotify").GetComponent<Toggle>().isOn;
        temp.LINENotifyToken = SettingsForm.transform.Find("LINENotifyToken").GetComponent<InputField>().text;
        temp.LINENotifyPurchaseNotice = SettingsForm.transform.Find("LINENotifyPurchaseNotice").GetComponent<Toggle>().isOn;
        temp.BarcodeReader = SettingsForm.transform.Find("BarcodeReader").GetComponent<Toggle>().isOn;
        if (SettingsForm.transform.Find("BarcodeReaderTimeOut").GetComponent<InputField>().text != "")
        {
            temp.BarcodeReaderTimeOut = double.Parse(SettingsForm.transform.Find("BarcodeReaderTimeOut").GetComponent<InputField>().text);
        }
        else
        {
            temp.BarcodeReaderTimeOut = 0.1;
        }
        temp.Printer = SettingsForm.transform.Find("Printer").GetComponent<Toggle>().isOn;
        temp.PrinterName = SettingsForm.transform.Find("PrinterName").GetComponent<InputField>().text;

        GetComponent<DataLoader>().SaveConfig(temp);
        setSettingsData();
    }

}
