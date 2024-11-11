using UnityEngine;
using UnityEngine.SceneManagement;

public class Offline : MonoBehaviour
{
    [SerializeField] GameObject PausePanel;
    [SerializeField] GameObject Buttons;
	[SerializeField, SceneSelector] string SceneName;

//===== Hide --------------------------------------------------------------------------------------------------------------------
    private OfflinePlayer player;
	private bool Panel;


    private void Start()
    {
        PausePanel.SetActive(false);
        Buttons.SetActive(false);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<OfflinePlayer>(); // オフラインプレイヤーの取得
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!Panel)
            {
                Panel = true;
                Time.timeScale = 0f;
                PausePanel.SetActive(true);
                Buttons.SetActive(true);
                player.SetPauseState(true); // ポーズ状態に設定
            }
            else
            {
                Panel = false;
                Time.timeScale = 1f;
                PausePanel.SetActive(false);
                Buttons.SetActive(false);
                player.SetPauseState(false); // ポーズ解除状態に設定
            }
        }
    }

    public void Retrun()
    {
        Panel = false;
        Time.timeScale = 1f;
        PausePanel.SetActive(false);
        Buttons.SetActive(false);
        player.SetPauseState(false); // ポーズ解除状態に設定
    }


    public void Title()
    {
        SceneManager.LoadScene(SceneName);
    }
}
