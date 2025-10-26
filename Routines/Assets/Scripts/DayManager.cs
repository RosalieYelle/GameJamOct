using UnityEngine;
[System.Serializable]
public class ClientData
{
    public TextAsset inkFile;
    public Sprite clientSpriteLayingDown;
    public Sprite portrait;
}

[System.Serializable]
public class DayData
{
    public ClientData[] clients; //Size 2, cause 2 per day
}
public class DayManager : MonoBehaviour
{
    public DayData[] days; //size 3, since 3 days
    public int currentDay = 0;
    public int currentClient = 0;

    public DialogueManager dialogueManager;
    public UIManager uiManager;

    void Start()
    {
        // Start the first client/day automatically at game start
        StartNextClient();
    }
    
    public void StartNextClient()
    {
        if (currentDay >= days.Length)
        {
            Debug.Log("Game Finished!");
            return;
        }

        // Show the day panel only if this is the first client of the day
        if (currentClient == 0)
        {
            uiManager.ShowNewDay(currentDay);
        }

        var data = days[currentDay].clients[currentClient];

        // Update UI portrait
        uiManager.UpdatePortrait(data.portrait);

        // Start the Ink dialogue
        dialogueManager.StartDialogue(data.inkFile);

        // Assign the callback — must match the method name exactly
        dialogueManager.onDialogueEnd = OnDialogueFinished;
    }

    // This method MUST be inside the class
    public void OnDialogueFinished()
    {
        currentClient++;

        if (currentClient >= days[currentDay].clients.Length)
        {
            currentClient = 0;
            currentDay++;

            if (currentDay >= days.Length)
            {
                Debug.Log("Game Finished!");
                return;
            }

            // Next day will now show panel at the start automatically in StartNextClient
        }

        StartNextClient();
    }
   

    
}
