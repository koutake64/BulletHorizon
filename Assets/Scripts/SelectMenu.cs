using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectMenu : MonoBehaviour
{
    [SerializeField] GameObject ConfigPanel;
    [SerializeField] GameObject Buttons;
	[SerializeField] GameObject GameQuitPanel;

	[SerializeField, SceneSelector] string SceneName;
	[SerializeField, SceneSelector] string SceneName2;

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

    public  void Openpanel()
    {
        CloseUI();
        ConfigPanel.SetActive(true);
    }
}
