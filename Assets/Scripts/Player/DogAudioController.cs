
using System.Collections;
using UnityEngine;

/// <summary>
/// controls walking, panting, sniffing, and objective-completion audio
/// </summary>
public class DogAudioController : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource walkingSource;
    [SerializeField] private AudioSource pantingSource;
    [SerializeField] private AudioSource sniffingSource;
    [SerializeField] private AudioSource completionSource;

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

    private Coroutine pantingCoroutine;
    private bool wasMoving;

    private void Awake()
    {
        ConfigureAudioSource(walkingSource, walkingClip, walkingVolume, true);
        ConfigureAudioSource(pantingSource, pantingClip, pantingVolume, true);
        ConfigureAudioSource(sniffingSource, sniffingClip, sniffingVolume, true);
        ConfigureAudioSource(completionSource, completionClip, completionVolume, false);
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
    /// plays the key-completion sound once
    /// </summary>
    public void PlayCompletionSound()
    {
        if (completionSource == null || completionClip == null)
        {
            return;
        }

        completionSource.PlayOneShot(completionClip);
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
    }
}