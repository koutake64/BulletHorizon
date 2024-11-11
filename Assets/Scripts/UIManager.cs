using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField, Header("玉表示テキスト")] Text ammoText;
    [SerializeField, Header("死亡時テキスト")] Text deathText;
	[SerializeField, Header("キルログ表示テキスト")] Text killLogText;
    [SerializeField, Header("HPスライダー")] Slider HPSlider;
	
	[Header("各パネル設定")]
	[SerializeField] GameObject deathPanel;
    [SerializeField] GameObject scoreboard;
	[SerializeField] GameObject endPanel;

	public PlayerInformation info;


	// 弾薬テキストを設定
	public void SettingBulletsText(int ammoClip, int ammunition)
    {
        ammoText.text = ammoClip + "/" + ammunition;
    }

    // HPを更新
    public void UpdateHP(int maxHP, int currentHP)
    {
        HPSlider.maxValue = maxHP;
        HPSlider.value = currentHP;
    }

    // デスUIを更新
    public void UpdateDeathUI(string name)
    {
        deathPanel.SetActive(true);
        deathText.text = name + "に倒された。";
        Invoke("CloseDeathUI", 5f);
    }

	public void UpdateFallDeathUI()
	{
		deathPanel.SetActive(true);
		deathText.text = "落下死した。";
		Invoke("CloseDeathUI", 5f);
	}

	// デスUIを閉じる
	public void CloseDeathUI()
    {
        deathPanel.SetActive(false);
    }

    // スコアボードを開く
    public void ChangeScoreUI()
    {
        scoreboard.SetActive(!scoreboard.activeInHierarchy);
    }

    // 終了パネルを開く
    public void OpenEndPanel()
    {
        endPanel.SetActive(true);
    }

    // キルログを更新
    public void UpdateKillLog(string killer, string victim)
    {
        killLogText.text += $"{killer} が {victim} を倒した\n";
        Invoke("ClearKillLog", 5f); // 5秒後にログをクリアする
    }

    // キルログをクリア
    private void ClearKillLog()
    {
        killLogText.text = "";
    }
}
