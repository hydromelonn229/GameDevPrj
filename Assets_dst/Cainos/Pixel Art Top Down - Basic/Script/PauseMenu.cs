using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cainos.PixelArtTopDown_Basic
{
    public class PauseMenu : MonoBehaviour
    {
        public GameObject pauseMenuUI;
        
        // Audio settings for button clicks
        public AudioSource buttonAudioSource;
        public AudioClip buttonClickClip;
        
        private void Start()
        {
            // Make sure pause menu is hidden at start
            if (pauseMenuUI != null)
            {
                pauseMenuUI.SetActive(false);
            }
            
            // Ensure button audio source doesn't auto-play
            if (buttonAudioSource != null)
            {
                buttonAudioSource.playOnAwake = false;
                buttonAudioSource.Stop(); // Stop any audio that might be playing
            }
        }
        
        // Resume button functionality - resumes the current stage
        public void Resume()
        {
            PlayButtonSound();
            
            if (pauseMenuUI != null)
            {
                pauseMenuUI.SetActive(false);
            }
            Time.timeScale = 1f;
        }
        
        // Restart button functionality - restarts the current stage
        public void RestartStage()
        {
            StartCoroutine(PlaySoundAndRestartStage());
        }
        
        // Main Menu button functionality - goes to main menu
        public void GoToMainMenu()
        {
            StartCoroutine(PlaySoundAndGoToMainMenu());
        }
        
        // Coroutine to play sound and restart stage
        private IEnumerator PlaySoundAndRestartStage()
        {
            PlayButtonSound();
            
            // Wait for sound to finish - get the clip length properly
            float clipLength = 0f;
            if (buttonAudioSource != null)
            {
                if (buttonClickClip != null)
                {
                    clipLength = buttonClickClip.length;
                }
                else if (buttonAudioSource.clip != null)
                {
                    clipLength = buttonAudioSource.clip.length;
                }
            }
            
            if (clipLength > 0)
            {
                yield return new WaitForSecondsRealtime(clipLength);
            }
            
            Time.timeScale = 1f;
            // Reload the current scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
        // Coroutine to play sound and go to main menu
        private IEnumerator PlaySoundAndGoToMainMenu()
        {
            PlayButtonSound();
            
            // Wait for sound to finish - get the clip length properly
            float clipLength = 0f;
            if (buttonAudioSource != null)
            {
                if (buttonClickClip != null)
                {
                    clipLength = buttonClickClip.length;
                }
                else if (buttonAudioSource.clip != null)
                {
                    clipLength = buttonAudioSource.clip.length;
                }
            }
            
            if (clipLength > 0)
            {
                yield return new WaitForSecondsRealtime(clipLength);
            }
            
            Time.timeScale = 1f;
            
            // Stop background music before loading main menu
            if (BackGroundMusicAudio.Instance != null)
            {
                BackGroundMusicAudio.Instance.StopAndDestroy();
            }
            
            // Load the main menu scene (adjust scene name if needed)
            SceneManager.LoadScene("Main Menu");
        }
        
        // Helper method to play button click sound
        private void PlayButtonSound()
        {
            if (buttonAudioSource != null && buttonClickClip != null)
            {
                buttonAudioSource.PlayOneShot(buttonClickClip);
            }
            else if (buttonAudioSource != null && buttonAudioSource.clip != null)
            {
                // If no buttonClickClip is assigned, use the AudioSource's default clip
                buttonAudioSource.PlayOneShot(buttonAudioSource.clip);
            }
        }
        
        // Alternative method to detect current stage and resume to correct scene
        public void ResumeToCurrentStage()
        {
            Time.timeScale = 1f;
            
            string currentScene = SceneManager.GetActiveScene().name;
            
            // Check which stage we're in and resume accordingly
            if (currentScene.Contains("Stage1") || currentScene.Contains("Scene1"))
            {
                if (pauseMenuUI != null)
                {
                    pauseMenuUI.SetActive(false);
                }
            }
            else if (currentScene.Contains("Stage2") || currentScene.Contains("Scene2"))
            {
                if (pauseMenuUI != null)
                {
                    pauseMenuUI.SetActive(false);
                }
            }
            else
            {
                // Default resume behavior
                Resume();
            }
        }
        
        // Helper method to check if game is paused
        public static bool IsGamePaused()
        {
            return Time.timeScale == 0f;
        }
        
        // Alternative method for going to specific scenes
        public void LoadScene(string sceneName)
        {
            StartCoroutine(PlaySoundAndLoadScene(sceneName));
        }
        
        // Coroutine to play sound and load specific scene
        private IEnumerator PlaySoundAndLoadScene(string sceneName)
        {
            PlayButtonSound();
            
            // Wait for sound to finish - get the clip length properly
            float clipLength = 0f;
            if (buttonAudioSource != null)
            {
                if (buttonClickClip != null)
                {
                    clipLength = buttonClickClip.length;
                }
                else if (buttonAudioSource.clip != null)
                {
                    clipLength = buttonAudioSource.clip.length;
                }
            }
            
            if (clipLength > 0)
            {
                yield return new WaitForSecondsRealtime(clipLength);
            }
            
            Time.timeScale = 1f;
            
            // Stop background music when loading any scene (useful for main menu transitions)
            if (BackGroundMusicAudio.Instance != null)
            {
                BackGroundMusicAudio.Instance.StopAndDestroy();
            }
            
            SceneManager.LoadScene(sceneName);
        }
    }
}
