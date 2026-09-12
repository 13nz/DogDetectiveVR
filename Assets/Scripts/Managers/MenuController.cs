using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Menu Input")]
    [SerializeField] private InputActionReference menuAction;

    [Header("Player Object")]
    [SerializeField] private GameObject playerObject;

    [Header("Scenes")]
    [SerializeField] private string introSceneName = "Intro";

    private DogCarryController dogCarryController;
    private DogPoseController dogPoseController;
    private DogSniffController dogSniffController;
    private DogSprintController dogSprintController;

    private bool menuOpen;

    private void Awake()
    {
        menuOpen = false;

        FindPlayerControllers();

        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        SetGameplayEnabled(true);
    }

    private void OnEnable()
    {
        if (menuAction != null)
        {
            menuAction.action.performed += OnMenuButtonPressed;
            menuAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (menuAction != null)
        {
            menuAction.action.performed -= OnMenuButtonPressed;
            menuAction.action.Disable();
        }

        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    private void FindPlayerControllers()
    {
        if (playerObject == null)
        {
            Debug.LogWarning("player object is not assigned");
            return;
        }

        dogCarryController = playerObject.GetComponent<DogCarryController>();
        dogPoseController = playerObject.GetComponent<DogPoseController>();
        dogSniffController = playerObject.GetComponent<DogSniffController>();
        dogSprintController = playerObject.GetComponent<DogSprintController>();
    }

    private void OnMenuButtonPressed(InputAction.CallbackContext context)
    {
        ToggleMenu();
    }

    public void ToggleMenu()
    {
        if (menuOpen)
        {
            CloseMenu();
        }
        else
        {
            OpenMenu();
        }
    }

    public void OpenMenu()
    {
        menuOpen = true;

        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        SetGameplayEnabled(false);

        Time.timeScale = 0f;
        AudioListener.pause = true;
    }

    public void CloseMenu()
    {
        menuOpen = false;

        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        Time.timeScale = 1f;
        AudioListener.pause = false;

        SetGameplayEnabled(true);
    }

    public void ResumeGame()
    {
        CloseMenu();
    }

    public void OpenSettings()
    {
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }
    }

    public void QuitToIntro()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        SetGameplayEnabled(true);

        SceneManager.LoadScene(introSceneName);
    }

    private void SetGameplayEnabled(bool enabled)
    {
        if (dogCarryController != null)
        {
            dogCarryController.enabled = enabled;
        }

        if (dogPoseController != null)
        {
            dogPoseController.enabled = enabled;
        }

        if (dogSniffController != null)
        {
            dogSniffController.enabled = enabled;
        }

        if (dogSprintController != null)
        {
            dogSprintController.enabled = enabled;
        }
    }
}