using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tama : MonoBehaviour
{
    float x = 0;
    GameObject obj;
    float timer = 0.0f;
    public float interval = 0.5f; // 3�b�Ԋu

    // Start is called before the first frame update
    void Start()
    {
        // Cube�v���n�u��GameObject�^�Ŏ擾
        obj = (GameObject)Resources.Load("tama");
    }

    // Update is called once per frame
    void Update()
    {
        // �o�ߎ��Ԃ��X�V
        timer += Time.deltaTime;

        // timer��interval�𒴂����珈�������s
        if (timer >= interval)
        {
            x = Random.Range(-85.0f, 70.0f);
            GameObject newObj = Instantiate(obj, new Vector3(x, 80.0f, 90.0f), Quaternion.identity);

            // �V�����I�u�W�F�N�g�Ɉړ��Ə����̏�����ǉ�
            StartCoroutine(MoveAndDestroy(newObj));

            // timer�����Z�b�g
            timer = 0.0f;
        }
    }

    IEnumerator MoveAndDestroy(GameObject obj)
    {
        // 5�b�҂�
        yield return new WaitForSeconds(10.0f);

        // y����10�グ��
        if (obj != null)
        {
            obj.transform.position += new Vector3(2, 15, 0);
        }

        // �����2�b�҂�
        yield return new WaitForSeconds(2.0f);

        // �I�u�W�F�N�g������
        if (obj != null)
        {
            Destroy(obj);
        }
    }
}
