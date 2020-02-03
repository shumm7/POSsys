using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BarcodeReader : MonoBehaviour
{
    public double TimeOut = 1;
    [SerializeField] private UnityEvent OnFoundData = new UnityEvent();
    private string tempData;
    private string tempStringData;
    public static string Data;
    private DateTime Time;

    void Update()
    {
        if ((DateTime.Now - Time).TotalSeconds > TimeOut)
        {
            tempStringData = "";
        }

        tempData = Input.inputString;
        if (tempData != "")
        {
            tempStringData += tempData.ToString();
            tempStringData = tempStringData.Replace(" ", "");
            Time = DateTime.Now;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (tempStringData.Substring(0, tempStringData.Length - 1) != "")
            {
                Data = tempStringData.Substring(0, tempStringData.Length-1);
                OnFoundData.Invoke();
                tempStringData = "";
            }
        }


    }

    public static void RemoveData()
    {
        Data = "";
    }

}
