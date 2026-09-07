using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// allows the dog to detect and carry nearby objects
/// disables carried object colliders to prevent physics from pushing the dog upward
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

    // stores the rigidbody belonging to the carried object
    private Rigidbody carriedRigidbody;

    // stores every collider belonging to the carried object and its children
    private Collider[] carriedColliders;

    /// <summary>
    /// returns the object currently being carried
    /// </summary>
    public CarryableObject CarriedObject => carriedObject;

    private void OnEnable()
    {
        // stops the script from running if no grab action has been assigned
        if (grabAction == null)
            return;

        // enables the assigned grab input action
        grabAction.action.Enable();

        // listens for the grab button being pressed or released
        grabAction.action.performed += OnGrabPressed;
        grabAction.action.canceled += OnGrabReleased;
    }

    private void OnDisable()
    {
        // stops the script from running if no grab action has been assigned
        if (grabAction == null)
            return;

        // removes the input event listeners
        grabAction.action.performed -= OnGrabPressed;
        grabAction.action.canceled -= OnGrabReleased;

        // disables the assigned grab input action
        grabAction.action.Disable();

        // releases the object if the script is disabled while carrying something
        ReleaseCarriedObject();
    }

    private void OnGrabPressed(InputAction.CallbackContext context)
    {
        // prevents the dog from carrying more than one object at a time
        if (carriedObject != null)
            return;

        // checks that the required references have been assigned
        if (cameraTransform == null || carryPoint == null)
            return;

        // creates a ray that starts at the camera and points forward
        Ray ray = new Ray(
            cameraTransform.position,
            cameraTransform.forward
        );

        // checks for a carryable object in front of the dog
        if (!Physics.SphereCast(
                ray,
                detectionRadius,
                out RaycastHit hit,
                maxCarryDistance,
                carryableLayers,
                QueryTriggerInteraction.Ignore))
        {
            return;
        }

        // searches the detected object and its parents for the carryable script
        CarryableObject carryable =
            hit.collider.GetComponentInParent<CarryableObject>();

        // stops if the detected object is not carryable
        if (carryable == null)
        {
            Debug.Log("the detected object does not have a carryable object script");
            return;
        }

        // stores the object that will be carried
        carriedObject = carryable;

        // searches the object and its parents for a rigidbody
        carriedRigidbody =
            carriedObject.GetComponentInParent<Rigidbody>();

        // stores all colliders belonging to the object and its children
        carriedColliders =
            carriedObject.GetComponentsInChildren<Collider>(true);

        // disables physics movement while the object is being carried
        if (carriedRigidbody != null)
        {
            carriedRigidbody.isKinematic = true;
            carriedRigidbody.linearVelocity = Vector3.zero;
            carriedRigidbody.angularVelocity = Vector3.zero;
        }

        // disables the carried colliders so they cannot push the character controller
        if (carriedColliders != null)
        {
            foreach (Collider carriedCollider in carriedColliders)
            {
                if (carriedCollider != null)
                    carriedCollider.enabled = false;
            }
        }

        // moves the object to the carry point
        carriedObject.transform.SetPositionAndRotation(
            carryPoint.position,
            carryPoint.rotation
        );

        // checks whether the carried object is an objective item
        ObjectiveItem objectiveItem =
            carriedObject.GetComponentInParent<ObjectiveItem>();

        // completes the key objective when the correct item is collected
        if (objectiveItem != null &&
            GameManager.Instance != null &&
            GameManager.Instance.CurrentState == GameManager.GameState.FindKeys)
        {
            GameManager.Instance.KeysCollected();
        }
    }

    private void OnGrabReleased(InputAction.CallbackContext context)
    {
        // stops if there is no object being carried
        if (carriedObject == null)
            return;

        // releases the carried object
        ReleaseCarriedObject();
    }

    private void ReleaseCarriedObject()
    {
        // stops if there is no object being carried
        if (carriedObject == null)
            return;

        // restores the colliders after the object is released
        if (carriedColliders != null)
        {
            foreach (Collider carriedCollider in carriedColliders)
            {
                if (carriedCollider != null)
                    carriedCollider.enabled = true;
            }
        }

        // restores normal rigidbody physics after the object is released
        if (carriedRigidbody != null)
        {
            carriedRigidbody.isKinematic = false;
        }

        // clears the stored object references
        carriedObject = null;
        carriedRigidbody = null;
        carriedColliders = null;
    }

    private void LateUpdate()
    {
        // stops if there is no object being carried
        if (carriedObject == null || carryPoint == null)
            return;

        // keeps the carried object aligned with the carry point
        carriedObject.transform.SetPositionAndRotation(
            carryPoint.position,
            carryPoint.rotation
        );
    }

    /// <summary>
    /// returns true when the dog is carrying an objective item
    /// </summary>
    public bool IsCarryingObjectiveItem()
    {
        // returns false when the dog is not carrying anything
        if (carriedObject == null)
            return false;

        // searches the carried object and its parents for an objective item
        ObjectiveItem objectiveItem =
            carriedObject.GetComponentInParent<ObjectiveItem>();

        // returns true only when the carried object is an objective item
        return objectiveItem != null;
    }
}