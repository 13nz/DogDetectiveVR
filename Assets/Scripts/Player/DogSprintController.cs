using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class DogSprintController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference sprintAction;

    [Header("Movement")]
    [SerializeField] private float normalMoveSpeed = 1.5f;
    [SerializeField] private float sprintMoveSpeed = 3.5f;

    [Header("Audio")]
    [SerializeField] private DogAudioController dogAudioController;

    private DynamicMoveProvider dynamicMoveProvider;

    public bool IsSprinting { get; private set; }

    private void Awake()
    {
        // finds the existing Dynamic Move Provider on this XR Origin
        dynamicMoveProvider = GetComponentInChildren<DynamicMoveProvider>(true);
        if (dynamicMoveProvider == null)
        {
            Debug.LogError(
                "DogSprintController requires a Dynamic Move Provider on the same XR Origin."
            );
        }
    }

    private void Start()
    {
        ApplyMovementSpeed();
    }

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
        ApplyMovementSpeed();
    }

    private void OnSprintStarted(InputAction.CallbackContext context)
    {
        IsSprinting = true;

        ApplyMovementSpeed();

        if (dogAudioController != null)
            dogAudioController.SetSprinting(true);
    }

    private void OnSprintEnded(InputAction.CallbackContext context)
    {
        IsSprinting = false;

        ApplyMovementSpeed();

        if (dogAudioController != null)
            dogAudioController.SetSprinting(false);
    }

    private void ApplyMovementSpeed()
    {
        if (dynamicMoveProvider == null)
            return;

        dynamicMoveProvider.moveSpeed = IsSprinting
            ? sprintMoveSpeed
            : normalMoveSpeed;
    }
}