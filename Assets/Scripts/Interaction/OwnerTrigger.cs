using UnityEngine;

/// <summary>
/// detects when the player reaches the table and defines the key placement area
/// </summary>
public class OwnerTrigger : MonoBehaviour
{
    private Collider placementCollider;

    private void Awake()
    {
        // gets the collider attached to the table
        placementCollider = GetComponent<Collider>();
    }

    /// <summary>
    /// checks whether a position is inside the table placement area
    /// </summary>
    public bool IsPositionInsideTable(Vector3 position)
    {
        // stops if no collider exists
        if (placementCollider == null)
        {
            return false;
        }

        // checks whether the position is inside the placement collider
        return placementCollider.bounds.Contains(position);
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