using System.Collections;
using UnityEngine;

/// <summary>
/// controls the gameplay flow
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        Intro,
        FindKeys,
        ReturnKeys,
        Complete
    }

    [Header("References")]
    [SerializeField] private DogCarryController dogCarryController;


    [SerializeField] private DogAudioController dogAudioController;

    public GameState CurrentState { get; private set; }


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

    private IEnumerator Start()
    {
        CurrentState = GameState.Intro;

        ObjectiveManager.Instance.HideObjective();

        // avoid missing subtitles
        SubtitleManager.Instance.ClearSubtitle();
        ObjectiveManager.Instance.HideObjective();

        yield return StartCoroutine(
            SubtitleManager.Instance.ShowSubtitle(
                "I can't find my keys...",
                3f));

        yield return StartCoroutine(
            SubtitleManager.Instance.ShowSubtitle(
                "Could you help me find them buddy?",
                3f));

        ObjectiveManager.Instance.ShowObjective();
        ObjectiveManager.Instance.SetObjective(
            "Find your human's keys");

        CurrentState = GameState.FindKeys;
    }

    /// <summary>
    /// called when the player picks up the keys.
    /// </summary>
    public void KeysCollected()
    {
        if (CurrentState != GameState.FindKeys)
            return;

        CurrentState = GameState.ReturnKeys;

        ObjectiveManager.Instance.SetObjective(
            "Bring the keys to your human");

        if (dogAudioController != null)
        {
            dogAudioController.PlayCompletionSound();
        }
    }

    /// <summary>
    /// checks whether the objective can be completed
    /// </summary>
    public void TryCompleteObjective()
    {

        if (dogCarryController == null)
        {
            return;
        }


        if (CurrentState != GameState.ReturnKeys)
        {
            return;
        }

        bool carrying = dogCarryController.IsCarryingObjectiveItem();


        if (!carrying)
        {
            return;
        }


        StartCoroutine(CompleteRoutine());
    }

    /// <summary>
    /// plays the ending sequence
    /// </summary>
    private IEnumerator CompleteRoutine()
    {
        CurrentState = GameState.Complete;

        ObjectiveManager.Instance.HideObjective();

        CarryableObject carriedObject = dogCarryController.CarriedObject;

        if (carriedObject != null)
        {
            carriedObject.gameObject.SetActive(false);
        }

        yield return StartCoroutine(
            SubtitleManager.Instance.ShowSubtitle(
                "There they are! Good boy!",
                3f));

        yield return StartCoroutine(
            SubtitleManager.Instance.ShowSubtitle(
                "All done!",
                2f));
    }
}