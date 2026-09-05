using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// allows the dog to pick up and carry nearby objects.
/// objects are carried at the carry point in front of the camera.
/// </summary>
public class DogCarryController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform carryPoint;
    [SerializeField] private InputActionReference grabAction;

    [Header("Detection")]
    [SerializeField] private float maxCarryDistance = 0.8f;
    [SerializeField] private float detectionRadius = 0.12f;
    [SerializeField] private LayerMask carryableLayers = ~0;


    // stores the object currently being carried
    private CarryableObject carriedObject;

    /// <summary>
    /// returns the object currently being carried.
    /// </summary>
    public CarryableObject CarriedObject => carriedObject;

    private void OnEnable()
    {
        grabAction.action.Enable();

        grabAction.action.performed += OnGrabPressed;
        grabAction.action.canceled += OnGrabReleased;
    }

    private void OnDisable()
    {
        grabAction.action.performed -= OnGrabPressed;
        grabAction.action.canceled -= OnGrabReleased;

        grabAction.action.Disable();
    }

    private void OnGrabPressed(InputAction.CallbackContext context)
    {
        // don't pick up another object if one is already being carried
        if (carriedObject != null)
            return;

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        if (Physics.SphereCast(ray, detectionRadius, out RaycastHit hit, maxCarryDistance, carryableLayers))
        {
            CarryableObject carryable = hit.collider.GetComponent<CarryableObject>();

            if (carryable == null)
                return;

            carriedObject = carryable;

            Rigidbody rb = carriedObject.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.isKinematic = true;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            carriedObject.transform.position = carryPoint.position;
            carriedObject.transform.rotation = carryPoint.rotation;

            // check if objective item and update
            ObjectiveItem objectiveItem = carriedObject.GetComponent<ObjectiveItem>();

            if (objectiveItem != null &&
                GameManager.Instance.CurrentState == GameManager.GameState.FindKeys)
            {
                GameManager.Instance.KeysCollected();
            }
        }
    }

    private void OnGrabReleased(InputAction.CallbackContext context)
    {
        if (carriedObject == null)
            return;

        Rigidbody rb = carriedObject.GetComponent<Rigidbody>();

        // carriedObject.transform.SetParent(null);

        if (rb != null)
        {
            rb.isKinematic = false;
        }

        carriedObject = null;
    }

    private void LateUpdate()
    {
        if (carriedObject == null)
            return;

        carriedObject.transform.position = carryPoint.position;
        carriedObject.transform.rotation = carryPoint.rotation;
    }

    /// <summary>
    /// returns true if the player is carrying the keys
    /// </summary>
    public bool IsCarryingObjectiveItem()
    {

        if (carriedObject == null)
            return false;

        ObjectiveItem objectiveItem = carriedObject.GetComponent<ObjectiveItem>();

        return objectiveItem != null;
    }
}