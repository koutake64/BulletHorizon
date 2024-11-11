using UnityEngine;
using DG.Tweening;

public class TitleUI : MonoBehaviour
{
	[SerializeField] float DurationSeconds;
	[SerializeField] Ease EaseType;

//===== Hide --------------------------------------------------------------------------------------------------------------------
	private CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.DOFade(0.0f, DurationSeconds).SetEase(EaseType).SetLoops(-1, LoopType.Yoyo);
    }
}
