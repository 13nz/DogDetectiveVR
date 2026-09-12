using UnityEngine;

/// <summary>
/// detects when the player reaches the table and defines the key placement area
/// </summary>
public class OwnerTrigger : MonoBehaviour
{
    private Collider placementCollider;

    private void Awake()
    {
        // gets the placement collider
        placementCollider = GetComponent<Collider>();
    }

    /// <summary>
    /// checks whether a position is over the table placement area
    /// </summary>
    public bool IsPositionInsideTable(Vector3 position)
    {
        // stops if no collider exists
        if (placementCollider == null)
        {
            return false;
        }

        // gets the collider bounds
        Bounds bounds = placementCollider.bounds;

        // ignores the height of the keys
        Vector3 positionOnTablePlane = new Vector3(
            position.x,
            bounds.center.y,
            position.z
        );

        // checks the horizontal position of the keys
        return bounds.Contains(positionOnTablePlane);
    }

    private void OnTriggerEnter(Collider other)
    {
        // only responds to the player
        if (!other.CompareTag("Player"))
        {
            return;
        }

        // tells the game manager that the player reached the table
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerReachedTable(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // only responds to the player
        if (!other.CompareTag("Player"))
        {
            return;
        }

        // tells the game manager that the player left the table
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerLeftTable(this);
        }
    }
}