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
    public bool warningShowed = false;
    private int lastTutorialStep;

    // Start is called before the first frame update
    void Start()
    {
        tutorialStep = 0;
        SwitchTutotialDialogue();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void IncrementTutorialStep()
    {
        if (((tutorialSkipped == false) && (tutorialStep != 0)) || ((warningShowed == false) && (tutorialStep >= 20)))
        {
            tutorialStep++;
            SwitchTutotialDialogue();
            AudioSystem.Instance.PlayClickSound();
        }
    }

    public void StartTutorial()
    {
        tutorialSkipped = false;
        tutorialStep++;
        SwitchTutotialDialogue();
        AudioSystem.Instance.PlayBonkSound();
        tutorialButtons.SetActive(false);
    }

    public void SkipTutorial()
    {
        tutorialSkipped = true;
        warningShowed = true;
        AudioSystem.Instance.PlayBonkSound();
        tutorialParent.SetActive(false);
    }

    public void ShowWarning()
    {
        if (warningShowed == false)
        {
            lastTutorialStep = tutorialStep;
            tutorialStep = 20;
            SwitchTutotialDialogue();
        }
    }

    public void SwitchTutotialDialogue()
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
                tutorialSpeechText.text = "Du hast gerade eine <b>Karte</b> erhalten!";
                break;
            case 8:
                tutorialSpeechText.text = "Du kannst diese während des nächsten <i>Angriffes</i> verwenden, um uns bei der Verteidigung zu unterstützen.";
                break;
            case 9:
                tutorialSpeechText.text = "Allerdings kannst du während einer Runde nur bis zu <b>3 Karten</b> auf der Hand haben. Mit genügend <b>Gold</b> kannst du dieses Limit während eines <i>Angriffes</i> erhöhen.";
                break;
            case 10:
                tutorialSpeechText.text = "Es wurden mehr <i>Monster</i> gesichtet. <i><b>Doppelt</b></i> so viele wie beim letzten Angriff!";
                break;
            case 11:
                tutorialSpeechText.text = "Passe deine Verteidigung an, indem du neue Einheiten platzierst, Gebäude errichtest oder die bestehende Verteidigung <i>verbesserst</i>!";
                break;
            case 12:
                tutorialSpeechText.text = "Mehr kann ich dir nicht beibringen. Viel Erfolg...";
                break;
            case 13:
                tutorialSkipped = true;
                tutorialParent.SetActive(false);
                return;
            case 20:
                tutorialSpeechText.text = "Whoops, da habe ich wohl doch etwas vergessen zu erwähnen.";
                break;
            case 21:
                tutorialSpeechText.text = "Wenn die <i>Monster</i> keinen Weg zu dem <b>Schneckenturm</b> finden, drehen sie durch und greifen einfach andere Türme an!";
                break;
            case 22:
                tutorialSpeechText.text = "Sei also stets vorsichtig beim Planen der nächsten Verteidigung und <i>entferne</i> notfalls bestehende Einheiten wieder.";
                break;
            case 23:
                tutorialSpeechText.text = "Das müsste jetzt aber wirklich alles gewesen sein? Viel Erfolg!";
                break;
            case 24:
                warningShowed = true;
                tutorialStep = lastTutorialStep;
                SwitchTutotialDialogue();
                tutorialParent.SetActive(false);
                return;
        }
    }
}
