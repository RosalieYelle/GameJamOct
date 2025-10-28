using UnityEngine;
using UnityEngine.UI;

// Optional Input System support:
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// UI manager: toggles corner menu, controls global volume and Exit.
/// Improvements: keeps menu button on top so it remains clickable, and
/// supports the new Input System via an InputActionReference or callback.
/// </summary>
public class MenuManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject menuPanel;     // the panel to show/hide
    public Button menuButton;        // the small corner MenuButton (must remain clickable)
    public Slider volumeSlider;      // the slider inside the menu
    public Text volumeText;          // text that shows percent
    public Button exitButton;        // the Exit button

    [Header("Settings")]
    [Range(0f, 1f)]
    public float defaultVolume = 1f; // starting volume
    public KeyCode toggleKey = KeyCode.Escape; // fallback input (old Input system)

    // Optional: assign in Inspector to use the new Input System (InputAction asset).
    // Create an Input Action (e.g., "UI/ToggleMenu") bound to "Escape" and reference it here.
#if ENABLE_INPUT_SYSTEM
    public InputActionReference toggleActionReference = null;
#endif

    void Start()
    {
        if (menuPanel == null || menuButton == null)
        {
            Debug.LogWarning("UIManager: assign menuPanel and menuButton in the Inspector.");
        }

        // Make sure the MenuButton stays on top of other UI so you can always click it.
        // If MenuButton is a child of the same Canvas, this will move it to the end of sibling list (rendered on top).
        if (menuButton != null)
            menuButton.transform.SetAsLastSibling();

        // Initialize global volume & UI
        AudioListener.volume = Mathf.Clamp01(defaultVolume);
        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
            volumeSlider.value = AudioListener.volume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
        UpdateVolumeText(AudioListener.volume);

        // Hook button clicks
        if (menuButton != null) menuButton.onClick.AddListener(ToggleMenu);
        if (exitButton != null) exitButton.onClick.AddListener(QuitGame);

        // Start hidden
        if (menuPanel != null) menuPanel.SetActive(false);
    }

    void OnEnable()
    {
#if ENABLE_INPUT_SYSTEM
        // If an InputActionReference was assigned, subscribe to it
        if (toggleActionReference != null && toggleActionReference.action != null)
        {
            toggleActionReference.action.performed += OnToggleActionPerformed;
            // Ensure the action is enabled
            toggleActionReference.action.Enable();
        }
#endif
    }

    void OnDisable()
    {
#if ENABLE_INPUT_SYSTEM
        if (toggleActionReference != null && toggleActionReference.action != null)
        {
            toggleActionReference.action.performed -= OnToggleActionPerformed;
            toggleActionReference.action.Disable();
        }
#endif
    }

    void Update()
    {
        // Keep the button on top in case other UI is created dynamically later
        if (menuButton != null)
            menuButton.transform.SetAsLastSibling();

        // Fallback for old Input system (or simple keyboard input)
    }

#if ENABLE_INPUT_SYSTEM
    // Called when the InputActionReference is triggered
    private void OnToggleActionPerformed(InputAction.CallbackContext ctx)
    {
        ToggleMenu();
    }

    // Also provide a method that PlayerInput can call directly (if you prefer wiring it that way)
    // e.g., in PlayerInput's Actions map, bind a "ToggleMenu" action and set the component to call this.
    public void OnToggleMenu(InputAction.CallbackContext context)
    {
        if (context.performed) ToggleMenu();
    }
#endif

    /// <summary>
    /// Toggles the menu open/closed. If the panel covers the button, SetAsLastSibling() above keeps the button clickable.
    /// </summary>
    public void ToggleMenu()
    {
        if (menuPanel == null) return;
        bool next = !menuPanel.activeSelf;
        menuPanel.SetActive(next);

        // ensure MenuButton remains on top after toggling
        if (menuButton != null) menuButton.transform.SetAsLastSibling();
    }

    public void SetVolume(float value)
    {
        float v = Mathf.Clamp01(value);
        AudioListener.volume = v;
        UpdateVolumeText(v);
    }

    void UpdateVolumeText(float v)
    {
        if (volumeText != null)
            volumeText.text = $"Volume: {(int)(v * 100f)}%";
    }

    public void QuitGame()
    {
        Debug.Log("Quit requested.");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}