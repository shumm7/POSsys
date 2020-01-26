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

    public static string MarkDecimal(int _price)
    {
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

}
