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

    //Sprite Changes
    [Header("Character Portrait")]
    public Image characterImage; // Drag your UI Image here
    public Image expressionImage; 
    public Image layingDownImage;
    private Sprite[] currentClientExpressions;

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

    public void StartDialogue(ClientData clientData)
    {
        story = new Story(clientData.inkFile.text);
        currentClientExpressions = clientData.expressionSprites; 

        // Initialize base face
        if (characterImage != null && clientData.portrait != null)
            characterImage.sprite = clientData.portrait;

        // Expression overlay
        if (expressionImage != null && currentClientExpressions.Length > 0)
            expressionImage.sprite = currentClientExpressions[0];

        layingDownImage.sprite = clientData.clientSpriteLayingDown;

        RefreshUI();
    }

    void RefreshUI()
    {
        foreach (Transform child in choicesPanel.transform)
            Destroy(child.gameObject);

        string text = "";

        // Handle any tags from the current line
        HandleTags(story.currentTags);

        while (story.canContinue)
        {
            text += story.Continue();
            if (story.currentChoices.Count > 0)
                break;
        }

        // If no sprite tag found on this line, show neutral by default
        // if (!story.currentTags.Exists(t => t.StartsWith("sprite:")))
        // {
        //     ChangeSprite("neutral");
        // }

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
                Button button = Instantiate(choiceButtonPrefab, choicesPanel.transform, false);
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
    //For the different sprite expressions
    void HandleTags(System.Collections.Generic.List<string> tags)
    {
        foreach (string tag in tags)
        {
            Debug.Log("Raw Tag: " + tag);

            if (tag.StartsWith("sprite:"))
            {
                string[] splitTag = tag.Split(' ');
                string spriteTagPart = splitTag[0];  // only keep first token

                string spriteName = spriteTagPart.Split(':')[1].Trim().ToLower();
                ChangeSprite(spriteName);
            }
        }
    }

    void ChangeSprite(string spriteName)
    {
        if (currentClientExpressions == null || currentClientExpressions.Length == 0)
            return;

        Sprite newExpression = null;

        switch (spriteName)
        {
            case "neutral":
                newExpression = currentClientExpressions[0];
                break;
            case "happy":
                newExpression = currentClientExpressions.Length > 1 ? currentClientExpressions[1] : currentClientExpressions[0];
                break;
            case "angry":
                newExpression = currentClientExpressions.Length > 2 ? currentClientExpressions[2] : currentClientExpressions[0];
                break;
            case "surprised":
                newExpression = currentClientExpressions.Length > 3 ? currentClientExpressions[3] : currentClientExpressions[0];
                break;
            default:
                Debug.LogWarning("Unknown sprite tag: " + spriteName);
                break;
        }

        // Apply only to the overlay
        expressionImage.sprite = newExpression;
        expressionImage.preserveAspect = true; 
    }


}
