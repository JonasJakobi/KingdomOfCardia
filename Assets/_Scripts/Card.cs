using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
    Damage,
    Debuffs,
    Gold,
    Healing,
    Shields,
    Other
}

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject
{
    public CardType cardType;
    public string cardName;
    public string description;
    public Sprite artwork;
    public CardEffect effect;
    public int valueOfCard;
    public float duration;
}
