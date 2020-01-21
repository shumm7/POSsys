using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHolder : MonoBehaviour
{
    public static int PrimaryCashier = 0;
    public static int CurrentCashier = 0;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
