using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CheckWindow : MonoBehaviour
{
    Sequence fadeIn;
    public Image Panel;
    public GameObject Window;
    public string scene;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)) 
        {
            if (scene == "")
            {
                QuitGame();
            }
            else
            {
                Quit();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace))
        {
            Back();
        }
    }

    public void WindowAwake()
    {
        gameObject.SetActive(true);
        fadeIn = DOTween.Sequence();
        for (int i = 0; i < 4; i++)
        {
            fadeIn.Insert(0,
                DOTween.ToAlpha(() => Panel.color, a => Panel.color = a, 100f/255f, 0.5f)
            );
        }
        fadeIn.Join(
            DOVirtual.DelayedCall(0.5f, () =>
            {
                Window.SetActive(true);
                DOTween.ToAlpha(() => Panel.color, a => Panel.color = a, 100f / 255f, 0f);
            })
        );

        fadeIn.Play();
    }

    public void Back()
    {
        Window.SetActive(false);
        Sequence fadeOut = DOTween.Sequence();
        for (int i = 0; i < 4; i++)
        {
            fadeOut.Insert(0,
                DOTween.ToAlpha(() => Panel.color, a => Panel.color = a, 0f, 0.5f)
            );
        }
        fadeOut.Join(
            DOVirtual.DelayedCall(0.5f, () =>
            {
                gameObject.SetActive(false);
                Window.SetActive(true);
            })
        );

        fadeOut.Play();
    }

    public void Quit()
    {
        gameObject.SetActive(false);
        GetComponent<SceneLoader>().LoadScene(scene);
    }

    public void QuitGame()
    {
        gameObject.SetActive(false);
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #elif UNITY_STANDALONE
            UnityEngine.Application.Quit();
        #endif
    }
}
