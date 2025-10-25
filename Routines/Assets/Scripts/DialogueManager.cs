using UnityEngine;
using Ink.Runtime;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public GameObject choicesPanel;
    public Button choiceButtonPrefab;

    private Story story;

    public System.Action onDialogueEnd;

    private bool waitingForContinue = false;

    //Styling
    public float typingSpeed = 0.03f; // Medium speed
    public AudioSource audioSource;  // Blip sound source
    public GameObject continueIcon;  // (Press Space) blinking visual
    private Coroutine typingRoutine;
    private bool isTyping = false;
    private string currentLine = "";

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (isTyping)
            {
                StopCoroutine(typingRoutine);
                dialogueText.text = currentLine;
                isTyping = false;
                UpdateContinueIcon();
            }
            else if (waitingForContinue)
            {
                continueIcon.SetActive(false);
                waitingForContinue = false;
                ContinueStory();
            }
        }
    }

    public void StartDialogue(TextAsset inkJSON)
    {
        story = new Story(inkJSON.text);
        RefreshUI();
    }

    void RefreshUI()
    {
        foreach (Transform child in choicesPanel.transform)
            Destroy(child.gameObject);

        string text = "";

        while (story.canContinue)
        {
            text += story.Continue();
            if (story.currentChoices.Count > 0)
                break;
        }

        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        typingRoutine = StartCoroutine(TypeText(text.Trim()));

        if (story.currentChoices.Count > 0)
        {
            waitingForContinue = false;
            for (int i = 0; i < story.currentChoices.Count; i++)
            {
                int choiceIndex = i;
                var choice = story.currentChoices[i];
                Button button = Instantiate(choiceButtonPrefab, choicesPanel.transform);
                button.GetComponentInChildren<TMPro.TMP_Text>().text = choice.text;
                button.onClick.AddListener(() => MakeChoice(choiceIndex));
                button.gameObject.SetActive(true);
            }
        }
        else if (text.Length > 0)
        {
            waitingForContinue = true;
        }
        else
        {
            onDialogueEnd?.Invoke();
        }

        UpdateContinueIcon();
    }

    void MakeChoice(int choiceIndex)
    {
        story.ChooseChoiceIndex(choiceIndex);
        RefreshUI();
    }

    void ContinueStory()
    {
        RefreshUI();
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";
        currentLine = text;

        foreach (char c in text)
        {
            dialogueText.text += c;

            if (audioSource && !char.IsWhiteSpace(c))
                audioSource.Play();

            if (Keyboard.current.spaceKey.isPressed)
            {
                dialogueText.text = text;
                break;
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        UpdateContinueIcon();
    }

    void UpdateContinueIcon()
    {
        bool hasChoices = story.currentChoices.Count > 0;
        bool textDone = !isTyping;
        bool canContinue = story.canContinue || waitingForContinue;

        // Show icon ONLY when text is done AND no choices visible
        continueIcon.SetActive(textDone && !hasChoices && canContinue);
    }
}
