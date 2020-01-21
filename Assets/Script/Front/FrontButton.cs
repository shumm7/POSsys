using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontButton : MonoBehaviour
{
    public GameObject CheckWindow;

    public void OnClick(int num)
    {
        switch (num)
        {
            case 0: //注文・会計
                GetComponent<SceneLoader>().LoadScene("Order");
                break;
            case 1:
                GetComponent<SceneLoader>().LoadScene("Check");
                break;
            case 2:
                GetComponent<SceneLoader>().LoadScene("Sales");
                break;
            case 3:
                GetComponent<SceneLoader>().LoadScene("Goods");
                break;
            case 4:
                GetComponent<SceneLoader>().LoadScene("System");
                break;
        }
    }
}
