using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.cyborgAssets.inspectorButtonPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class CardManager : Singleton<CardManager>
{
    public int handSize = 3;
    public int cardsPlayed = 0;
    public int currentUpgradeCost = 500;
    public GameObject cardUIPrefab;
    public List<Card> allCards;
    public List<Card> deck;
    public List<Card> fullDeck;
    public RectTransform cardArea;
    public RectTransform cardSelectionArea;
    public GameObject skipCardButton;
    public float positionDuration = 0.5f; // Dauer der Positionsänderung
    public GameObject cardSpacer;
    public GameObject cardSelectionBackground;


    private void Start()
    {
        //update button foer upgrading
        UIChangeManager.Instance.IncreaseCardUpgradeCost("Kaufen (" + currentUpgradeCost + ")");
    }

    private List<GameObject> hand = new List<GameObject>();
    [SerializeField] private List<CardUI> displayedCards = new List<CardUI>();
    private Coroutine positionCoroutine;



    [ProButton]
    /// <summary>
    /// Draw random cards from allCards and display them in the center of the screen
    /// </summary>
    public void DrawRandomCards()
    {
        if (displayedCards.Count > 0) return; // Only one set of random card can be present at the same time

        cardSelectionBackground.SetActive(true);
        skipCardButton.SetActive(true);

        for (int i = 0; i < 3; i++)
        {
            Card randomCard = allCards[Random.Range(0, allCards.Count)];
            GameObject cardObject = Instantiate(cardUIPrefab, cardSelectionArea);
            CardUI cardUI = cardObject.GetComponent<CardUI>();
            cardUI.changeCardSelection(true);
            cardUI.Initialize(randomCard, this);

            // Position the cards next to eachother
            RectTransform rectTransform = cardObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(i * 150f - 150f, 0);

            displayedCards.Add(cardUI);
        }
    }


    /// <summary>
    /// Select one of the randomly drawn cards
    /// </summary>
    /// <param name="selectedCardUI">CardUI of the selected card</param>
    public void SelectCard(CardUI selectedCardUI)
    {
        // Karte zum Deck hinzufügen
        AddCardToDeck(selectedCardUI.cardData);

        UIChangeManager.Instance.TutorialCheck();
        UIChangeManager.Instance.WarningCheck();

        // Entfernen der angezeigten Karten
        DiscardSelection();
    }

    /// <summary>
    /// Add a card to the players deck
    /// </summary>
    /// <param name="card">The card to add to the players deck</param>
    public void AddCardToDeck(Card card)
    {
        fullDeck.Add(card);
        Debug.Log("Karte hinzugefügt: " + card.cardName);
        deck.Add(card);

        //Add the following line to this function in order to remove the deck component.
        //DrawCard();
    }

    [ProButton]
    /// <summary>
    /// Draw a card from the deck into the hand
    /// </summary>
    public void DrawCard()
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

    /// <summary>
    /// Remove a card from the hand
    /// </summary>
    /// <param name="card">The card to be removed from the hand</param>
    public void RemoveCardFromHand(GameObject card)
    {
        hand.Remove(card);
        if (positionCoroutine != null)
        {
            StopCoroutine(positionCoroutine);
        }
        positionCoroutine = StartCoroutine(PositionCards());
    }

    /// <summary>
    /// Update the position of all cards
    /// </summary>
    /// <returns></returns>
    IEnumerator PositionCards()
    {
        float screenWidth = GridManager.WIDTH;
        float cardWidth = cardUIPrefab.GetComponent<RectTransform>().sizeDelta.x;
        float spacing = -25f; // Veränderbarer Spacing-Wert
        float totalWidth = (cardWidth + spacing) * hand.Count - spacing; // Total width considering spacing
        float startX = (screenWidth - totalWidth) / 2;

        float midX = screenWidth - cardWidth / 2f;

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

    /// <summary>
    /// Manually update the position of all cards (For debug purposes)
    /// </summary>
    [ProButton]
    public void RePositionCards()
    {
        if (positionCoroutine != null)
        {
            StopCoroutine(positionCoroutine);
        }
        positionCoroutine = StartCoroutine(PositionCards());
    }

    public void ClearHand()
    {
        foreach (var card in hand)
        {
            Destroy(card.gameObject);
        }
        hand.Clear();
    }

    public void DiscardSelection()
    {
        // Entfernen der angezeigten Karten
        foreach (var card in displayedCards)
        {
            Destroy(card.gameObject);
        }
        displayedCards.Clear();
        cardSelectionBackground.SetActive(false);
        skipCardButton.SetActive(false);
    }

    public void DrawNewCards()
    {
        deck.Clear();
        deck.AddRange(fullDeck);
        for (float i = 0f; i < (float)handSize; i++)
        {
            StartCoroutine(CardDrawDelay(i * 0.2f));

        }
    }

    [ProButton]
    public void IncreaseHandSize()
    {
        if (handSize < 6)
        {
            if (MoneyManager.Instance.CanAfford(currentUpgradeCost))
            {
                MoneyManager.Instance.RemoveMoney(currentUpgradeCost);
                handSize++;
                DrawCard();
                currentUpgradeCost *= 25;
                if (currentUpgradeCost <= 312500)
                {
                    UIChangeManager.Instance.IncreaseCardUpgradeCost("Kaufen (" + currentUpgradeCost + ")");
                }
                else
                {
                    UIChangeManager.Instance.IncreaseCardUpgradeCost("Maximal");
                }
            }
            else Debug.Log("Can't afford upgrade.");
        }
        else Debug.Log("Already at max deck size");
    }

    private IEnumerator CardDrawDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        DrawCard();
    }
}