using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Text;



[System.Serializable]
public class ClientData {
    public int name;
    public TextAsset inkFile;
    
    //public Sprite[] expressionSprites; // 0=neutral, 1=happy, 2=FakeHappy, 3=Mask
}

[System.Serializable]
public class CharacterData {
    [Header("Expression Sprites")]
    public Dictionary<string, Sprite> expressionSprites = new Dictionary<string, Sprite>();

    public string[] expressionNames;
    public Sprite[] expressionOrig;
    public string portraitFolder;
    public Sprite clientSpriteLayingDown;
    public Sprite portrait;
}

[System.Serializable]
public class DayData {
    public ClientData[] clients; //Size 2, cause 2 per day
}

[System.Serializable]
public class ClientInteraction : MonoBehaviour
{
    public GameObject dialogueScene;
    public GameObject interactUI; //Press E text
    public GameObject chair;
    public GameObject player;
    public GameObject start;
    public CinemachineCamera closeUpCamera;
    public CinemachineCamera mainCamera; //Top down cam
    private DialogueManager dialogueManager;

    private bool dialogueStarted = false;

    public UIManager uiManager;

    public CharacterData[] characters;
    public DayData[] days; //size 3, since 3 days

    public int currentDay = 0;
    public int currentClient = 0;
    public bool newDay = true;
    public bool newClient = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            for (int j = 0; j < characters[i].expressionNames.Length; j++)
            {
                var name = characters[i].expressionNames[j];
                characters[i].expressionSprites[name] = characters[i].expressionOrig[j];
            }
        }

        //interactUI.SetActive(false);
        dialogueManager = FindFirstObjectByType<DialogueManager>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (newClient) 
        {
            NewDay();
        }

        if (currentDay >= days.Length)
        {
            newClient = true;
            return; // Game over
        }

        ChairDetector chairDetector = chair.GetComponent<ChairDetector>();
        if (chairDetector.playerInRange && Keyboard.current.eKey.isPressed && !dialogueStarted)
        {
            StartNextClient();
        }
    }

    void NewDay()
    {

        if (!newDay && !newClient) return;

        if (newDay && currentDay == days.Length - 1)
        {
            newDay = false;
            StartNextClient();

        }

        if (newDay == true && currentDay == days.Length)
        {
            newDay = false;
            uiManager.ShowEnd();
        }

        if (newDay == true && currentClient == 0 && currentDay < days.Length)
        {
            CrossfadeAudioManager music = FindFirstObjectByType<CrossfadeAudioManager>();
            music.PlayTrack(System.Math.Min(2, currentDay));
            if (currentDay == 0)
            {
                uiManager.ShowFirstDay();
            }
            else
            {
                uiManager.ShowNewDay(currentDay);
            }

            newDay = false;
        }
        
        if (newClient == true && currentDay < days.Length)
        {
            player.transform.position = start.transform.position;
            Debug.Log(characters[days[currentDay].clients[currentClient].name]);
            dialogueManager.ChangeClient(characters[days[currentDay].clients[currentClient].name]);
            newClient = false;
        }
    }

    void StartNextClient()
    {
        
        // SAFETY CHECK
        if (currentDay >= days.Length ||
            currentClient >= days[currentDay].clients.Length)
        {
            Debug.LogWarning("No more clients today!");
            return;
        }

        mainCamera.Priority = 0;
        closeUpCamera.Priority = 20;
        dialogueStarted = true;
        dialogueScene.SetActive(true);


        FindFirstObjectByType<PlayerController>().canMove = false;
        //Debug.Log(currentClient);

        // Show the day panel only if this is the first client of the day

        var client = days[currentDay].clients[currentClient];
        var character = characters[client.name];
        //Debug.Log($"StartDialogue for client '{client.name}' -> character '{character}'. expressionSprites instance id: {(character.expressionSprites != null ? character.expressionSprites.GetHashCode().ToString() : "null")}");

        // Update UI portrait
        uiManager.UpdatePortrait(character.portrait);

        // Start the Ink dialogue
        dialogueManager.StartDialogue(client, character);

        // Assign the callback — must match the method name exactly
        dialogueManager.onDialogueEnd = OnDialogueFinished;
    }


    public void OnDialogueFinished()
    {

        dialogueScene.SetActive(false);

        // Switch back to main camera
        dialogueStarted = false;
        closeUpCamera.Priority = 0;
        mainCamera.Priority = 10;


        // Re-enable player movement
        FindFirstObjectByType<PlayerController>().canMove = true;

        currentClient++;
        newClient = true;

        if (currentClient >= days[currentDay].clients.Length)
        {
            currentClient = 0;
            currentDay++;
            newDay = true;

            if (currentDay >= days.Length)
            {
                Debug.Log("Game Finished!");
                return;
            }

            // Next day will now show panel at the start automatically in StartNextClient
        }

    }

}
