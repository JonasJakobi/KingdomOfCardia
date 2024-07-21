using UnityEngine;

public class TowerStars : MonoBehaviour
{
    [SerializeField]
    private Sprite[] starSprites;

    public Transform starSpawnPoint;

    private GameObject starInstance;

    private void Start()
    {
        starSpawnPoint = transform;
    }

    public void SetStars(int amount, StarColor color)
    {
        int finalAmount = amount;
        if (color.Equals(StarColor.Silver))
        {
            finalAmount += 3;
        }
        else if (color.Equals(StarColor.Gold))
        {
            finalAmount += 6;
        }
        else if (color.Equals(StarColor.Black))
        {
            finalAmount += 9;
        }
        if (finalAmount <= 0 || finalAmount > starSprites.Length)
        {
            GetComponent<SpriteRenderer>().sprite = null;
            return;
        }

        GetComponent<SpriteRenderer>().sprite = starSprites[finalAmount - 1];
    }
}
