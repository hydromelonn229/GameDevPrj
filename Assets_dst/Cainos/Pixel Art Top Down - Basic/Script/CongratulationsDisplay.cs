using UnityEngine;
using TMPro;

namespace Cainos.PixelArtTopDown_Basic
{
    public class CongratulationsDisplay : MonoBehaviour
    {
        [Header("Score Display")]
        public TextMeshProUGUI currentScoreText;
        public TextMeshProUGUI bestScoreText;
        public TextMeshProUGUI stage1BestText;
        public TextMeshProUGUI stage2BestText;
        
        [Header("Settings")]
        public bool showMilliseconds = true;
        public string currentScoreLabel = "Your Score: ";
        public string bestScoreLabel = "Best Time: ";
        public string stage1Label = "Stage 1 Best: ";
        public string stage2Label = "Stage 2 Best: ";
        public string noScoreText = "--:--";
        
        void OnEnable()
        {
            // Update scores when the canvas becomes active
            UpdateScoreDisplay();
        }
        
        public void UpdateScoreDisplay()
        {
            // Get the current stage time from StageTimer
            float currentTime = 0f;
            string currentStageName = "";
            
            if (StageTimer.Instance != null)
            {
                currentTime = StageTimer.Instance.GetCurrentTime();
                currentStageName = StageTimer.Instance.stageName;
                Debug.Log($"CongratulationsDisplay: Got time {currentTime} from StageTimer for stage {currentStageName}");
            }
            else
            {
                Debug.LogError("CongratulationsDisplay: StageTimer.Instance is null!");
                // Try to find StageTimer in the scene as backup
                StageTimer stageTimer = FindObjectOfType<StageTimer>();
                if (stageTimer != null)
                {
                    currentTime = stageTimer.GetCurrentTime();
                    currentStageName = stageTimer.stageName;
                    Debug.Log($"CongratulationsDisplay: Found StageTimer via FindObjectOfType, got time {currentTime}");
                }
            }
            
            // Display current score
            if (currentScoreText != null)
            {
                if (currentTime > 0)
                {
                    currentScoreText.text = currentScoreLabel + FormatTime(currentTime);
                }
                else
                {
                    currentScoreText.text = currentScoreLabel + noScoreText;
                }
            }
            
            // Display best score for current stage
            if (bestScoreText != null)
            {
                float bestTime = GetBestTimeForStage(currentStageName);
                if (bestTime > 0)
                {
                    bestScoreText.text = bestScoreLabel + FormatTime(bestTime);
                }
                else
                {
                    bestScoreText.text = bestScoreLabel + noScoreText;
                }
            }
            
            // Display Stage 1 best time
            if (stage1BestText != null)
            {
                float stage1Best = PlayerPrefs.GetFloat("HighScore_Stage1", 0f);
                if (stage1Best > 0)
                {
                    stage1BestText.text = stage1Label + FormatTime(stage1Best);
                }
                else
                {
                    stage1BestText.text = stage1Label + noScoreText;
                }
            }
            
            // Display Stage 2 best time
            if (stage2BestText != null)
            {
                float stage2Best = PlayerPrefs.GetFloat("HighScore_Scene2", 0f);
                if (stage2Best == 0f)
                {
                    stage2Best = PlayerPrefs.GetFloat("HighScore_Stage2", 0f);
                }
                
                if (stage2Best > 0)
                {
                    stage2BestText.text = stage2Label + FormatTime(stage2Best);
                }
                else
                {
                    stage2BestText.text = stage2Label + noScoreText;
                }
            }
            
            Debug.Log($"Congratulations Display Updated - Current: {FormatTime(currentTime)}, Stage: {currentStageName}");
        }
        
        private float GetBestTimeForStage(string stageName)
        {
            if (string.IsNullOrEmpty(stageName))
                return 0f;
                
            return PlayerPrefs.GetFloat($"HighScore_{stageName}", 0f);
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
        
        // Method to manually refresh display (can be called from buttons)
        public void RefreshDisplay()
        {
            UpdateScoreDisplay();
        }
        
        // Method to manually set the completion time (useful if StageTimer instance is lost)
        public void SetCompletionTime(float completionTime)
        {
            if (currentScoreText != null)
            {
                currentScoreText.text = currentScoreLabel + FormatTime(completionTime);
                Debug.Log($"CongratulationsDisplay: Manually set completion time to {FormatTime(completionTime)}");
            }
        }
    }
}