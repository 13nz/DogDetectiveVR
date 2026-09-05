using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// controls the dog's sniff ability
/// while the player holds the right trigger, nearby scent sources become visible.
/// </summary>
public class DogSniffController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference sniffAction;

    // stores every scent source in the scene
    private ScentSource[] scentSources;

    // tracks whether the player is currently sniffing
    private bool isSniffing;

    private void Awake()
    {
        scentSources = FindObjectsByType<ScentSource>(FindObjectsSortMode.None);
    }

    // state based sniffing
    private void Update()
    {
        if (!isSniffing)
            return;

        UpdateVisibleScents();
    }

    /// <summary>
    /// updates which scent sources should currently be visible.
    /// </summary>
    private void UpdateVisibleScents()
    {
        foreach (ScentSource source in scentSources)
        {
            float distance = Vector3.Distance(
                transform.position,
                source.transform.position);

            bool canSmell = distance <= source.ScentRange;

            source.SetSniffVisible(canSmell);
        }
    }

    private void OnEnable()
    {
        sniffAction.action.Enable();

        sniffAction.action.performed += OnSniffStarted;
        sniffAction.action.canceled += OnSniffEnded;
    }

    private void OnDisable()
    {
        sniffAction.action.performed -= OnSniffStarted;
        sniffAction.action.canceled -= OnSniffEnded;

        sniffAction.action.Disable();
    }


    private void OnSniffStarted(InputAction.CallbackContext context)
    {
        isSniffing = true;
    }

    /// <summary>
    /// hides every scent source when sniffing ends.
    /// </summary>
    private void OnSniffEnded(InputAction.CallbackContext context)
    {
        isSniffing = false;

        foreach (ScentSource source in scentSources)
        {
            source.SetSniffVisible(false);
        }
}
}