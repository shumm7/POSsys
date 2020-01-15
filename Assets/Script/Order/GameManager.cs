using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text TimeUI;
    public GameObject Tabs;
    public GameObject OrderButton;
    private DataLoader.List productList;
    public int TabMode = 0;

    void Start()
    {
        productList = GetComponent<DataLoader>().LoadList();
        for(int i = 1; i<=4; i++)
        {
            Tabs.transform.Find(i.ToString()).transform.Find("Text").GetComponent<Text>().text = productList.CategoryName[i - 1];
            Tabs.transform.Find(i.ToString()).gameObject.GetComponent<Button>().interactable = true;
        }
        Tabs.transform.Find("1").gameObject.GetComponent<Button>().interactable = false;
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

    public void TabButtonClicked(int num)
    {
        TabMode = num;
        for (int i = 0; i < 6; i++)
        {
            OrderButton.transform.Find((i + 1).ToString()).gameObject.GetComponent<OrderButton>().TabChanged(TabMode, i + 1);
        }
        for(int i=0; i<4; i++)
        {
            Tabs.transform.Find((i+1).ToString()).gameObject.GetComponent<Button>().interactable = true;
        }
        Tabs.transform.Find((num+1).ToString()).gameObject.GetComponent<Button>().interactable = false;
    }
}
