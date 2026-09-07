using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// adds a subtle walking head-bobbing effect to the dog's camera.
/// </summary>
public class DogHeadBobbing : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private InputActionReference moveAction;

    [Header("Walking bob")]
    [SerializeField] private float bobFrequency = 6f;
    [SerializeField] private float bobVerticalAmount = 0.025f;
    [SerializeField] private float bobHorizontalAmount = 0.015f;

    [Header("Sprinting bob")]
    [SerializeField] private float sprintBobFrequency = 8f;
    [SerializeField] private float sprintVerticalAmount = 0.035f;
    [SerializeField] private float sprintHorizontalAmount = 0.02f;

    [Header("Smoothing")]
    [SerializeField] private float movementSmoothing = 8f;
    [SerializeField] private float returnSpeed = 10f;

    private Vector3 startingLocalPosition;
    private float bobTimer;
    private float movementAmount;

    private DogSprintController sprintController;

    private void Awake()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main != null ? Camera.main.transform : null;

        if (cameraTransform != null)
            startingLocalPosition = cameraTransform.localPosition;

        sprintController = GetComponentInParent<DogSprintController>();
    }

    private void Update()
    {
        if (cameraTransform == null)
            return;

        Vector2 movementInput = Vector2.zero;

        if (moveAction != null)
            movementInput = moveAction.action.ReadValue<Vector2>();

        float targetMovementAmount = Mathf.Clamp01(movementInput.magnitude);

        movementAmount = Mathf.Lerp(
            movementAmount,
            targetMovementAmount,
            movementSmoothing * Time.deltaTime
        );

        bool isSprinting =
            sprintController != null &&
            sprintController.IsSprinting;

        float frequency = isSprinting
            ? sprintBobFrequency
            : bobFrequency;

        float verticalAmount = isSprinting
            ? sprintVerticalAmount
            : bobVerticalAmount;

        float horizontalAmount = isSprinting
            ? sprintHorizontalAmount
            : bobHorizontalAmount;

        if (movementAmount > 0.05f)
        {
            bobTimer += Time.deltaTime * frequency;

            float verticalBob =
                Mathf.Sin(bobTimer * 2f) *
                verticalAmount *
                movementAmount;

            float horizontalBob =
                Mathf.Cos(bobTimer) *
                horizontalAmount *
                movementAmount;

            Vector3 targetPosition = startingLocalPosition;

            targetPosition.y += verticalBob;
            targetPosition.x += horizontalBob;

            cameraTransform.localPosition = Vector3.Lerp(
                cameraTransform.localPosition,
                targetPosition,
                returnSpeed * Time.deltaTime
            );
        }
        else
        {
            bobTimer = 0f;

            cameraTransform.localPosition = Vector3.Lerp(
                cameraTransform.localPosition,
                startingLocalPosition,
                returnSpeed * Time.deltaTime
            );
        }
    }
}