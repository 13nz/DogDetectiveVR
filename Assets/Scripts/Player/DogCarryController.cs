using System.Collections;
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

    [Header("Placement")]
    [SerializeField] private float placementCheckDelay = 0.4f;

    private CarryableObject carriedObject;
    private Rigidbody carriedRigidbody;
    private Collider[] carriedColliders;

    public CarryableObject CarriedObject => carriedObject;

    private void OnEnable()
    {
        if (grabAction == null)
        {
            return;
        }

        grabAction.action.Enable();
        grabAction.action.performed += OnGrabPressed;
        grabAction.action.canceled += OnGrabReleased;
    }

    private void OnDisable()
    {
        if (grabAction == null)
        {
            return;
        }

        grabAction.action.performed -= OnGrabPressed;
        grabAction.action.canceled -= OnGrabReleased;
        grabAction.action.Disable();

        ReleaseCarriedObject();
    }

    private void OnGrabPressed(InputAction.CallbackContext context)
    {
        if (carriedObject != null)
        {
            return;
        }

        if (cameraTransform == null || carryPoint == null)
        {
            return;
        }

        Ray ray = new Ray(
            cameraTransform.position,
            cameraTransform.forward
        );

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

        CarryableObject carryable =
            hit.collider.GetComponentInParent<CarryableObject>();

        if (carryable == null)
        {
            Debug.Log("the detected object does not have a carryable object script");
            return;
        }

        carriedObject = carryable;

        carriedRigidbody =
            carriedObject.GetComponentInParent<Rigidbody>();

        carriedColliders =
            carriedObject.GetComponentsInChildren<Collider>(true);

        // disables physics while the dog is carrying the object
        if (carriedRigidbody != null)
        {
            carriedRigidbody.isKinematic = true;
            carriedRigidbody.linearVelocity = Vector3.zero;
            carriedRigidbody.angularVelocity = Vector3.zero;
        }

        // disables the object colliders while carrying
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

        // moves the object to the dogs carry point
        carriedObject.transform.SetPositionAndRotation(
            carryPoint.position,
            carryPoint.rotation
        );

        ObjectiveItem objectiveItem =
            carriedObject.GetComponentInParent<ObjectiveItem>();

        // tells the game manager that the keys were found
        if (objectiveItem != null &&
            GameManager.Instance != null &&
            GameManager.Instance.CurrentState == GameManager.GameState.FindKeys)
        {
            GameManager.Instance.KeysCollected();
        }
    }

    private void OnGrabReleased(InputAction.CallbackContext context)
    {
        if (carriedObject == null)
        {
            return;
        }

        // remembers whether the released object is the objective item
        ObjectiveItem objectiveItem =
            carriedObject.GetComponentInParent<ObjectiveItem>();

        // remembers the released object
        CarryableObject releasedObject = carriedObject;

        // restores the object and its physics
        ReleaseCarriedObject();

        // checks the final position after the object has had time to fall
        if (objectiveItem != null &&
            GameManager.Instance != null)
        {
            StartCoroutine(
                CheckKeyPlacementAfterRelease(
                    releasedObject
                )
            );
        }
    }

    private IEnumerator CheckKeyPlacementAfterRelease(
        CarryableObject releasedObject)
    {
        // waits for physics to move the keys onto the table
        yield return new WaitForSeconds(
            placementCheckDelay
        );

        // stops if the object no longer exists
        if (releasedObject == null)
        {
            yield break;
        }

        // checks the actual position where the keys landed
        Vector3 finalPosition =
            releasedObject.transform.position;

        // checks whether the keys landed on the table
        if (GameManager.Instance != null &&
            GameManager.Instance.CanPlaceKeysAtPosition(
                finalPosition
            ))
        {
            GameManager.Instance.KeysPlacedOnTable();
        }
    }

    private void ReleaseCarriedObject()
    {
        if (carriedObject == null)
        {
            return;
        }

        // restores all object colliders
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
        if (carriedObject == null || carryPoint == null)
        {
            return;
        }

        // keeps the object attached to the dogs carry point
        carriedObject.transform.SetPositionAndRotation(
            carryPoint.position,
            carryPoint.rotation
        );
    }

    public bool IsCarryingObjectiveItem()
    {
        if (carriedObject == null)
        {
            return false;
        }

        ObjectiveItem objectiveItem =
            carriedObject.GetComponentInParent<ObjectiveItem>();

        return objectiveItem != null;
    }

    public void PlaceCarriedObjectAt(Transform placementPoint)
    {
        if (carriedObject == null)
        {
            return;
        }

        // optionally moves the object to a specified position
        if (placementPoint != null)
        {
            carriedObject.transform.SetPositionAndRotation(
                placementPoint.position,
                placementPoint.rotation
            );
        }

        // restores all object colliders
        if (carriedColliders != null)
        {
            foreach (Collider collider in carriedColliders)
            {
                if (collider != null)
                {
                    collider.enabled = true;
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
}