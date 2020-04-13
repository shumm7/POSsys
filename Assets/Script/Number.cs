using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Number : MonoBehaviour
{

    public static string LintNumber(int num)
    {
        if (num < 10)
        {
            return "0" + num.ToString();
        }
        else
        {
            return num.ToString();
        }
    }

    public static string MarkDecimal(int price)
    {
        int _price = price;
        if (_price < 1000 && _price >=0)
        {
            return _price.ToString();
        }
        else if(_price >=1000)
        {
            string text = _price.ToString();
            string ret = "";
            int count = 0;
            for (int i = 0; i < text.Length; i++)
            {
                ret = text.Substring(text.Length - i - 1, 1) + ret;
                count++;
                if (count == 3 && i != text.Length - 1)
                {
                    ret = "," + ret;
                    count = 0;
                }
            }

            return ret;
        }
        else
        {
            string text = (-_price).ToString();
            string ret = "";
            int count = 0;
            for (int i = 0; i < text.Length; i++)
            {
                ret = text.Substring(text.Length - i - 1, 1) + ret;
                count++;
                if (count == 3 && i != text.Length - 1)
                {
                    ret = "," + ret;
                    count = 0;
                }
            }

            return "- " + ret;
        }
    }

    public static string MarkDecimal(string price)
    {
        int _price;
        try
        {
            _price = int.Parse(price);
        }
        catch(System.Exception)
        {
            _price = 0;
        }

        if (_price < 1000 && _price >= 0)
        {
            return _price.ToString();
        }
        else if (_price >= 1000)
        {
            string text = _price.ToString();
            string ret = "";
            int count = 0;
            for (int i = 0; i < text.Length; i++)
            {
                ret = text.Substring(text.Length - i - 1, 1) + ret;
                count++;
                if (count == 3 && i != text.Length - 1)
                {
                    ret = "," + ret;
                    count = 0;
                }
            }

            return ret;
        }
        else
        {
            string text = (-_price).ToString();
            string ret = "";
            int count = 0;
            for (int i = 0; i < text.Length; i++)
            {
                ret = text.Substring(text.Length - i - 1, 1) + ret;
                count++;
                if (count == 3 && i != text.Length - 1)
                {
                    ret = "," + ret;
                    count = 0;
                }
            }

            return "- " + ret;
        }
    }

    public static bool CheckNumber(string num)
    {
        try
        {
            double temp = double.Parse(num);
        }
        catch (System.Exception)
        {
            return false;
        }
        return true;
    }

    public static int Range(int num, int min, int max)
    {
        int res = num;
        if (res > max)
        {
            res = min;
        }
        else if (res < min)
        {
            res = max;
        }
        return res;
    }

    public static int InsideTax(int price, int tax)
    {
        return price * tax / (100 + tax);
    }

    public static int ToNumber(string Number)
    {
        if (CheckNumber(Number))
        {
            try
            {
                return int.Parse(Number);
            }
            catch (System.Exception)
            {
            }
            return 0;
        }
        else
        {
            return 0;
        }
    }

    public static string FormatDate(int year, int month, int day)
    {
        string res = "";

        if (year < 1000)
        {
            for (int i = 0; i < year.ToString().Length; i++)
                res += "0";
            res += year.ToString();
        }
        else
        {
            res += year.ToString();
        }

        if (month < 10)
        {
            res += "0" + month.ToString();
        }
        else
        {
            res += month.ToString();
        }

        if (day < 10)
        {
            res += "0" + day.ToString();
        }
        else
        {
            res += day.ToString();
        }

        return res;
    }
}
