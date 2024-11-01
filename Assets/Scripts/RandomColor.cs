using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomColor : MonoBehaviour
{
    // �ΏۃI�u�W�F�N�g��Renderer�R���|�[�l���g
    private Renderer objRenderer;

    void Start()
    {
        // Renderer�R���|�[�l���g���擾
        objRenderer = GetComponent<Renderer>();

        // Renderer�R���|�[�l���g�����݂��邩�m�F
        if (objRenderer != null)
        {
            // �}�e���A���̐F�������_���ɕύX
            SetRandomColor();
        }
        else
        {
            Debug.LogError("Renderer�R���|�[�l���g��������܂���I");
        }
    }

    void SetRandomColor()
    {
        // �����_���ȐF�𐶐�
        Color randomColor = new Color(Random.value, Random.value, Random.value);

        // �}�e���A���̐F��ύX
        objRenderer.material.color = randomColor;

        // ���O�Ŋm�F
        Debug.Log("�V�����F: " + randomColor);
    }
}
