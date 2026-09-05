using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// controls subtitle messages shown to the player
/// </summary>
public class SubtitleManager : MonoBehaviour
{
    public static SubtitleManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI subtitleText;

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
    /// displays a subtitle for a short period of time
    /// </summary>
    public IEnumerator ShowSubtitle(string text, float duration)
    {
        subtitleText.text = text;

        yield return new WaitForSeconds(duration);

        subtitleText.text = "";
    }

    public void ClearSubtitle()
    {
        subtitleText.text = "";
    }
}