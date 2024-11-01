using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomColor : MonoBehaviour
{
    // 対象オブジェクトのRendererコンポーネント
    private Renderer objRenderer;

    void Start()
    {
        // Rendererコンポーネントを取得
        objRenderer = GetComponent<Renderer>();

        // Rendererコンポーネントが存在するか確認
        if (objRenderer != null)
        {
            // マテリアルの色をランダムに変更
            SetRandomColor();
        }
        else
        {
            Debug.LogError("Rendererコンポーネントが見つかりません！");
        }
    }

    void SetRandomColor()
    {
        // ランダムな色を生成
        Color randomColor = new Color(Random.value, Random.value, Random.value);

        // マテリアルの色を変更
        objRenderer.material.color = randomColor;

        // ログで確認
        Debug.Log("新しい色: " + randomColor);
    }
}
