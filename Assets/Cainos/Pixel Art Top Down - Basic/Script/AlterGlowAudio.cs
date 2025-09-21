using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Plays an audio clip when the altar glows (awakened)
using Cainos.PixelArtTopDown_Basic;

public class AlterGlowAudio : MonoBehaviour
{
    public AudioClip glowClip; // Assign your glow sound in the inspector
    public int altarId = 0; // Set to match the altar's ID

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        AltarManager.OnAltarAwakened += OnAltarAwakened;
    }

    void OnAltarAwakened(int id)
    {
        if (id == altarId && glowClip != null)
        {
            audioSource.PlayOneShot(glowClip);
        }
    }

    void OnDestroy()
    {
        AltarManager.OnAltarAwakened -= OnAltarAwakened;
    }
}
