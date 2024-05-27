using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIChangeTest : MonoBehaviour
{

    public TMP_Text UIText;

    public RoundManager roundManager;

    public string textBeforeNumber;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        UIText.text = textBeforeNumber + " " + roundManager.round.ToString();
    }
}
