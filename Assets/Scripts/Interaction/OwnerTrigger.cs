using UnityEngine;

/// <summary>
/// detects when the objective item enters the table placement area
/// </summary>
public class OwnerTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // finds an objective item entering the placement area
        ObjectiveItem objectiveItem =
            other.GetComponentInParent<ObjectiveItem>();

        // stops if the object is not the keys
        if (objectiveItem == null)
        {
            return;
        }

        // tells the game manager that the keys entered the placement area
        if (GameManager.Instance != null)
        {
            GameManager.Instance.KeysEnteredPlacementZone();
        }
    }
}