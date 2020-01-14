#define USE_JAGGED

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

#if USE_JAGGED
[System.Serializable]

public class FrontSceneChange : MonoBehaviour
{
    public Text CurrentTime;
    public Text StoreName;

    void Start()
    {
        string Name;
        if(GetComponent<DataLoader>().checkExist(@"config.json")==false)
        {
            DataLoader.Config temp = new DataLoader.Config();
            GetComponent<DataLoader>().SaveConfig(temp);
            Name = GetComponent<DataLoader>().LoadConfig().StoreName;
        }
        else
        {
            Name = GetComponent<DataLoader>().LoadConfig().StoreName;
        }

        if (GetComponent<DataLoader>().checkExist(@"list.json") == false)
        {
            DataLoader.List temp = new DataLoader.List();
            for(int i = 0; i < 4; i++)
            {
                for(int j=0; j< temp.MAX_PRODUCT_AMOUNT; j++)
                {
                    temp.ProductName[i][j] = "";
                }
            }
            GetComponent<DataLoader>().SaveList(temp);

        }

        StoreName.text = Name + " 管理画面";
    }

    // Update is called once per frame
    void Update()
    {
        DateTime Time = DateTime.Now;
        CurrentTime.text = Time.Year.ToString() + "年" + LintNumber(Time.Month) + "月" + LintNumber(Time.Day) + "日（" + GetDayOfTheWeek(Time) + "）" + LintNumber(Time.Hour) + "時" + LintNumber(Time.Minute) + "分";
    }

    private string LintNumber(int num)
    {
        if(num < 10)
        {
            return "0" + num.ToString();
        }
        else
        {
            return num.ToString();
        }
    }

    private string GetDayOfTheWeek(System.DateTime dateTime)
    {
        string japanese = "";
        switch (dateTime.DayOfWeek)
        {
            case DayOfWeek.Sunday:
                japanese = "日";
                break;
            case DayOfWeek.Monday:
                japanese = "月";
                break;
            case DayOfWeek.Tuesday:
                japanese = "火";
                break;
            case DayOfWeek.Wednesday:
                japanese = "水";
                break;
            case DayOfWeek.Thursday:
                japanese = "木";
                break;
            case DayOfWeek.Friday:
                japanese = "金";
                break;
            case DayOfWeek.Saturday:
                japanese = "土";
                break;
        }

        return japanese;
    }
}
#endif