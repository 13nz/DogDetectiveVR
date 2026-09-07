
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// controls sprinting while the left grip is held.
/// </summary>
public class DogSprintController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference sprintAction;

    [Header("Movement")]
    [SerializeField] private float normalMoveSpeed = 1.5f;
    [SerializeField] private float sprintMoveSpeed = 3.5f;

    [Header("Audio")]
    [SerializeField] private DogAudioController dogAudioController;

    public bool IsSprinting { get; private set; }

    private void OnEnable()
    {
        if (sprintAction == null)
            return;

        sprintAction.action.Enable();
        sprintAction.action.performed += OnSprintStarted;
        sprintAction.action.canceled += OnSprintEnded;
    }

    private void OnDisable()
    {
        if (sprintAction == null)
            return;

        sprintAction.action.performed -= OnSprintStarted;
        sprintAction.action.canceled -= OnSprintEnded;
        sprintAction.action.Disable();

        IsSprinting = false;
    }

    private void OnSprintStarted(InputAction.CallbackContext context)
    {
        IsSprinting = true;

        if (dogAudioController != null)
        {
            dogAudioController.SetSprinting(true);
        }
    }

    private void OnSprintEnded(InputAction.CallbackContext context)
    {
        IsSprinting = false;

        if (dogAudioController != null)
        {
            dogAudioController.SetSprinting(false);
        }
    }
}