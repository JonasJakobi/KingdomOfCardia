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
    private Coroutine typeSentence;
    private string stringSaver;
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
            if (typeSentence != null)
            {
                StopCoroutine(typeSentence);
                typeSentence = null;
                tutorialSpeechText.text = stringSaver;
            }
            else
            {
                tutorialStep++;
                SwitchTutotialDialogue();
            }

            AudioSystem.Instance.PlayClickSound();
        }
    }

    public void StartTutorial()
    {
        if (typeSentence != null)
        {
            StopCoroutine(typeSentence);
            Debug.Log("Coroutine stopped");
            typeSentence = null;
        }
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

    public void ReactivateTutorial()
    {
        SwitchTutotialDialogue();
    }

    IEnumerator TypeSentence(string sentence)
    {
        stringSaver = sentence;
        tutorialSpeechText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            AudioSystem.Instance.PlayDialogueSound();
            tutorialSpeechText.text += letter;
            yield return new WaitForSeconds(0.02f);
        }
        typeSentence = null;
    }

    public void SwitchTutotialDialogue()
    {
        switch (tutorialStep)
        {
            case 0:
                typeSentence = StartCoroutine(TypeSentence("Bist Du bereits in die <b>Grundlagen</b> eingeweiht?"));
                break;

            case 1:
                typeSentence = StartCoroutine(TypeSentence("Wir müssen um jeden Preis den <b>Schneckenturm</b> verteidigen!"));
                break;

            case 2:
                typeSentence = StartCoroutine(TypeSentence("Dafür benötigen wir allerdings ordentlich Hilfe und die Schatzkammer ist fast leer."));
                break;

            case 3:
                typeSentence = StartCoroutine(TypeSentence("Unsere Späher konnten ein <i>Monster</i> ausfindig machen! Es wurde auf deiner Karte markiert."));
                break;

            case 4:
                typeSentence = StartCoroutine(TypeSentence("Wir müssen nun mit unserem verbliebenem <b>Gold</b> einen <b>Zauberer</b> rekrutieren, sodass wir diese <i>erste Bedrohung</i> erfolgreich abwehren können."));
                break;

            case 5:
                typeSentence = StartCoroutine(TypeSentence("<b>Achtung:</b> Während eines aktiven <i>Angriffes</i> können wir weder Einheiten rekrutieren noch Gebäude errichten."));
                break;
            case 6:
                IncrementTutorialStep();
                tutorialParent.SetActive(false);
                break;
            case 7:
                typeSentence = StartCoroutine(TypeSentence("Du hast gerade eine <b>Karte</b> erhalten!"));
                break;
            case 8:
                typeSentence = StartCoroutine(TypeSentence("Du kannst diese während des nächsten <i>Angriffes</i> verwenden, um uns bei der Verteidigung zu unterstützen."));
                break;
            case 9:
                typeSentence = StartCoroutine(TypeSentence("Allerdings kannst du während einer Runde nur bis zu <b>3 Karten</b> auf der Hand haben. Mit genügend <b>Gold</b> kannst du dieses Limit während eines <i>Angriffes</i> erhöhen."));
                break;
            case 10:
                typeSentence = StartCoroutine(TypeSentence("Es wurden mehr <i>Monster</i> gesichtet. <i><b>Doppelt</b></i> so viele wie beim letzten Angriff!"));
                break;
            case 11:
                typeSentence = StartCoroutine(TypeSentence("Passe deine Verteidigung an, indem du neue Einheiten platzierst, Gebäude errichtest oder die bestehende Verteidigung <i>verbesserst</i>!"));
                break;
            case 12:
                typeSentence = StartCoroutine(TypeSentence("Mehr kann ich dir nicht beibringen. Viel Erfolg..."));
                break;
            case 13:
                tutorialSkipped = true;
                tutorialParent.SetActive(false);
                return;
            case 20:
                typeSentence = StartCoroutine(TypeSentence("Whoops, da habe ich wohl doch etwas vergessen zu erwähnen."));
                break;
            case 21:
                typeSentence = StartCoroutine(TypeSentence("Wenn die <i>Monster</i> keinen Weg zu dem <b>Schneckenturm</b> finden, drehen sie durch und greifen einfach andere Türme an!"));
                break;
            case 22:
                typeSentence = StartCoroutine(TypeSentence("Sei also stets vorsichtig beim Planen der nächsten Verteidigung und <i>entferne</i> notfalls bestehende Einheiten wieder."));
                break;
            case 23:
                typeSentence = StartCoroutine(TypeSentence("Das müsste jetzt aber wirklich alles gewesen sein? Viel Erfolg!"));
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
