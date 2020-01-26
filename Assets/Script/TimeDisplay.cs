using UnityEngine;
using UnityEngine.UI;
using System;

public class TimeDisplay : MonoBehaviour
{
    public Text UI;
    public bool 年表示;

    void Start()
    {
        if (UI == null)
        {
            UI = this.gameObject.GetComponent<Text>();
        }
    }

    void Update()
    {
        if (年表示) 
        {
            UI.text = DateTime.Now.ToString("yyyy年MM月dd日 HH時mm分");
        }
        else
        {
            UI.text = DateTime.Now.ToString("MM月dd日 HH時mm分");
        }
    }
}
