using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumPadKey : MonoBehaviour
{
    public Text PriceUI;
    public Text CashUI;
    public Text ChangeUI;

    public void KeyPressed(string _button)
    {
        string ButtonName = "";
        if (_button == "Numpad")
        {
            ButtonName = transform.name;
            PriceUI = transform.parent.parent.parent.Find("メイン画面").Find("明細").Find("合計金額").GetComponent<Text>();
            CashUI = transform.parent.parent.Find("現金").Find("Text").GetComponent<Text>();
            ChangeUI = transform.parent.parent.Find("釣銭").Find("Text").GetComponent<Text>();
        }
        else
        {
            ButtonName = _button;
        }

        string text = CashUI.text.Substring(0, CashUI.text.Length - 2).Replace(",", "");


        if (ButtonName == "00")
        {
            CashUI.text = LintNumber(text + ButtonName) + " 円";
        }
        else if(ButtonName == "Del")
        {
            if (text.Length >= 2)
            {
                CashUI.text = LintNumber(text.Substring(0, text.Length - 1)) + " 円";
            }
            else if (text.Length < 2)
            {
                CashUI.text = "0 円";
            }
        }
        else if (ButtonName == "Clear")
        {
            CashUI.text = "0 円";
        }
        else if(int.Parse(ButtonName)>=0 && int.Parse(ButtonName)<=9)
        {
            CashUI.text = LintNumber(text + ButtonName) + " 円";
        }

        if (int.Parse(CashUI.text.Substring(0, CashUI.text.Length - 2).Replace(",", "")) >= 10000000)
        {
            CashUI.text = "9,999,999 円";
        }

        int Change = int.Parse(CashUI.text.Substring(0, CashUI.text.Length - 2).Replace(",", "")) - int.Parse(PriceUI.text.Substring(0, PriceUI.text.Length - 2).Replace(",", ""));
        if(Change >= 0)
        {
            ChangeUI.color = new Color(0f, 0f, 0f, 1f);
        }
        else
        {
            ChangeUI.color = new Color(1f, 0f, 0f, 1f);
        }
        ChangeUI.text = LintNumber(Change.ToString()) + " 円";
    }

    private string LintNumber(string Num)
    {
        return MarkDecimal(int.Parse(Num));
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
