using System.Collections;
using UnityEngine;

/// <summary>
/// controls walking panting sniffing key and voice line audio
/// </summary>
public class DogAudioController : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource walkingSource;
    [SerializeField] private AudioSource pantingSource;
    [SerializeField] private AudioSource sniffingSource;
    [SerializeField] private AudioSource completionSource;
    [SerializeField] private AudioSource voiceLineSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip walkingClip;
    [SerializeField] private AudioClip pantingClip;
    [SerializeField] private AudioClip sniffingClip;
    [SerializeField] private AudioClip completionClip;

    [Header("Settings")]
    [SerializeField] private float pantingDuration = 5f;
    [SerializeField] private float walkingVolume = 0.35f;
    [SerializeField] private float pantingVolume = 0.6f;
    [SerializeField] private float sniffingVolume = 0.7f;
    [SerializeField] private float completionVolume = 0.8f;
    [SerializeField] private float voiceLineVolume = 1f;

    private Coroutine pantingCoroutine;
    private bool wasMoving;

    private void Awake()
    {
        // automatically finds the completion audio source if one was not assigned
        if (completionSource == null)
        {
            completionSource = GetComponent<AudioSource>();
        }

        // loads the key sound automatically if the clip was not assigned
        if (completionClip == null)
        {
            completionClip =
                Resources.Load<AudioClip>("Audio/Sounds/keys_found");
        }

        ConfigureAudioSource(
            walkingSource,
            walkingClip,
            walkingVolume,
            true
        );

        ConfigureAudioSource(
            pantingSource,
            pantingClip,
            pantingVolume,
            true
        );

        ConfigureAudioSource(
            sniffingSource,
            sniffingClip,
            sniffingVolume,
            true
        );

        ConfigureAudioSource(
            completionSource,
            completionClip,
            completionVolume,
            false
        );

        if (voiceLineSource != null)
        {
            voiceLineSource.playOnAwake = false;
            voiceLineSource.loop = false;
            voiceLineSource.spatialBlend = 0f;
            voiceLineSource.volume = voiceLineVolume;
        }

        // reports a useful error if the key sound could not be found
        if (completionSource == null)
        {
            Debug.LogError(
                "no completion audio source was found on the dog audio controller"
            );
        }

        if (completionClip == null)
        {
            Debug.LogError(
                "the keys found audio clip could not be found"
            );
        }
    }

    /// <summary>
    /// configures an audio source safely
    /// </summary>
    private void ConfigureAudioSource(
        AudioSource source,
        AudioClip clip,
        float volume,
        bool looping)
    {
        if (source == null)
        {
            return;
        }

        source.clip = clip;
        source.volume = volume;
        source.loop = looping;
        source.playOnAwake = false;
        source.spatialBlend = 0f;
    }

    /// <summary>
    /// starts or stops the walking sound
    /// </summary>
    public void SetWalking(bool isWalking)
    {
        if (walkingSource == null || walkingClip == null)
        {
            return;
        }

        if (isWalking && !walkingSource.isPlaying)
        {
            walkingSource.Play();
        }
        else if (!isWalking && walkingSource.isPlaying)
        {
            walkingSource.Stop();
        }

        wasMoving = isWalking;
    }

    /// <summary>
    /// starts panting after the player stops moving
    /// </summary>
    public void SetStoppedAfterWalking()
    {
        if (!wasMoving)
        {
            return;
        }

        wasMoving = false;

        if (pantingCoroutine != null)
        {
            StopCoroutine(pantingCoroutine);
        }

        pantingCoroutine = StartCoroutine(PantingRoutine());
    }

    /// <summary>
    /// starts or stops the sniffing sound
    /// </summary>
    public void SetSniffing(bool isSniffing)
    {
        if (sniffingSource == null || sniffingClip == null)
        {
            return;
        }

        if (isSniffing && !sniffingSource.isPlaying)
        {
            sniffingSource.Play();
        }
        else if (!isSniffing && sniffingSource.isPlaying)
        {
            sniffingSource.Stop();
        }
    }

    /// <summary>
    /// handles the sprinting state and panting after sprinting
    /// </summary>
    public void SetSprinting(bool isSprinting)
    {
        if (isSprinting)
        {
            if (pantingCoroutine != null)
            {
                StopCoroutine(pantingCoroutine);
                pantingCoroutine = null;
            }

            if (pantingSource != null)
            {
                pantingSource.Stop();
            }
        }
        else
        {
            if (pantingCoroutine != null)
            {
                StopCoroutine(pantingCoroutine);
            }

            pantingCoroutine = StartCoroutine(PantingRoutine());
        }
    }

    /// <summary>
    /// plays the keys sound
    /// </summary>
    public void PlayCompletionSound()
    {
        if (completionSource == null)
        {
            Debug.LogError(
                "cannot play the keys sound because the completion audio source is missing"
            );

            return;
        }

        if (completionClip == null)
        {
            Debug.LogError(
                "cannot play the keys sound because the completion audio clip is missing"
            );

            return;
        }

        // plays the keys sound without changing the configured source clip
        completionSource.PlayOneShot(
            completionClip,
            completionVolume
        );
    }

    /// <summary>
    /// plays a voice line and waits for it to finish
    /// </summary>
    public IEnumerator PlayVoiceLine(AudioClip clip)
    {
        if (voiceLineSource == null || clip == null)
        {
            yield break;
        }

        voiceLineSource.Stop();
        voiceLineSource.clip = clip;
        voiceLineSource.Play();

        yield return new WaitForSeconds(clip.length);
    }

    /// <summary>
    /// plays panting for the configured duration
    /// </summary>
    private IEnumerator PantingRoutine()
    {
        if (pantingSource == null || pantingClip == null)
        {
            yield break;
        }

        pantingSource.Play();

        yield return new WaitForSeconds(pantingDuration);

        pantingSource.Stop();
        pantingCoroutine = null;
    }

    /// <summary>
    /// stops all dog audio
    /// </summary>
    public void StopAllAudio()
    {
        if (pantingCoroutine != null)
        {
            StopCoroutine(pantingCoroutine);
            pantingCoroutine = null;
        }

        if (walkingSource != null)
        {
            walkingSource.Stop();
        }

        if (pantingSource != null)
        {
            pantingSource.Stop();
        }

        if (sniffingSource != null)
        {
            sniffingSource.Stop();
        }

        if (completionSource != null)
        {
            completionSource.Stop();
        }

        if (voiceLineSource != null)
        {
            voiceLineSource.Stop();
        }
    }
}