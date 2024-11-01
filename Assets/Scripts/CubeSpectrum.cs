using UnityEngine;

public class CubeSpectrum : MonoBehaviour
{
    [SerializeField] AudioSpectrum spectrum;

    [SerializeField,Header("�I�u�W�F�N�g�z��")]        Transform[] cubes;
    [SerializeField, Header("�X�y�N�g�����̍����{��")] float scale;

    private void Update()
    {
        int i = 0;

        foreach (var cube in cubes)
        {
            //�I�u�W�F�N�g�̃X�P�[�����擾
            var localScale = cube.localScale;
            //�X�y�N�g�����̃��x�����X�P�[����Y�X�P�[���ɒu��������
            localScale.y = spectrum.Levels[i] * scale;
            cube.localScale = localScale;
            i++;
        }
    }
}
