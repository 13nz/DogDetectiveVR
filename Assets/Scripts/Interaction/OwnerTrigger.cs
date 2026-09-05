using UnityEngine;

/// <summary>
/// detects when the player reaches the owner
/// </summary>
public class OwnerTrigger : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        GameManager.Instance.TryCompleteObjective();
    }
}