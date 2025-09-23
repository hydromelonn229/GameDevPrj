using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Plays alternating walking sounds when the player moves, using Rigidbody2D for movement detection
[RequireComponent(typeof(AudioSource))]
public class WalkAudio : MonoBehaviour
{
    public AudioClip[] walkClips; // Assign your 2 wav files in the inspector
    public float minVelocity = 0.1f; // Minimum velocity to trigger walking sound

    private AudioSource audioSource;
    private Rigidbody2D rb;
    private int currentClip = 0;
    private bool isWalking = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        audioSource.loop = false; // We'll handle looping manually
    }

    void Update()
    {
        bool moving = rb.velocity.magnitude > minVelocity;
        if (moving && !isWalking)
        {
            isWalking = true;
            PlayNextClip();
        }
        else if (!moving && isWalking)
        {
            isWalking = false;
            audioSource.Stop();
            CancelInvoke();
        }
    }

    void PlayNextClip()
    {
        if (!isWalking || walkClips.Length == 0) return;
        audioSource.clip = walkClips[currentClip];
        audioSource.Play();
        currentClip = (currentClip + 1) % walkClips.Length;
        Invoke(nameof(PlayNextClip), audioSource.clip.length);
    }

    void OnDisable()
    {
        audioSource.Stop();
        CancelInvoke();
    }
}
