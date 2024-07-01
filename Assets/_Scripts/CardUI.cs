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
    private bool inSelection = false;

    /// <summary>
    /// Set the initial states of the card
    /// </summary>
    /// <param name="card">The card</param>
    /// <param name="manager">The card manager</param>
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

    void Update()
    {
        if (isHovered)
        {
            transform.SetAsLastSibling();
            cardManager.cardSpacer.transform.SetSiblingIndex(originalSiblingIndex);
        }
    }

    /// <summary>
    /// Save the orginal state of the card
    /// </summary>
    /// <param name="verticalOffset">Vertical offset of the card depending on its distance to the center of the screen</param>
    /// <param name="siblingIndex">Sibling index of the card</param>
    /// <param name="targetPosition">Position of the card</param>
    /// <param name="targetRotation">Rotation of the card</param>
    public void SaveOriginalTransform(float verticalOffset, int siblingIndex, Vector3 targetPosition, Quaternion targetRotation)
    {
        originalPosition = targetPosition;
        originalRotation = targetRotation;
        originalSiblingIndex = siblingIndex;
        originalVerticalOffset = verticalOffset;
    }

    /// <summary>
    /// Move and or scale card and set its rotation to 0
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isPlayed == false && inSelection == false)
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

            float additionalYOffset = (float)Screen.height / 10.0f + originalVerticalOffset;
            moveCoroutine = StartCoroutine(MoveCard(new Vector3(originalPosition.x, originalPosition.y + additionalYOffset, originalPosition.z), Quaternion.identity, 0.2f));
        }

        else if (isPlayed == false && inSelection == true)
        {
            isHovered = true;

            if (scaleCoroutine != null)
            {
                StopCoroutine(scaleCoroutine);
            }
            scaleCoroutine = StartCoroutine(ScaleCard(originalScale * 1.2f, 0.2f)); // Vergrößert die Karte
        }
    }


    /// <summary>
    /// Reset scale and position of this card
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (isPlayed == false && inSelection == false)
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

        else if (isPlayed == false && inSelection == true)
        {
            isHovered = false;

            if (scaleCoroutine != null)
            {
                StopCoroutine(scaleCoroutine);
            }
            scaleCoroutine = StartCoroutine(ScaleCard(originalScale, 0.2f)); // Setzt die Größe der Karte zurück
        }
    }

    /// <summary>
    /// Select this card or play this card, depending on the cards inSelection state
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (inSelection == true)
        {
            changeCardSelection(false);
            cardManager.SelectCard(this);
        }

        else
        {
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }
            StartCoroutine(PlayCard());
        }


    }

    /// <summary>
    /// Changes whether the card is currently part of the selection process or the players hand
    /// </summary>
    /// <param name="state">Current state of this card</param>
    public void changeCardSelection(bool state)
    {
        inSelection = state;
    }

    /// <summary>
    /// Move this card to the middle of the screen, trigger its effect and remove the card from the hand
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayCard()
    {
        isPlayed = true;
        Vector3 screenCenter = new Vector3(GridManager.WIDTH / 2f, GridManager.HEIGHT / 2f, 0);
        RectTransform rectTransform = GetComponent<RectTransform>();

        Vector3 targetPosition = screenCenter;

        float duration = 1.1f; // Duration of the animation
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

        // Trigger the effect of the card
        TriggerCardEffect();

        // Remove the card from the players hand
        cardManager.RemoveCardFromHand(gameObject);

        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }

    /// <summary>
    /// Trigger the effect of a card depending on its card type
    /// </summary>
    private void TriggerCardEffect()
    {
        if (cardData.cardType == CardType.Damage)
        {
            cardData.effect.DealDamage(cardData.valueOfCard);
        }

        else if (cardData.cardType == CardType.Healing)
        {
            cardData.effect.HealBaseTowers(cardData.valueOfCard);
        }

        else if (cardData.cardType == CardType.Shields)
        {
            cardData.effect.ShieldBaseTowers(cardData.valueOfCard, cardData.duration);
        }

        else if (cardData.cardType == CardType.Debuffs && cardData.effect.damageType == DamageType.Ice)
        {
            cardData.effect.SlowAllEnemies(cardData.valueOfCard, cardData.duration);
        }

        else if (cardData.cardType == CardType.Gold)
        {
            cardData.effect.MultiplyGold(cardData.valueOfCard);
        }

        else if (cardData.cardType == CardType.DamageOverTime && cardData.effect.damageType == DamageType.Fire)
        {
            cardData.effect.StartFireDamage(cardData.duration, cardData.valueOfCard);
        }

        else if (cardData.cardType == CardType.DamageOverTime && cardData.effect.damageType == DamageType.Necrotic)
        {
            cardData.effect.StartNecroticDamage(cardData.duration, cardData.valueOfCard);
        }

        else if (cardData.cardType == CardType.DamageOverTime && cardData.effect.damageType == DamageType.Electricity)
        {
            cardData.effect.StartElectricDamage(cardData.duration, cardData.valueOfCard);
        }

        else if (cardData.cardType == CardType.Debuffs && cardData.effect.damageType == DamageType.Piercing)
        {
            cardData.effect.ReduceDamage(cardData.valueOfCard, cardData.duration);
        }

        else if (cardData.cardType == CardType.Debuffs && cardData.effect.damageType == DamageType.Air)
        {
            cardData.effect.OverallSlower(cardData.valueOfCard, cardData.duration);
        }

        else
        {
            Debug.Log("Anderer Typ!");
        }
    }

    /// <summary>
    /// Coroutine to scale the card
    /// </summary>
    /// <param name="targetScale">Target scale of the card</param>
    /// <param name="duration">Duration of the animation</param>
    /// <returns></returns>
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

    /// <summary>
    /// Coroutine to move the card to a target position within a set amount of time
    /// </summary>
    /// <param name="targetPosition">Target position of the card</param>
    /// <param name="targetRotation">Target rotation of the card</param>
    /// <param name="duration">Duration of the animation</param>
    /// <returns></returns>
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

    /// <summary>
    /// Reset the sibling index of this card to its original state
    /// </summary>
    /// <param name="delay">Delay before resetting the sibling index</param>
    /// <returns></returns>
    private IEnumerator ResetSiblingIndexAfterDelay(float delay)
    {
        cardManager.cardSpacer.transform.SetAsLastSibling();
        yield return new WaitForSeconds(delay);
        if (!isHovered) // Verhindert das Zurücksetzen, falls die Karte erneut gehovered wird
        {
            transform.SetSiblingIndex(originalSiblingIndex);
        }
    }
}