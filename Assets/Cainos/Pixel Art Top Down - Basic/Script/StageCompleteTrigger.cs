using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cainos.PixelArtTopDown_Basic
{
    //when player enters the trigger, load the transition scene
    //used for stage completion triggers like opened doors
    
    public class StageCompleteTrigger : MonoBehaviour
    {
        [Header("Completion Type")]
        public bool showCongratulationsOverlay = false;  // If true, shows overlay instead of loading next scene
        
        [Header("Scene Settings")]
        public string transitionSceneName = "TransitionScene";  // Name of the scene to load
        
        [Header("Congratulations Overlay")]
        public GameObject congratulationsCanvas;  // Canvas with congratulations UI
        
        [Header("Player Detection")]
        public string playerTag = "Player";  // Tag to identify the player
        
        [Header("Optional Settings")]
        public bool playSound = true;  // Whether to play a sound effect
        public AudioClip completionSound;  // Sound to play when triggered
        
        private bool hasTriggered = false;  // Prevent multiple triggers
        private AudioSource audioSource;
        
        void Start()
        {
            // Get or add AudioSource component if we want to play sounds
            if (playSound)
            {
                audioSource = GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                }
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!hasTriggered && other.CompareTag(playerTag))
            {
                hasTriggered = true;
                
                // Stop the timer (try both approaches)
                StageTimer.StopCurrentTimer();
                
                if (showCongratulationsOverlay)
                {
                    // Hide timer UI for congratulations overlay (stage completion)
                    StageTimer.HideCurrentTimerUI();
                    StartCoroutine(ShowCongratulationsOverlay());
                }
                else
                {
                    StartCoroutine(LoadTransitionScene());
                }
            }
        }
        
        private IEnumerator LoadTransitionScene()
        {
            // Play completion sound if enabled
            if (playSound && completionSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(completionSound);
                // Wait for sound to finish playing
                yield return new WaitForSeconds(completionSound.length);
            }
            
            // Stop background music before loading new scene
            if (BackGroundMusicAudio.Instance != null)
            {
                BackGroundMusicAudio.Instance.StopAndDestroy();
            }
            
            // Load the transition scene
            SceneManager.LoadScene(transitionSceneName);
        }
        
        private IEnumerator ShowCongratulationsOverlay()
        {
            // Disable player movement immediately
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            if (player != null)
            {
                TopDownCharacterController playerController = player.GetComponent<TopDownCharacterController>();
                if (playerController != null)
                {
                    playerController.enabled = false; // Disable the controller
                }
                
                // Stop player movement
                Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    playerRb.velocity = Vector2.zero;
                }
                
                // Stop player animation
                Animator playerAnimator = player.GetComponent<Animator>();
                if (playerAnimator != null)
                {
                    playerAnimator.SetBool("IsMoving", false);
                }
            }
            
            // Stop background music
            if (BackGroundMusicAudio.Instance != null)
            {
                BackGroundMusicAudio.Instance.StopAndDestroy();
            }
            
            // Show congratulations overlay immediately
            if (congratulationsCanvas != null)
            {
                congratulationsCanvas.SetActive(true);
                
                // Get the completion time and pass it to the congratulations display
                float completionTime = 0f;
                if (StageTimer.Instance != null)
                {
                    completionTime = StageTimer.Instance.GetCurrentTime();
                }
                
                // Find and update the congratulations display
                CongratulationsDisplay congratsDisplay = congratulationsCanvas.GetComponent<CongratulationsDisplay>();
                if (congratsDisplay != null)
                {
                    congratsDisplay.SetCompletionTime(completionTime);
                    congratsDisplay.UpdateScoreDisplay();
                }
                
                Debug.Log($"Congratulations overlay displayed with time: {completionTime}!");
            }
            else
            {
                Debug.LogWarning("StageCompleteTrigger: Congratulations Canvas is not assigned!");
            }
            
            // Play completion sound after showing the overlay
            if (playSound && completionSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(completionSound);
                // Wait for sound to finish playing
                yield return new WaitForSeconds(completionSound.length);
            }
        }
    }
}
