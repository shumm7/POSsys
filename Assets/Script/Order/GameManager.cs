using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text TimeUI;
    public GameObject Tabs;
    private DataLoader.List productList;

    void Start()
    {
        productList = GetComponent<DataLoader>().LoadList();
        Debug.Log(productList.CategoryName[1]);
        for(int i = 1; i<=4; i++)
        {
            Tabs.transform.Find(i.ToString()).transform.Find("Text").GetComponent<Text>().text = productList.CategoryName[i - 1];
        }
    }

    // Update is called once per frame
    void Update()
    {
        DateTime Time = DateTime.Now;
        TimeUI.text = LintNumber(Time.Month) + "月" + LintNumber(Time.Day) + "日　" + LintNumber(Time.Hour) + "：" + LintNumber(Time.Minute);
    }

    private string LintNumber(int num)
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
}
