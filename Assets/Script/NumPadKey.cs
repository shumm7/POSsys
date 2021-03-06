﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumPadKey : MonoBehaviour
{
    public static Text TextUI;
    public static InputField InputFieldUI;
    static string mode;
    static int Limit = 10000000;
    public static int Value;

    public static void 対象切り替え(Text _text)
    {
        TextUI = _text;
        if (TextUI.text != "")
        {
            Value = int.Parse(TextUI.text.Replace(",", "").Replace(" 円", "").Replace("￥ ", ""));
        }
        else
        {
            Value = 0;
        }
        mode = "Text";
    }
    public static void 対象切り替え(InputField _text)
    {
        InputFieldUI = _text;
        if (InputFieldUI.text != "")
        {
            try
            {
                Value = int.Parse(InputFieldUI.text.Replace(",", "").Replace(" 円", "").Replace("￥ ", ""));
            }
            catch(System.Exception)
            {
                Value = 0;
            }
        }
        else
        {
            Value = 0;
        }
        mode = "InputField";
    }

    public static void 数値上限設定(int _limit)
    {
        Limit = _limit;
    }

    public void KeyPressed(string _button)
    {
        string text = Value.ToString();
        int ret = 0;

        string ButtonName;
        if (_button == "Numpad")
        {
            ButtonName = transform.name;
        }
        else
        {
            ButtonName = _button;
        }

        if (ButtonName == "00") //00キー
        {
            if (int.Parse(text + ButtonName) < Limit)
            {
                setUI(LintNumber(text + ButtonName) + " 円");
                ret = int.Parse(text + ButtonName);
            }
            else
            {
                setUI(Number.MarkDecimal(Limit-1) + " 円");
                ret = Limit-1;
            }
        }
        else if(ButtonName == "Del") //Del
        {
            if (text.Length >= 2)
            {
                setUI(LintNumber(text.Substring(0, text.Length - 1)) + " 円");
                ret = int.Parse(text.Substring(0, text.Length - 1));

            }
            else if (text.Length < 2)
            {
                setUI("0 円");
                ret = 0;
            }
        }
        else if (ButtonName == "Clear" || ButtonName == "CA") //CA
        {
            setUI("0 円");
            ret = 0;
        }
        else if (ButtonName == "Enter") //CA
        {
            setUI(LintNumber(text) + " 円");
            ret = int.Parse(text);
            Text vd = null;
            対象切り替え(vd);
        }
        else if(int.Parse(ButtonName)>=0 && int.Parse(ButtonName)<=9) //数字キー
        {
            if (int.Parse(text + ButtonName) < Limit)
            {
                setUI(LintNumber(text + ButtonName) + " 円");
                ret = int.Parse(text + ButtonName);
            }
            else
            {
                setUI(Number.MarkDecimal(Limit - 1) + " 円");
                ret = Limit - 1;
            }
        }
        Value = ret;
    }

    private string LintNumber(string number)
    {
        return Number.MarkDecimal(int.Parse(number));
    }

    private void setUI(string text)
    {
        if (mode == "Text")
        {
            if (TextUI != null)
            {
                TextUI.text = text;
            }
        }
        else if(mode=="InputField")
        {
            if (InputFieldUI != null)
            {
                InputFieldUI.text = text;
            }
        }
    }
}
