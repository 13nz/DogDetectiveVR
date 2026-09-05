using UnityEngine;

/// <summary>
/// stores references to all scent particle prefabs
/// scent sources request the correct prefab based on their category
/// </summary>
public class ScentLibrary : MonoBehaviour
{
    public static ScentLibrary Instance { get; private set; }

    [Header("Scent Prefabs")]
    [SerializeField] private GameObject humanScentPrefab;
    [SerializeField] private GameObject dogScentPrefab;
    [SerializeField] private GameObject foodScentPrefab;
    [SerializeField] private GameObject flowerScentPrefab;
    [SerializeField] private GameObject plantScentPrefab;
    [SerializeField] private GameObject animalScentPrefab;
    [SerializeField] private GameObject badSmellPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public GameObject GetScentPrefab(ScentSource.ScentCategory category)
    {
        switch (category)
        {
            case ScentSource.ScentCategory.Human:
                return humanScentPrefab;

            case ScentSource.ScentCategory.Dog:
                return dogScentPrefab;

            case ScentSource.ScentCategory.Food:
                return foodScentPrefab;

            case ScentSource.ScentCategory.Flower:
                return flowerScentPrefab;

            case ScentSource.ScentCategory.Plant:
                return plantScentPrefab;

            case ScentSource.ScentCategory.Animal:
                return animalScentPrefab;

            case ScentSource.ScentCategory.BadSmell:
                return badSmellPrefab;

            default:
                return null;
        }
    }
}