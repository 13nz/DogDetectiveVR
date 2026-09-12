using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

/// <summary>
/// controls the gameplay flow voice lines subtitles and objectives
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
    [SerializeField] private SubtitleManager subtitleManager;

    [Header("Voice Line Settings")]
    [SerializeField] private float voiceLineVolume = 1f;

    [Header("Ending Scene")]
    [SerializeField] private string endingSceneName = "Ending";
    [SerializeField] private float endingDelay = 3f;

    [Header("Random Voice Settings")]
    [SerializeField] private float minimumRandomDelay = 12f;
    [SerializeField] private float maximumRandomDelay = 25f;
    [SerializeField] private float introDelay = 2f;

    public GameState CurrentState { get; private set; }

    private AudioSource voiceLineSource;

    private AudioClip intro1;
    private AudioClip intro2;
    private AudioClip intro3;

    private AudioClip keysFound1;
    private AudioClip keysFound2;

    private AudioClip nearTable;
    private AudioClip objectiveComplete;

    private AudioClip random1;
    private AudioClip random2;
    private AudioClip random3;
    private AudioClip random4;
    private AudioClip random5;

    [Header("Audio")]
    [SerializeField] private AudioMixerGroup voiceMixerGroup;

    private Coroutine randomVoiceCoroutine;

    private bool keyFoundSequencePlaying;
    private bool endingSequencePlaying;

    private bool waitingForKeyPlacement;
    private bool nearTableSequencePlaying;

    private OwnerTrigger currentTableTrigger;

    private void Awake()
    {
        // keeps only one game manager active
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // finds the dog carry controller automatically
        if (dogCarryController == null)
        {
            dogCarryController =
                FindFirstObjectByType<DogCarryController>();
        }

        // finds the dog audio controller automatically
        if (dogAudioController == null)
        {
            dogAudioController =
                FindFirstObjectByType<DogAudioController>();
        }

        // finds the subtitle manager automatically
        if (subtitleManager == null)
        {
            subtitleManager =
                FindFirstObjectByType<SubtitleManager>();
        }

        // creates a dedicated audio source for owner voice lines
        voiceLineSource = GetComponent<AudioSource>();

        if (voiceLineSource == null)
        {
            voiceLineSource =
                gameObject.AddComponent<AudioSource>();
        }

        // configures the voice line audio source
        voiceLineSource.outputAudioMixerGroup = voiceMixerGroup;
        voiceLineSource.playOnAwake = false;
        voiceLineSource.loop = false;
        voiceLineSource.volume = voiceLineVolume;
        voiceLineSource.spatialBlend = 0f;

        // loads all voice lines
        LoadVoiceLines();
    }

    private IEnumerator Start()
    {
        // starts the game with the introduction
        CurrentState = GameState.Intro;

        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.HideObjective();
        }

        if (subtitleManager != null)
        {
            subtitleManager.ClearSubtitle();
        }

        // gives the player time to get ready
        yield return new WaitForSeconds(introDelay);

        // plays the first introduction voice line
        yield return StartCoroutine(
            PlayVoiceLine(
                intro1,
                "I cannot find my keys anywhere"
            )
        );

        // plays the second introduction voice line
        yield return StartCoroutine(
            PlayVoiceLine(
                intro2,
                "Buddy can you help me find them"
            )
        );

        // plays the third introduction voice line
        yield return StartCoroutine(
            PlayVoiceLine(
                intro3,
                "I think I had them somewhere around the house"
            )
        );

        // changes the game into the key search state
        CurrentState = GameState.FindKeys;

        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.ShowObjective();

            ObjectiveManager.Instance.SetObjective(
                "Find your humans keys"
            );
        }

        // starts occasional owner dialogue while searching
        randomVoiceCoroutine =
            StartCoroutine(RandomVoiceRoutine());
    }

    private void LoadVoiceLines()
    {
        // loads the introduction recordings
        intro1 =
            Resources.Load<AudioClip>(
                "Audio/VoiceLines/voice_line_intro_1"
            );

        intro2 =
            Resources.Load<AudioClip>(
                "Audio/VoiceLines/voice_line_intro_2"
            );

        intro3 =
            Resources.Load<AudioClip>(
                "Audio/VoiceLines/voice_line_intro_3"
            );

        // loads the key discovery recordings
        keysFound1 =
            Resources.Load<AudioClip>(
                "Audio/VoiceLines/voice_line_keys_found_1"
            );

        keysFound2 =
            Resources.Load<AudioClip>(
                "Audio/VoiceLines/voice_line_keys_found_2"
            );

        // loads the table recording
        nearTable =
            Resources.Load<AudioClip>(
                "Audio/VoiceLines/voice_line_near_table"
            );

        // loads the completion recording
        objectiveComplete =
            Resources.Load<AudioClip>(
                "Audio/VoiceLines/voice_line_objective_complete"
            );

        // loads the random recordings
        random1 =
            Resources.Load<AudioClip>(
                "Audio/VoiceLines/voice_line_random_1"
            );

        random2 =
            Resources.Load<AudioClip>(
                "Audio/VoiceLines/voice_line_random_2"
            );

        random3 =
            Resources.Load<AudioClip>(
                "Audio/VoiceLines/voice_line_random_3"
            );

        random4 =
            Resources.Load<AudioClip>(
                "Audio/VoiceLines/voice_line_random_4"
            );

        random5 =
            Resources.Load<AudioClip>(
                "Audio/VoiceLines/voice_line_random_5"
            );

        // checks that the recordings loaded
        CheckVoiceLine(intro1, "voice_line_intro_1");
        CheckVoiceLine(intro2, "voice_line_intro_2");
        CheckVoiceLine(intro3, "voice_line_intro_3");

        CheckVoiceLine(
            keysFound1,
            "voice_line_keys_found_1"
        );

        CheckVoiceLine(
            keysFound2,
            "voice_line_keys_found_2"
        );

        CheckVoiceLine(
            nearTable,
            "voice_line_near_table"
        );

        CheckVoiceLine(
            objectiveComplete,
            "voice_line_objective_complete"
        );

        CheckVoiceLine(random1, "voice_line_random_1");
        CheckVoiceLine(random2, "voice_line_random_2");
        CheckVoiceLine(random3, "voice_line_random_3");
        CheckVoiceLine(random4, "voice_line_random_4");
        CheckVoiceLine(random5, "voice_line_random_5");
    }

    private void CheckVoiceLine(
        AudioClip clip,
        string clipName)
    {
        if (clip == null)
        {
            Debug.LogError(
                "could not load voice line " + clipName
            );
        }
    }

    /// <summary>
    /// handles the moment when the dog finds the keys
    /// </summary>
    public void KeysCollected()
    {
        // prevents the keys from being collected more than once
        if (CurrentState != GameState.FindKeys)
        {
            return;
        }

        // prevents the sequence from starting multiple times
        if (keyFoundSequencePlaying)
        {
            return;
        }

        keyFoundSequencePlaying = true;

        // stops random dialogue after the keys are found
        if (randomVoiceCoroutine != null)
        {
            StopCoroutine(randomVoiceCoroutine);
            randomVoiceCoroutine = null;
        }

        // plays the key sound when the dog picks up the keys
        if (dogAudioController != null)
        {
            dogAudioController.PlayCompletionSound();
        }

        // starts the key discovery dialogue
        StartCoroutine(KeysFoundRoutine());
    }

    private IEnumerator KeysFoundRoutine()
    {
        // changes the game into the return keys state
        CurrentState = GameState.ReturnKeys;

        // tells the player that the keys were found
        yield return StartCoroutine(
            PlayVoiceLine(
                keysFound1,
                "You found them"
            )
        );

        // tells the dog where to take the keys
        yield return StartCoroutine(
            PlayVoiceLine(
                keysFound2,
                "Good boy bring them to the kitchen table"
            )
        );

        // updates the objective
        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.SetObjective(
                "Bring the keys to the kitchen table"
            );
        }

        keyFoundSequencePlaying = false;
    }

    /// <summary>
    /// detects when the player reaches the table
    /// </summary>
    public void PlayerReachedTable(OwnerTrigger tableTrigger)
    {
        // only responds during the return keys state
        if (CurrentState != GameState.ReturnKeys)
        {
            return;
        }

        // prevents the ending from starting again
        if (endingSequencePlaying)
        {
            return;
        }

        // stores the table trigger
        currentTableTrigger = tableTrigger;

        // remembers that the player is at the table
        waitingForKeyPlacement = true;

        // tells the dog to put the keys on the table
        StartCoroutine(
            PlayVoiceLine(
                nearTable,
                "That is it buddy put them on the table"
            )
        );
    }

    /// <summary>
    /// plays the dialogue when the dog reaches the table
    /// </summary>
    private IEnumerator NearTableRoutine()
    {
        // prevents this dialogue from playing multiple times
        nearTableSequencePlaying = true;

        // plays the table instruction
        yield return StartCoroutine(
            PlayVoiceLine(
                nearTable,
                "That is it buddy put them on the table"
            )
        );

        nearTableSequencePlaying = false;
    }

    /// <summary>
    /// detects when the player leaves the table
    /// </summary>
    public void PlayerLeftTable(OwnerTrigger tableTrigger)
    {
        // only clears the current table trigger
        if (currentTableTrigger != tableTrigger)
        {
            return;
        }

        // remembers that the player left the table
        waitingForKeyPlacement = false;
        currentTableTrigger = null;
    }

    public bool IsAtKeyPlacementArea()
    {
        // checks whether the player is currently at the table
        return waitingForKeyPlacement &&
            currentTableTrigger != null;
    }
   

    /// <summary>
    /// completes the game when the dog places the keys on the table
    /// </summary>
    public void KeysPlacedOnTable()
    {
        // only allows completion during the return keys state
        if (CurrentState != GameState.ReturnKeys)
        {
            return;
        }

        // prevents the ending from starting more than once
        if (endingSequencePlaying)
        {
            return;
        }

        // clears the table placement state
        waitingForKeyPlacement = false;
        currentTableTrigger = null;

        // starts the final sequence
        StartCoroutine(CompleteRoutine());
    }

    public void KeysEnteredPlacementZone()
    {
        // only responds during the return keys state
        if (CurrentState != GameState.ReturnKeys)
        {
            return;
        }

        // prevents the ending from starting more than once
        if (endingSequencePlaying)
        {
            return;
        }

        // starts the completion sequence
        StartCoroutine(CompleteRoutine());
    }

    /// <summary>
    /// plays the final dialogue and completes the game
    /// </summary>
    private IEnumerator CompleteRoutine()
    {
        // prevents multiple ending sequences
        endingSequencePlaying = true;

        // changes the game state to complete
        CurrentState = GameState.Complete;

        // stops random dialogue
        if (randomVoiceCoroutine != null)
        {
            StopCoroutine(randomVoiceCoroutine);
            randomVoiceCoroutine = null;
        }

        // hides the objective
        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.HideObjective();
        }

        // plays the key sound after successful placement
        if (dogAudioController != null)
        {
            dogAudioController.PlayCompletionSound();
        }

        // waits briefly before the final voice line
        yield return new WaitForSeconds(0.2f);

        // checks that the completion voice line exists
        if (objectiveComplete == null)
        {
            Debug.LogError(
                "the objective complete voice line is missing"
            );

            endingSequencePlaying = false;
            yield break;
        }

        // plays the final voice line
        yield return StartCoroutine(
            PlayVoiceLine(
                objectiveComplete,
                "Perfect thank you buddy"
            )
        );

        // clears the final subtitle
        if (subtitleManager != null)
        {
            subtitleManager.ClearSubtitle();
        }

        // waits before loading the ending scene
        yield return new WaitForSeconds(endingDelay);

        // loads the ending scene
        SceneManager.LoadScene(endingSceneName);
    }

    /// <summary>
    /// occasionally plays owner dialogue while searching
    /// </summary>
    private IEnumerator RandomVoiceRoutine()
    {
        // continues until the keys are found
        while (CurrentState == GameState.FindKeys)
        {
            // waits before playing another random line
            float delay =
                Random.Range(
                    minimumRandomDelay,
                    maximumRandomDelay
                );

            yield return new WaitForSeconds(delay);

            // stops if the game has changed state
            if (CurrentState != GameState.FindKeys)
            {
                yield break;
            }

            // selects a random voice line
            AudioClip randomClip =
                GetRandomVoiceLine();

            // gets the matching subtitle
            string subtitle =
                GetRandomSubtitle(randomClip);

            // plays the random voice line
            yield return StartCoroutine(
                PlayVoiceLine(
                    randomClip,
                    subtitle
                )
            );
        }
    }

    /// <summary>
    /// selects one random owner voice line
    /// </summary>
    private AudioClip GetRandomVoiceLine()
    {
        // stores all random recordings
        AudioClip[] clips =
        {
            random1,
            random2,
            random3,
            random4,
            random5
        };

        // selects one recording at random
        return clips[
            Random.Range(
                0,
                clips.Length
            )
        ];
    }

    /// <summary>
    /// returns the subtitle matching a random voice line
    /// </summary>
    private string GetRandomSubtitle(AudioClip clip)
    {
        if (clip == random1)
        {
            return "Where could those keys have gone";
        }

        if (clip == random2)
        {
            return "Maybe they are in the living room";
        }

        if (clip == random3)
        {
            return "Keep looking buddy";
        }

        if (clip == random4)
        {
            return "I know they are around here somewhere";
        }

        if (clip == random5)
        {
            return "Have you found them yet";
        }

        return "";
    }

    /// <summary>
    /// plays a voice line and matching subtitle
    /// </summary>
    private IEnumerator PlayVoiceLine(
        AudioClip clip,
        string subtitle)
    {
        if (clip == null)
        {
            yield break;
        }

        // stops the previous voice line
        if (voiceLineSource != null)
        {
            voiceLineSource.Stop();
            voiceLineSource.clip = clip;
            voiceLineSource.Play();
        }

        // starts the subtitle at the same time
        if (subtitleManager != null)
        {
            StartCoroutine(
                subtitleManager.ShowSubtitle(
                    subtitle,
                    clip.length
                )
            );
        }

        // waits for the voice line to finish
        yield return new WaitForSeconds(clip.length);
    }
}