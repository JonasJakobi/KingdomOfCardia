using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyDisplay : MonoBehaviour
{
    public float changePerRound;

    public void Changes()
    {
        GetComponent<RectTransform>().position = new Vector3(GetComponent<RectTransform>().position.x + changePerRound, GetComponent<RectTransform>().position.y, GetComponent<RectTransform>().position.z);
    }
}
