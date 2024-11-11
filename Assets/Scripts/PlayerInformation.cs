using UnityEngine;
using UnityEngine.UI;

public class PlayerInformation : MonoBehaviour
{
	[SerializeField, Header("名前テキスト")] Text playerNameText;
    [SerializeField, Header("キルテキスト")] Text killesText;
	[SerializeField, Header("デステキスト")] Text deathText;

    // 表に名前やキルデス数を表示する
    public void SetPlayerDetailes(string name, int kill, int death)
    {
        playerNameText.text = name;
        killesText.text = kill.ToString();
        deathText.text = death.ToString();
    }
}
