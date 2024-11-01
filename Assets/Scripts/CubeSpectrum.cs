using UnityEngine;

public class CubeSpectrum : MonoBehaviour
{
    [SerializeField] AudioSpectrum spectrum;

    [SerializeField,Header("オブジェクト配列")]        Transform[] cubes;
    [SerializeField, Header("スペクトラムの高さ倍率")] float scale;

    private void Update()
    {
        int i = 0;

        foreach (var cube in cubes)
        {
            //オブジェクトのスケールを取得
            var localScale = cube.localScale;
            //スペクトラムのレベル＊スケールをYスケールに置き換える
            localScale.y = spectrum.Levels[i] * scale;
            cube.localScale = localScale;
            i++;
        }
    }
}
