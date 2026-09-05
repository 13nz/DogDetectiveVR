using UnityEngine;
using UnityEngine.InputSystem;

public class DogPoseController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraOffset;

    [Header("Input")]
    [SerializeField] private InputActionReference standAction;

    [Header("Standing Settings")]
    [SerializeField] private float standingHeightOffset = 0.5f;
    [SerializeField] private float transitionSpeed = 5f;

    // stores the default camera offset so the player
    // always returns to the normal dog height
    private Vector3 defaultPosition;

    // tracks whether the player is currently standing
    private bool isStanding;

    private void Awake()
    {
        defaultPosition = cameraOffset.localPosition;
    }

    private void OnEnable()
    {
        if (standAction != null)
        {
            standAction.action.Enable();
            standAction.action.performed += OnStandPressed;
            standAction.action.canceled += OnStandReleased;
        }
    }

    private void OnDisable()
    {
        if (standAction != null)
        {
            standAction.action.performed -= OnStandPressed;
            standAction.action.canceled -= OnStandReleased;
            standAction.action.Disable();
        }
    }

    private void Update()
    {
        // smoothly moves the camera upward while
        // the player holds the left grip button

        Vector3 targetPosition = defaultPosition;

        if (isStanding)
        {
            targetPosition.y += standingHeightOffset;
        }

        cameraOffset.localPosition = Vector3.Lerp(
            cameraOffset.localPosition,
            targetPosition,
            transitionSpeed * Time.deltaTime);
    }

    private void OnStandPressed(InputAction.CallbackContext context)
    {
        isStanding = true;
    }

    private void OnStandReleased(InputAction.CallbackContext context)
    {
        isStanding = false;
    }
}