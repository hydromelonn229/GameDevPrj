using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAudio : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip doorOpenSound;     // Sound when door opens
    
    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float volume = 1f;

    void Start()
    {
        // No need to initialize AudioSource anymore
    }
    
    // Call this method when the door opens
    public void PlayDoorOpenSound()
    {
        if (doorOpenSound != null)
        {
            Debug.Log($"Playing door sound: {doorOpenSound.name} at volume {volume}");
            AudioSource.PlayClipAtPoint(doorOpenSound, transform.position, volume);
        }
        else
        {
            Debug.LogWarning("Door open sound is null! Please assign an audio clip to the 'Door Open Sound' field in the DoorAudio component.");
        }
    }
    
    // Alternative method (same as above now)
    public void PlayDoorOpenSoundOneShot()
    {
        PlayDoorOpenSound();
    }
}
