using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleTap : MonoBehaviour
{
    //public GameObject Button;
    public string SceneName;
    public Image fadePanel;
    public GameObject fadePanelPrefab;
    public float fadeDuration = 1.0f;

    private void Start()
    {
        //StartCoroutine(FadeOutAndLoadScene());
    }

    public void ChangeScene()
    {
        StartCoroutine(FadeOutAndLoadScene());
        //SceneManager.LoadScene(SceneName);
    }

    public IEnumerator FadeOutAndLoadScene()
    {
        fadePanelPrefab.SetActive(true);
        fadePanel.enabled = true;                 // パネルを有効化
        float elapsedTime = 0.0f;                 // 経過時間を初期化
        Color startColor = fadePanel.color;       // フェードパネルの開始色を取得
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 1.0f); // フェードパネルの最終色を設定

        // フェードアウトアニメーションを実行
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;                        // 経過時間を増やす
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);  // フェードの進行度を計算
            fadePanel.color = Color.Lerp(startColor, endColor, t); // パネルの色を変更してフェードアウト
            yield return null;                                     // 1フレーム待機
        }

        fadePanel.color = endColor;  // フェードが完了したら最終色に設定
        SceneManager.LoadScene(SceneName); // シーンをロードしてメニューシーンに遷移
    }
}
