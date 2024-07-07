using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

public class Tutorial : MonoBehaviour
{
    public GameObject tutorialParent;
    public GameObject tutorialButtons;
    public TMP_Text tutorialSpeechText;
    public int tutorialStep;
    public bool tutorialSkipped = true;

    // Start is called before the first frame update
    void Start()
    {
        tutorialStep = 0;
        switchTutotialDialogue();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void IncrementTutorialStep()
    {
        if ((tutorialSkipped != true) && (tutorialStep != 0))
        {
            tutorialStep++;
            switchTutotialDialogue();
            AudioSystem.Instance.PlayClickSound();
        }
    }

    public void StartTutorial()
    {
        tutorialSkipped = false;
        tutorialStep++;
        switchTutotialDialogue();
        AudioSystem.Instance.PlayBonkSound();
        tutorialButtons.SetActive(false);
    }

    public void SkipTutorial()
    {
        tutorialSkipped = true;
        AudioSystem.Instance.PlayBonkSound();
        tutorialParent.SetActive(false);
    }

    public void switchTutotialDialogue()
    {
        switch (tutorialStep)
        {
            case 0:
                tutorialSpeechText.text = "Bist Du bereits in die <b>Grundlagen</b> eingeweiht?";
                break;

            case 1:
                tutorialSpeechText.text = "Wir müssen um jeden Preis den <b>Schneckenturm</b> verteidigen!";
                break;

            case 2:
                tutorialSpeechText.text = "Dafür benötigen wir allerdings ordentlich Hilfe und die Schatzkammer ist fast leer.";
                break;

            case 3:
                tutorialSpeechText.text = "Unsere Späher konnten ein <i>Monster</i> ausfindig machen! Es wurde auf deiner Karte markiert.";
                break;

            case 4:
                tutorialSpeechText.text = "Wir müssen nun mit unserem verbliebenem <b>Gold</b> einen <b>Zauberer</b> rekrutieren, sodass wir diese <i>erste Bedrohung</i> erfolgreich abwehren können.";
                break;

            case 5:
                tutorialSpeechText.text = "<b>Achtung:</b> Während eines aktiven <i>Angriffes</i> können wir weder Einheiten rekrutieren noch Gebäude errichten.";
                break;
            case 6:
                IncrementTutorialStep();
                tutorialParent.SetActive(false);
                break;
            case 7:
                tutorialSpeechText.text = "Du hast gerade deine erste <b>Karte</b> erhalten!";
                break;
            case 8:
                tutorialSpeechText.text = "Du kannst diese während des nächsten <i>Angriffes</i> verwenden, um uns bei der Verteidigung zu unterstützen.";
                break;
            case 9:
                tutorialSpeechText.text = "Es wurden mehr <i>Monster</i> gesichtet. <i><b>Doppelt</b></i> so viele wie beim letzten Angriff!";
                break;
            case 10:
                tutorialSpeechText.text = "Mehr kann ich dir nicht beibringen. Viel Erfolg...";
                break;
            case 11:
                tutorialSkipped = true;
                tutorialParent.SetActive(false);
                return;
        }
    }
}
