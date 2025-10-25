using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;


public class ClientInteraction : MonoBehaviour
{
    public GameObject dialogueScene;
    public GameObject interactUI;
    public CinemachineCamera closeUpCamera;

    private bool playerInRange;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        interactUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && Keyboard.current.eKey.isPressed)
        {
            dialogueScene.SetActive(true);
            closeUpCamera.Priority = 20;
            //TODO: Disable player movement here
        }
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

    public void EndInteraction()
    {
        dialogueScene.SetActive(false);
        closeUpCamera.Priority = 0; // top-down becomes active again
        // TODO: Re-enable player movement
    }
}
