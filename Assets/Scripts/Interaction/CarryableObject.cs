
using UnityEngine;

/// <summary>
/// marks an object as something the dog can carry.
/// </summary>
public class CarryableObject : MonoBehaviour
{
    [Header("Grab Settings")]
    [SerializeField] private float grabRange = 0.8f;

    /// <summary>
    /// returns the maximum distance from which this object can be grabbed.
    /// </summary>
    public float GrabRange => grabRange;
}