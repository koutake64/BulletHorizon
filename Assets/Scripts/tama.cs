using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tama : MonoBehaviour
{
    float x = 0;
    GameObject obj;
    float timer = 0.0f;
    public float interval = 0.5f; // 3秒間隔

    // Start is called before the first frame update
    void Start()
    {
        // CubeプレハブをGameObject型で取得
        obj = (GameObject)Resources.Load("tama");
    }

    // Update is called once per frame
    void Update()
    {
        // 経過時間を更新
        timer += Time.deltaTime;

        // timerがintervalを超えたら処理を実行
        if (timer >= interval)
        {
            x = Random.Range(-85.0f, 70.0f);
            GameObject newObj = Instantiate(obj, new Vector3(x, 80.0f, 90.0f), Quaternion.identity);

            // 新しいオブジェクトに移動と消去の処理を追加
            StartCoroutine(MoveAndDestroy(newObj));

            // timerをリセット
            timer = 0.0f;
        }
    }

    IEnumerator MoveAndDestroy(GameObject obj)
    {
        // 5秒待つ
        yield return new WaitForSeconds(10.0f);

        // y軸に10上げる
        if (obj != null)
        {
            obj.transform.position += new Vector3(2, 15, 0);
        }

        // さらに2秒待つ
        yield return new WaitForSeconds(2.0f);

        // オブジェクトを消去
        if (obj != null)
        {
            Destroy(obj);
        }
    }
}
