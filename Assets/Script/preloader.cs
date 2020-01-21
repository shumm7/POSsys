using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class preloader : MonoBehaviour
{
    void Start()
    {
        GetComponent<SceneLoader>().LoadScene("Front");
    }

}
