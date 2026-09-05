using UnityEngine;

/// <summary>
/// marks an object as sniffable
/// each scent source automatically creates the correct particle effect.
/// </summary>
public class ScentSource : MonoBehaviour
{
    public enum ScentCategory
    {
        Human,
        Dog,
        Food,
        Flower,
        Plant,
        Animal,
        BadSmell
    }
    
    [Header("Detection")]
    [SerializeField] private float scentRange = 5f;

    /// returns the maximum distance at which this scent can be detected.
    public float ScentRange => scentRange;

    [Header("Scent")]
    [SerializeField] private ScentCategory scentCategory = ScentCategory.Human;

    private ParticleSystem scentParticles;

    private void Start()    {
        if (ScentLibrary.Instance == null)
        {
            Debug.LogError("ScentLibrary not found in the scene.");
            return;
        }

        GameObject prefab = ScentLibrary.Instance.GetScentPrefab(scentCategory);
        if (prefab == null)
            return;

        GameObject particles = Instantiate(prefab, transform);

        particles.transform.localPosition = Vector3.zero;
        particles.transform.localRotation = Quaternion.identity;

        scentParticles = particles.GetComponent<ParticleSystem>();

        if (scentParticles != null)
        {
            scentParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    /// <summary>
    /// shows or hides this object's scent.
    /// </summary>
    public void SetSniffVisible(bool visible)
    {
        if (scentParticles == null)
            return;

        if (visible)
        {
            if (!scentParticles.isPlaying)
                scentParticles.Play();
        }
        else
        {
            scentParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}