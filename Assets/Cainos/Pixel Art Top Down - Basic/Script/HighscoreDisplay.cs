using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Cainos.PixelArtTopDown_Basic
{
    public class HighScoreDisplay : MonoBehaviour
    {
        [Header("UI References")]
        public TextMeshProUGUI stage1ScoreText;
        public TextMeshProUGUI stage2ScoreText;
        public TextMeshProUGUI combinedScoreText; // Optional: Shows both scores in one text
        
        [Header("Settings")]
        public bool updateOnStart = true;
        public bool showMilliseconds = true;
        
        [Header("Display Format")]
        public string stage1Label = "Stage 1 Best: ";
        public string stage2Label = "Stage 2 Best: ";
        public string noScoreText = "--:--";
        
        [Header("Audio Settings")]
        public AudioSource buttonAudioSource;
        public AudioClip buttonClickClip;
        
        void Start()
        {
            if (updateOnStart)
            {
                UpdateHighScoreDisplay();
            }
        }
        
        void OnEnable()
        {
            // Update scores whenever the UI becomes active (useful for menu panels)
            UpdateHighScoreDisplay();
        }
        
        public void UpdateHighScoreDisplay()
        {
            // Get high scores from PlayerPrefs (check multiple possible keys)
            float stage1Score = PlayerPrefs.GetFloat("HighScore_Stage1", 0f);
            float stage2Score = PlayerPrefs.GetFloat("HighScore_Scene2", 0f); // Check Scene2 first
            if (stage2Score == 0f)
            {
                stage2Score = PlayerPrefs.GetFloat("HighScore_Stage2", 0f); // Fallback to Stage2
            }
            
            // Display Stage 1 high score
            if (stage1ScoreText != null)
            {
                if (stage1Score > 0)
                {
                    stage1ScoreText.text = stage1Label + FormatTime(stage1Score);
                }
                else
                {
                    stage1ScoreText.text = stage1Label + noScoreText;
                }
            }
            
            // Display Stage 2 high score
            if (stage2ScoreText != null)
            {
                if (stage2Score > 0)
                {
                    stage2ScoreText.text = stage2Label + FormatTime(stage2Score);
                }
                else
                {
                    stage2ScoreText.text = stage2Label + noScoreText;
                }
            }
            
            // Display combined scores (optional)
            if (combinedScoreText != null)
            {
                string stage1Text = stage1Score > 0 ? FormatTime(stage1Score) : noScoreText;
                string stage2Text = stage2Score > 0 ? FormatTime(stage2Score) : noScoreText;
                
                combinedScoreText.text = $"HIGH SCORES\n{stage1Label}{stage1Text}\n{stage2Label}{stage2Text}";
            }
            
            Debug.Log($"High scores updated - Stage1: {FormatTime(stage1Score)}, Stage2: {FormatTime(stage2Score)}");
        }
        
        private string FormatTime(float time)
        {
            if (time <= 0) return noScoreText;
            
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            
            if (showMilliseconds)
            {
                int milliseconds = Mathf.FloorToInt((time * 1000f) % 1000f);
                return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
            }
            else
            {
                return string.Format("{0:00}:{1:00}", minutes, seconds);
            }
        }
        
        // Method to clear all high scores (useful for testing or settings menu)
        public void ClearAllHighScores()
        {
            StartCoroutine(PlaySoundAndClearScores());
        }
        
        private System.Collections.IEnumerator PlaySoundAndClearScores()
        {
            // Play button click sound
            if (buttonAudioSource != null && buttonClickClip != null)
            {
                buttonAudioSource.PlayOneShot(buttonClickClip);
                yield return new WaitForSeconds(buttonClickClip.length);
            }
            
            // Clear the scores
            PlayerPrefs.DeleteKey("HighScore_Stage1");
            PlayerPrefs.DeleteKey("HighScore_Stage2");
            PlayerPrefs.DeleteKey("HighScore_Scene2");
            PlayerPrefs.Save();
            UpdateHighScoreDisplay();
            Debug.Log("All high scores cleared!");
        }
        
        // Method to clear all high scores without sound (for direct calls)
        public void ClearAllHighScoresNoSound()
        {
            PlayerPrefs.DeleteKey("HighScore_Stage1");
            PlayerPrefs.DeleteKey("HighScore_Stage2");
            PlayerPrefs.DeleteKey("HighScore_Scene2");
            PlayerPrefs.Save();
            UpdateHighScoreDisplay();
            Debug.Log("All high scores cleared!");
        }
        
        // Get raw score values (useful for other scripts)
        public float GetStage1Score()
        {
            return PlayerPrefs.GetFloat("HighScore_Stage1", 0f);
        }
        
        public float GetStage2Score()
        {
            float score = PlayerPrefs.GetFloat("HighScore_Scene2", 0f);
            if (score == 0f)
            {
                score = PlayerPrefs.GetFloat("HighScore_Stage2", 0f);
            }
            return score;
        }
        
        // Method to clear specific stage scores with sound
        public void ClearStage1Score()
        {
            StartCoroutine(PlaySoundAndClearStage1());
        }
        
        public void ClearStage2Score()
        {
            StartCoroutine(PlaySoundAndClearStage2());
        }
        
        private System.Collections.IEnumerator PlaySoundAndClearStage1()
        {
            // Play button click sound
            if (buttonAudioSource != null && buttonClickClip != null)
            {
                buttonAudioSource.PlayOneShot(buttonClickClip);
                yield return new WaitForSeconds(buttonClickClip.length);
            }
            
            PlayerPrefs.DeleteKey("HighScore_Stage1");
            PlayerPrefs.Save();
            UpdateHighScoreDisplay();
            Debug.Log("Stage 1 score cleared!");
        }
        
        private System.Collections.IEnumerator PlaySoundAndClearStage2()
        {
            // Play button click sound
            if (buttonAudioSource != null && buttonClickClip != null)
            {
                buttonAudioSource.PlayOneShot(buttonClickClip);
                yield return new WaitForSeconds(buttonClickClip.length);
            }
            
            PlayerPrefs.DeleteKey("HighScore_Stage2");
            PlayerPrefs.DeleteKey("HighScore_Scene2");
            PlayerPrefs.Save();
            UpdateHighScoreDisplay();
            Debug.Log("Stage 2 score cleared!");
        }
        
        // Check if player has completed stages
        public bool HasCompletedStage1()
        {
            return GetStage1Score() > 0f;
        }
        
        public bool HasCompletedStage2()
        {
            return GetStage2Score() > 0f;
        }
    }
}