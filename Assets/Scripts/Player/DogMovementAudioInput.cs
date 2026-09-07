
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// connects the left controller thumbstick to the dog audio controller
/// </summary>
public class DogMovementAudioInput : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private DogAudioController dogAudioController;

    [Header("Settings")]
    [SerializeField] private float movementThreshold = 0.01f;

    private bool wasMoving;

    private void OnEnable()
    {
        if (moveAction != null)
        {
            moveAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (moveAction != null)
        {
            moveAction.action.Disable();
        }

        if (dogAudioController != null)
        {
            dogAudioController.SetWalking(false);
        }

        wasMoving = false;
    }

    private void Update()
    {
        if (moveAction == null || dogAudioController == null)
        {
            return;
        }

        Vector2 movementInput = moveAction.action.ReadValue<Vector2>();
        bool isMoving = movementInput.sqrMagnitude > movementThreshold;

        if (isMoving)
        {
            dogAudioController.SetWalking(true);
        }
        else if (wasMoving)
        {
            dogAudioController.SetWalking(false);
            dogAudioController.SetStoppedAfterWalking();
        }

        wasMoving = isMoving;
    }
}