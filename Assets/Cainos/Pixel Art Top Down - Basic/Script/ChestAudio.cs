using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Plays two consecutive sounds when the chest opens: open sound, then glow sound
public class ChestAudio : MonoBehaviour
{
    public static bool IsChestOpening = false; // Track if chest is opening
    private static bool isWaitingForKeyConfirmation = false;
    
    public AudioClip openClip; // Assign chest open sound
    public AudioClip glowClip; // Assign glow sound
    public KeyOverlay keyOverlay; // Assign in Inspector
    
    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float openSoundVolume = 1f; // Volume for chest open sound
    [Range(0f, 1f)]
    public float glowSoundVolume = 0.5f; // Volume for glow sound (lowered default)

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    // Call this method when the chest opens
    public void PlayChestOpenSequence()
    {
        StartCoroutine(PlayOpenThenGlow());
    }

    private System.Collections.IEnumerator PlayOpenThenGlow()
    {
        IsChestOpening = true; // Disable player movement
        
        // Lower background music volume
        var bgm = BackGroundMusicAudio.Instance;
        float reducedVolume = 0.3f; // Set to desired lower volume
        if (bgm != null) bgm.SetVolume(reducedVolume);

        if (openClip != null)
        {
            audioSource.PlayOneShot(openClip, openSoundVolume);
            yield return new WaitForSeconds(openClip.length);
        }

        if (glowClip != null)
        {
            // Show key overlay when glow sound starts
            if (keyOverlay != null) keyOverlay.ShowOverlayWaitForInput();
            isWaitingForKeyConfirmation = true;
            
            audioSource.PlayOneShot(glowClip, glowSoundVolume);
            yield return new WaitForSeconds(glowClip.length);
        }

        // Restore background music volume
        if (bgm != null) bgm.RestoreOriginalVolume();
        
        // Player movement will be re-enabled when key is confirmed
    }

    // Called when player confirms key acquisition with F
    public static void OnKeyObtained()
    {
        isWaitingForKeyConfirmation = false;
        IsChestOpening = false; // Re-enable player movement
    }

    // Check if we're waiting for key confirmation
    public static bool IsWaitingForKeyConfirmation()
    {
        return isWaitingForKeyConfirmation;
    }
}
