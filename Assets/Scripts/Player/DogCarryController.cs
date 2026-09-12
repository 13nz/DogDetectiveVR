using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// allows the dog to detect and carry nearby objects
/// disables carried object colliders while carrying
/// restores the object when the grab button is released
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

    private CarryableObject carriedObject;
    private Rigidbody carriedRigidbody;
    private Collider[] carriedColliders;

    public CarryableObject CarriedObject => carriedObject;

    private void OnEnable()
    {
        // stops the script if no grab action exists
        if (grabAction == null)
        {
            return;
        }

        // enables the grab action
        grabAction.action.Enable();

        // listens for grabbing and releasing
        grabAction.action.performed += OnGrabPressed;
        grabAction.action.canceled += OnGrabReleased;
    }

    private void OnDisable()
    {
        // stops the script if no grab action exists
        if (grabAction == null)
        {
            return;
        }

        // removes the input listeners
        grabAction.action.performed -= OnGrabPressed;
        grabAction.action.canceled -= OnGrabReleased;

        // disables the grab action
        grabAction.action.Disable();

        // releases the current object
        ReleaseCarriedObject();
    }

    private void OnGrabPressed(InputAction.CallbackContext context)
    {
        // prevents carrying multiple objects
        if (carriedObject != null)
        {
            return;
        }

        // checks the required references
        if (cameraTransform == null || carryPoint == null)
        {
            return;
        }

        // creates a ray from the camera
        Ray ray = new Ray(
            cameraTransform.position,
            cameraTransform.forward
        );

        // searches for a carryable object
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

        // finds the carryable object
        CarryableObject carryable =
            hit.collider.GetComponentInParent<CarryableObject>();

        // stops if the object cannot be carried
        if (carryable == null)
        {
            Debug.Log(
                "the detected object does not have a carryable object script"
            );

            return;
        }

        // stores the object
        carriedObject = carryable;

        // finds the rigidbody
        carriedRigidbody =
            carriedObject.GetComponentInParent<Rigidbody>();

        // finds all colliders
        carriedColliders =
            carriedObject.GetComponentsInChildren<Collider>(true);

        // disables physics while carrying
        if (carriedRigidbody != null)
        {
            carriedRigidbody.isKinematic = true;
            carriedRigidbody.linearVelocity = Vector3.zero;
            carriedRigidbody.angularVelocity = Vector3.zero;
        }

        // disables colliders while carrying
        if (carriedColliders != null)
        {
            foreach (Collider carriedCollider in carriedColliders)
            {
                if (carriedCollider != null)
                {
                    carriedCollider.enabled = false;
                }
            }
        }

        // moves the object to the carry point
        carriedObject.transform.SetPositionAndRotation(
            carryPoint.position,
            carryPoint.rotation
        );

        // checks whether this is the objective item
        ObjectiveItem objectiveItem =
            carriedObject.GetComponentInParent<ObjectiveItem>();

        // tells the game manager that the keys were found
        if (objectiveItem != null &&
            GameManager.Instance != null &&
            GameManager.Instance.CurrentState ==
            GameManager.GameState.FindKeys)
        {
            GameManager.Instance.KeysCollected();
        }
    }

    private void OnGrabReleased(InputAction.CallbackContext context)
    {
        // stops if nothing is being carried
        if (carriedObject == null)
        {
            return;
        }

        // checks whether this is the objective item
        ObjectiveItem objectiveItem =
            carriedObject.GetComponentInParent<ObjectiveItem>();

        // checks whether the player is inside the table zone
        bool atTable =
            GameManager.Instance != null &&
            GameManager.Instance.IsAtKeyPlacementArea();

        // releases the object first
        ReleaseCarriedObject();

        // completes the objective after the keys are released
        if (objectiveItem != null &&
            atTable &&
            GameManager.Instance != null)
        {
            GameManager.Instance.KeysPlacedOnTable();
        }
    }

    private void ReleaseCarriedObject()
    {
        // stops if nothing is being carried
        if (carriedObject == null)
        {
            return;
        }

        // restores all colliders
        if (carriedColliders != null)
        {
            foreach (Collider carriedCollider in carriedColliders)
            {
                if (carriedCollider != null)
                {
                    carriedCollider.enabled = true;
                }
            }
        }

        // restores normal physics
        if (carriedRigidbody != null)
        {
            carriedRigidbody.isKinematic = false;
            carriedRigidbody.linearVelocity = Vector3.zero;
            carriedRigidbody.angularVelocity = Vector3.zero;
        }

        // clears the carrying references
        carriedObject = null;
        carriedRigidbody = null;
        carriedColliders = null;
    }

    private void LateUpdate()
    {
        // stops if nothing is being carried
        if (carriedObject == null || carryPoint == null)
        {
            return;
        }

        // keeps the object at the carry point
        carriedObject.transform.SetPositionAndRotation(
            carryPoint.position,
            carryPoint.rotation
        );
    }

    public bool IsCarryingObjectiveItem()
    {
        // stops if nothing is being carried
        if (carriedObject == null)
        {
            return false;
        }

        // checks whether the object is an objective item
        ObjectiveItem objectiveItem =
            carriedObject.GetComponentInParent<ObjectiveItem>();

        return objectiveItem != null;
    }
}