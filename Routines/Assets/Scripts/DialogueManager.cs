using UnityEngine;
using Ink.Runtime;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.TextCore.Text;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public GameObject choicesPanel;
    public Button choiceButtonPrefab;

    private Story story;

    public System.Action onDialogueEnd;

    private bool waitingForContinue = false;

    //Styling
    public float typingSpeed; // Medium speed
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
    public Dictionary<string, Sprite> currentClientExpressions;

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

    public void StartDialogue(ClientData clientData, CharacterData character)
    {
        //Debug.Log(clientData);
        //Debug.Log(character);
        //Debug.Log(clientData.inkFile);
        string inkJson = clientData.inkFile.text;
        story = new Story(inkJson);

        var name = clientData.name;

        GlobalInkVarStore.Instance.PushToStory(story);

        object value;
        if (GlobalInkVarStore.Instance.TryGet("break_up", out value))
        {
            int break_up_value = (int)value;
            Debug.Log("break_up:" + break_up_value);
        }
        else
        {
            Debug.Log("No value for 'break_up' yet");
        }

        if (GlobalInkVarStore.Instance.TryGet("ending", out value))
        {
            string ending_value = (string)value;
            Debug.Log("ending:" + ending_value);
        }
        else
        {
            Debug.Log("No value for 'ending' yet");
        }

        if (GlobalInkVarStore.Instance.TryGet("mask_bernadette", out value))
        {
            bool mask_bernadette_value = (bool)value;
            Debug.Log("mask_bernadette:" + mask_bernadette_value);
        }
        else
        {
            Debug.Log("No value for 'mask_bernadette' yet");
        }

        if (GlobalInkVarStore.Instance.TryGet("mask_mateo", out value))
        {
            bool mask_mateo_value = (bool)value;
            Debug.Log("mask_mateo:" + mask_mateo_value);
        }
        else
        {
            Debug.Log("No value for 'mask_mateo' yet");
        }

        if (GlobalInkVarStore.Instance.TryGet("inspired", out value))
        {
            bool inspired_value = (bool)value;
            Debug.Log("inspired:" + inspired_value);
        }
        else
        {
            Debug.Log("No value for 'inspired' yet");
        }

        currentClientExpressions = character.expressionSprites;

        // Initialize base face
        if (characterImage != null && character.portrait != null)
            characterImage.sprite = character.portrait;

        // Expression overlay
        if (expressionImage != null && currentClientExpressions.Count > 0)
            expressionImage.sprite = currentClientExpressions["neutral"];

        RefreshUI();
    }
    
    public void ChangeClient(CharacterData character)
    {
        layingDownImage.sprite = character.clientSpriteLayingDown;
        //RefreshUI();
    }

    void RefreshUI()
    {
        foreach (Transform child in choicesPanel.transform)
            Destroy(child.gameObject);

        string text = "";

        // Handle any tags from the current line
        //Debug.Log("Current Tags: " + string.Join(", ", story.currentTags));

        while (story.canContinue)
        {
            text += story.Continue();
            HandleTags(story.currentTags);
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
            GlobalInkVarStore.Instance.PullFromStory(story);
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
        dialogueText.enableAutoSizing = false;
        dialogueText.fontSize = 0.35f;
        text = text.Replace("\n", "\n\n");
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

            if (tag.StartsWith("face:"))
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
        Debug.Log("Changing sprite to: " + spriteName);
        if (currentClientExpressions == null || currentClientExpressions.Count == 0)
        {
            Debug.Log("We Returned From Here!");
            return;
        }

        Sprite newExpression = currentClientExpressions[spriteName];

        /*switch (spriteName)
        {
            case "neutral":
                newExpression = currentClientExpressions[0];
                break;
            case "happy":
                newExpression = currentClientExpressions.Length > 1 ? currentClientExpressions[1] : currentClientExpressions[0];
                break;
            case "fake_happy":
                newExpression = currentClientExpressions.Length > 2 ? currentClientExpressions[2] : currentClientExpressions[0];
                break;
            case "mask_happy":
                newExpression = currentClientExpressions.Length > 3 ? currentClientExpressions[3] : currentClientExpressions[0];
                break;
            case "mask_neutral":
                newExpression = currentClientExpressions.Length > 4 ? currentClientExpressions[4] : currentClientExpressions[0];
                break;
            default:
                Debug.LogWarning("Unknown sprite tag: " + spriteName);
                break;
        }*/

        // Apply only to the overlay
        expressionImage.sprite = newExpression;
        characterImage.sprite = newExpression;
        //expressionImage.preserveAspect = false; 
    }


}
