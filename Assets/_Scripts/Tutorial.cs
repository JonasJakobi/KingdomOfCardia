using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

public class Tutorial : MonoBehaviour
{
    public TMP_Text tutorialSpeechText;
    public int tutorialStep = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        tutorialStep++;

    }

    public void switchTutotialDialogue()
    {
        switch (tutorialStep)
        {
            case 0:
                tutorialSpeechText.text = "Bist Du bereits in die Grundlagen eingeweiht?";
                break;

            case 1:
                tutorialSpeechText.text = "Wir müssen um jeden Preis den <b>Schneckenturm</b> verteidigen!";
                break;

            case 2:
                tutorialSpeechText.text = "Dafür benötigen wir allerdings ordentlich hilfe und die Schatzkammer ist fast leer.";
                break;

            case 3:
                tutorialSpeechText.text = "Unsere Späher konnten ein <i>Monster</i> ausfindig machen! Es wurde auf deiner Karte markiert.";
                break;

            case 4:
                tutorialSpeechText.text = "Wir müssen nun mit unserem verbliebenem Gold einen Zauberer rekrutieren, sodass wir diese erste Bedrohung erfolgreich abwehren können.";
                break;

            case 5:
                tutorialSpeechText.text = "Achtung: Während eines aktiven Angriffes können wir weder Einheiten rekrutieren noch Gebäude errichten.";
                break;
        }
    }
}
