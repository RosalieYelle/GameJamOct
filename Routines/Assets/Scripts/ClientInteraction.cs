using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;


public class ClientInteraction : MonoBehaviour
{
    public GameObject dialogueScene;
    public GameObject interactUI; //Press E text
    public CinemachineCamera closeUpCamera;
    public CinemachineCamera mainCamera; //Top down cam
    private DialogueManager dialogueManager;

    private bool playerInRange;
    private bool dialogueStarted = false;

    public UIManager uiManager;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        interactUI.SetActive(false);
        dialogueManager = FindFirstObjectByType<DialogueManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && Keyboard.current.eKey.isPressed && !dialogueStarted)
        {
            StartDialogue();
        }
    }
    void StartDialogue()
    {
        var dayManager = FindFirstObjectByType<DayManager>();

        // SAFETY CHECK
        if (dayManager.currentDay >= dayManager.days.Length ||
            dayManager.currentClient >= dayManager.days[dayManager.currentDay].clients.Length)
        {
            Debug.LogWarning("No more clients today!");
            return;
        }

        dialogueStarted = true;

        // Switch cameras
        mainCamera.Priority = 0;
        closeUpCamera.Priority = 20;

        // Show dialogue UI
        dialogueScene.SetActive(true);

        // Disable player movement
        FindFirstObjectByType<PlayerController>().canMove = false;

        // Start this client's dialogue
        var clientData = dayManager.days[dayManager.currentDay].clients[dayManager.currentClient];
        dialogueManager.StartDialogue(clientData);

        dialogueManager.onDialogueEnd = () =>
        {
            EndInteraction();
            dayManager.OnDialogueFinished();
        };
    }


    public void EndInteraction()
    {
        dialogueScene.SetActive(false);

        // Switch back to main camera
        closeUpCamera.Priority = 0;
        mainCamera.Priority = 10;

        // Re-enable player movement
        FindFirstObjectByType<PlayerController>().canMove = true;

        dialogueStarted = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            interactUI.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            interactUI.SetActive(false);
        }
    }
}
