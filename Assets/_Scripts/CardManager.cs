using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.cyborgAssets.inspectorButtonPro;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public GameObject cardUIPrefab;
    public List<Card> deck;
    public RectTransform cardArea;
    public Button drawCardButton;
    public float positionDuration = 0.5f; // Dauer der Positionsänderung

    private List<GameObject> hand = new List<GameObject>();
    private Coroutine positionCoroutine;

    void Start()
    {
        drawCardButton.onClick.AddListener(DrawCard);
    }

    void DrawCard()
    {
        if (deck.Count > 0)
        {
            int randomIndex = Random.Range(0, deck.Count);
            Card drawnCard = deck[randomIndex];
            deck.RemoveAt(randomIndex);

            GameObject cardUIInstance = Instantiate(cardUIPrefab, cardArea);
            CardUI cardUI = cardUIInstance.GetComponent<CardUI>();
            cardUI.Initialize(drawnCard, this);
            hand.Add(cardUIInstance);

            // Starte die Positionierung der Karten
            if (positionCoroutine != null)
            {
                StopCoroutine(positionCoroutine);
            }
            positionCoroutine = StartCoroutine(PositionCards());
        }
    }

    public void RemoveCardFromHand(GameObject card)
    {
        hand.Remove(card);
        if (positionCoroutine != null)
        {
            StopCoroutine(positionCoroutine);
        }
        positionCoroutine = StartCoroutine(PositionCards());
    }

    IEnumerator PositionCards()
    {
        float screenWidth = GridManager.WIDTH;
        float cardWidth = cardUIPrefab.GetComponent<RectTransform>().sizeDelta.x;
        float spacing = -25f; // Veränderbarer Spacing-Wert
        float totalWidth = (cardWidth + spacing) * hand.Count - spacing; // Total width considering spacing
        float startX = (screenWidth - totalWidth) / 2;

        float midX = screenWidth / 2f;

        List<Vector3> targetPositions = new List<Vector3>();
        List<Quaternion> targetRotations = new List<Quaternion>();

        for (int i = 0; i < hand.Count; i++)
        {
            RectTransform cardRectTransform = hand[i].GetComponent<RectTransform>();
            float newX = startX + (cardWidth + spacing) * i;
            Vector2 targetPosition = new Vector2(newX, -300); // Y position set to 0 for standard position

            // Berechne die Neigung basierend auf der Entfernung zur Mitte
            float distanceToMid = Mathf.Abs(midX - newX);
            float tiltAngle = distanceToMid * 0.05f; // Veränderbarer Winkelwert

            // Wende die Neigung an (positiv für rechte Seite, negativ für linke Seite)
            Quaternion targetRotation;
            if (newX > midX)
            {
                targetRotation = Quaternion.Euler(0f, 0f, -tiltAngle);
            }
            else if (newX < midX)
            {
                targetRotation = Quaternion.Euler(0f, 0f, tiltAngle);
            }
            else
            {
                targetRotation = Quaternion.identity; // Nullrotation für die Mitte
            }

            // Justiere die Y-Position basierend auf dem Rotationswinkel
            float verticalOffset = Mathf.Abs(tiltAngle) * 2.5f; // Skalierungsfaktor für die Vertikalverschiebung
            targetPosition.y -= verticalOffset;

            // Speichere die Zielpositionen und Rotationen
            targetPositions.Add(targetPosition);
            targetRotations.Add(targetRotation);

            // Speichere die originale Position und Rotation der Karte
            hand[i].GetComponent<CardUI>().SaveOriginalTransform(verticalOffset, i, targetPosition, targetRotation);
        }

        float time = 0f;
        while (time < positionDuration)
        {
            for (int i = 0; i < hand.Count; i++)
            {
                RectTransform cardRectTransform = hand[i].GetComponent<RectTransform>();
                cardRectTransform.anchoredPosition = Vector2.Lerp(cardRectTransform.anchoredPosition, targetPositions[i], time / positionDuration);
                cardRectTransform.localRotation = Quaternion.Lerp(cardRectTransform.localRotation, targetRotations[i], time / positionDuration);
            }
            time += Time.deltaTime;
            yield return null;
        }

        // Am Ende der Animation die Zielpositionen und Rotationen direkt setzen
        for (int i = 0; i < hand.Count; i++)
        {
            RectTransform cardRectTransform = hand[i].GetComponent<RectTransform>();
            cardRectTransform.anchoredPosition = targetPositions[i];
            cardRectTransform.localRotation = targetRotations[i];
        }
    }

    [ProButton]
    public void RePositionCards()
    {
        if (positionCoroutine != null)
        {
            StopCoroutine(positionCoroutine);
        }
        positionCoroutine = StartCoroutine(PositionCards());
    }
}