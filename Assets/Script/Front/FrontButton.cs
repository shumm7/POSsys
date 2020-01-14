using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontButton : MonoBehaviour
{
    public void OnClick(int num)
    {
        switch (num)
        {
            case 0: //注文・会計
                GetComponent<SceneLoader>().LoadScene("Order");
                break;
            case 1: 
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
        }
    }
}
