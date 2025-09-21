using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundMusicAudio : MonoBehaviour
{
    public static BackGroundMusicAudio Instance { get; private set; }
    public float volume = 1.0f; // Exposed in Inspector
    private AudioSource audioSource;
    private float originalVolume;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = volume;
        originalVolume = volume;
    }

    void OnValidate()
    {
        // Update volume in editor when changed
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (audioSource != null) audioSource.volume = volume;
        originalVolume = volume;
    }

    // Set background music volume (0.0 to 1.0)
    public void SetVolume(float v)
    {
        if (audioSource != null)
            audioSource.volume = v;
    }

    // Restore to original volume
    public void RestoreOriginalVolume()
    {
        if (audioSource != null)
            audioSource.volume = originalVolume;
    }

    // Optionally, expose the AudioSource for more control
    public AudioSource GetAudioSource() => audioSource;
    
    // Stop music and destroy this instance (useful for scene transitions)
    public void StopAndDestroy()
    {
        if (audioSource != null)
            audioSource.Stop();
        Instance = null;
        Destroy(gameObject);
    }
}
