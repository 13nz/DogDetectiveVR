using TMPro;
using UnityEngine;

/// <summary>
/// controls the objective text shown to the player
/// </summary>
public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI objectiveText;

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

    /// <summary>
    /// updates the current objective
    /// </summary>
    public void SetObjective(string objective)
    {
        objectiveText.text = $"objective\n\n{objective}";
    }

    /// <summary>
    /// shows the objective ui
    /// </summary>
    public void ShowObjective()
    {
        objectiveText.enabled = true;
    }

    /// <summary>
    /// hides the objective ui
    /// </summary>
    public void HideObjective()
    {
        objectiveText.enabled = false;
    }
}