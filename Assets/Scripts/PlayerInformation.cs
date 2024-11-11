using UnityEngine;
using UnityEngine.UI;

public class PlayerInformation : MonoBehaviour
{
	// 名前テキスト
	[SerializeField] Text playerNameText;

    // キルテキスト
    [SerializeField] Text killesText;

	// デステキスト
	[SerializeField] Text deathText;

    // 表に名前やキルデス数を表示する
    public void SetPlayerDetailes(string name, int kill, int death)
    {
        playerNameText.text = name;
        killesText.text = kill.ToString();
        deathText.text = death.ToString();
    }
}
