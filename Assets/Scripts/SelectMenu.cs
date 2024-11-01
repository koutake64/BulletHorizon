using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectMenu : MonoBehaviour
{
    public GameObject ConfigPanel;
    public GameObject Buttons;
    public GameObject GameQuitPanel;

    public string SceneName;
    public string SceneName2;

    private void Start()
    {
        CloseUI();
        Buttons.SetActive(true);
    }

    public void CloseUI()
    {
        ConfigPanel.SetActive(false);
        Buttons.SetActive(false);
        GameQuitPanel.SetActive(false);
    }

    public void GameQuitSelect()
    {
        CloseUI();
        GameQuitPanel.SetActive(true);
    }

    public void Retrun()
    {
        CloseUI();
        Buttons.SetActive(true);
    }

    public void GameQuit()
    {
        CloseUI();
        Application.Quit();
    }

    public void SceneChanege()
    {
        SceneManager.LoadScene(SceneName);
    }
    public void SceneChanege2()
    {
        SceneManager.LoadScene(SceneName2);
    }

    public  void openpanel()
    {
        CloseUI();
        ConfigPanel.SetActive(true);
    }
}
