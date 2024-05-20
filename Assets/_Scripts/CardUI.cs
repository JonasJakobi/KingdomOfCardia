using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

public class CardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Card cardData;
    public TMP_Text cardNameText;
    public TMP_Text descriptionText;
    public Image artworkImage;
    public CardManager cardManager;


    private Vector3 originalScale;
    private Vector3 originalPosition;
    private float originalVerticalOffset;
    private Quaternion originalRotation;
    private int originalSiblingIndex;

    private Coroutine scaleCoroutine;
    private Coroutine moveCoroutine;
    private bool isHovered = false;
    private bool isPlayed = false;

    public void Initialize(Card card, CardManager manager)
    {
        cardData = card;
        cardManager = manager;
        cardNameText.text = card.cardName;
        descriptionText.text = card.description;
        artworkImage.sprite = card.artwork;
        originalScale = transform.localScale;
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
        originalSiblingIndex = transform.GetSiblingIndex();
    }

    public void SaveOriginalTransform(float verticalOffset, int siblingIndex, Vector3 targetPosition, Quaternion targetRotation)
    {
        originalPosition = targetPosition;
        originalRotation = targetRotation;
        originalSiblingIndex = siblingIndex;
        originalVerticalOffset = verticalOffset;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isPlayed == false)
        {
            isHovered = true;
            transform.SetAsLastSibling(); // Bringt die Karte in den Vordergrund

            if (scaleCoroutine != null)
            {
                StopCoroutine(scaleCoroutine);
            }
            scaleCoroutine = StartCoroutine(ScaleCard(originalScale * 1.2f, 0.2f)); // Vergrößert die Karte

            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }

            float additionalYOffset = (float)GridManager.HEIGHT * 5.0f + originalVerticalOffset;
            moveCoroutine = StartCoroutine(MoveCard(new Vector3(originalPosition.x, originalPosition.y + additionalYOffset, originalPosition.z), Quaternion.identity, 0.2f));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isPlayed == false)
        {
            isHovered = false;

            if (scaleCoroutine != null)
            {
                StopCoroutine(scaleCoroutine);
            }
            scaleCoroutine = StartCoroutine(ScaleCard(originalScale, 0.2f)); // Setzt die Größe der Karte zurück

            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }
            moveCoroutine = StartCoroutine(MoveCard(originalPosition, originalRotation, 0.2f));

            // Verzögert das Zurücksetzen des Sibling Indexes, damit die Animation zuerst abgeschlossen wird
            StartCoroutine(ResetSiblingIndexAfterDelay(0.2f));
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        StartCoroutine(PlayCard());
    }

    private IEnumerator PlayCard()
    {
        isPlayed = true;
        Vector3 screenCenter = new Vector3(GridManager.WIDTH / 2f, GridManager.HEIGHT / 2f, 0);
        RectTransform rectTransform = GetComponent<RectTransform>();

        Vector3 targetPosition = screenCenter;

        float duration = 1.1f; // Dauer der Animation
        float time = 0f;

        while (time < duration)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPosition, time / duration);
            rectTransform.localScale = Vector3.Lerp(originalScale, originalScale * 1.1f, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = targetPosition;
        rectTransform.localScale = originalScale * 1.1f;

        // Effekt der Karte ausführen
        TriggerCardEffect();

        // Entferne die Karte aus der Hand im CardManager
        cardManager.RemoveCardFromHand(gameObject);

        // Warte eine kurze Zeit und zerstöre dann die Karte
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }

    //Beispielhafter Effekt einer Karte
    private void TriggerCardEffect()
    {
        // Hier wird der Effekt der Karte ausgelöst
        Debug.Log("Karte gespielt: " + cardData.cardName);
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            enemy.TakeDamage(cardData.damageAmount); // cardData.damageAmount sollte den Schaden der Karte enthalten
        }
    }

    private IEnumerator ScaleCard(Vector3 targetScale, float duration)
    {
        Vector3 startScale = transform.localScale;
        float time = 0f;

        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.localScale = targetScale;
    }

    private IEnumerator MoveCard(Vector3 targetPosition, Quaternion targetRotation, float duration)
    {
        Vector3 startPosition = transform.localPosition;
        Quaternion startRotation = transform.localRotation;
        float time = 0f;

        while (time < duration)
        {
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, time / duration);
            transform.localRotation = Quaternion.Lerp(startRotation, targetRotation, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = targetPosition;
        transform.localRotation = targetRotation;
    }

    private IEnumerator ResetSiblingIndexAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!isHovered) // Verhindert das Zurücksetzen, falls die Karte erneut gehovered wird
        {
            transform.SetSiblingIndex(originalSiblingIndex);
        }
    }
}