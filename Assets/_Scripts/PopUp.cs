using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
public class PopUp : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image popup;
    public float fadeDuration = 0.5f;
    public float delay = 1.5f;


    public void OnPointerEnter(PointerEventData eventData)
    {
        popup.gameObject.SetActive(true);
        var textMesh = popup.GetComponentInChildren<TextMeshProUGUI>();
        DOTween.Kill(popup);
        DOTween.Kill(textMesh);
        popup.DOFade(0, 0);
        textMesh.DOFade(0, 0);
        // Popup einblenden
        popup.DOFade(1, fadeDuration).SetDelay(delay);

        textMesh.DOFade(1, fadeDuration).SetDelay(delay);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Stop all current tweens
        DOTween.Kill(popup);
        var textMesh = popup.GetComponentInChildren<TextMeshProUGUI>();
        DOTween.Kill(textMesh);
        popup.DOFade(0, fadeDuration * 0.6f);
        textMesh.DOFade(0, fadeDuration * 0.6f).OnComplete(() => popup.gameObject.SetActive(false));


    }
}
