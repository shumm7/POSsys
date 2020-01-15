using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderButton : MonoBehaviour
{
    public GameObject Controller;
    private int TabMode;

    // Start is called before the first frame update
    void Start()
    {
        TabMode = Controller.GetComponent<GameManager>().TabMode;
    }

    public void TabChanged(int TabMode, int num)
    {
        Debug.Log(num);
    }

    public void ButtonPressed(int num)
    {

    }
}
