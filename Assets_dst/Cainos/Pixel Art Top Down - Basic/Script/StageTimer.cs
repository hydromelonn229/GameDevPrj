using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Cainos.PixelArtTopDown_Basic
{
    public class StageTimer : MonoBehaviour
    {
        [Header("Timer UI")]
        public TextMeshProUGUI timerText;
        public TextMeshProUGUI highScoreText;
        
        [Header("Stage Settings")]
        public string stageName = "Stage1"; // "Stage1", "Stage2", etc.
        public bool startTimerOnAwake = true;
        
        [Header("Timer Format")]
        public bool showMilliseconds = true;
        
        // Timer variables
        private float currentTime = 0f;
        private bool isTimerRunning = false;
        private bool stageCompleted = false;
        
        // High score key for PlayerPrefs
        private string highScoreKey => $"HighScore_{stageName}";
        
        // Static reference for easy access from other scripts
        public static StageTimer Instance { get; private set; }
        
        void Awake()
        {
            Debug.Log($"StageTimer Awake() called on {gameObject.name}");
            
            // Simple singleton pattern - allow only one instance per scene
            if (Instance == null)
            {
                Instance = this;
                Debug.Log($"StageTimer: Instance set to {gameObject.name}");
            }
            else if (Instance != this)
            {
                Debug.Log($"StageTimer: Duplicate instance on {gameObject.name} destroyed, keeping {Instance.gameObject.name}");
                Destroy(gameObject);
                return;
            }
            
            Debug.Log($"StageTimer: Awake completed successfully on {gameObject.name}");
        }
        
        void Start()
        {
            Debug.Log($"StageTimer Start() called on {gameObject.name}");
            Debug.Log($"Timer Text assigned: {timerText != null}");
            if (timerText != null)
            {
                Debug.Log($"Timer Text object name: {timerText.gameObject.name}");
                Debug.Log($"Timer Text current text: '{timerText.text}'");
            }
            Debug.Log($"Start Timer On Awake: {startTimerOnAwake}");
            Debug.Log($"Stage Name: {stageName}");
            
            if (startTimerOnAwake)
            {
                StartTimer();
            }
            else
            {
                Debug.Log("Timer not starting automatically - startTimerOnAwake is false");
            }
            
            DisplayHighScore();
            
            // Force initial display update
            Debug.Log("Forcing initial timer display update...");
            UpdateTimerDisplay();
            
            Debug.Log("StageTimer Start() completed successfully");
        }
        
        void Update()
        {
            if (isTimerRunning && !stageCompleted)
            {
                currentTime += Time.deltaTime;
                UpdateTimerDisplay();
            }
        }
        
        public void StartTimer()
        {
            isTimerRunning = true;
            currentTime = 0f;
            stageCompleted = false;
            Debug.Log($"{stageName}: Timer started! isTimerRunning={isTimerRunning}, currentTime={currentTime}");
            UpdateTimerDisplay(); // Update display immediately after starting
        }
        
        public void StopTimer()
        {
            if (!isTimerRunning) return;
            
            isTimerRunning = false;
            stageCompleted = true;
            
            Debug.Log($"{stageName}: Timer stopped! Final time: {FormatTime(currentTime)}");
            
            // Check if this is a new high score (lower time is better)
            CheckAndSaveHighScore();
        }
        
        public void PauseTimer()
        {
            isTimerRunning = false;
            Debug.Log($"{stageName}: Timer paused at {FormatTime(currentTime)}");
        }
        
        public void ResumeTimer()
        {
            if (!stageCompleted)
            {
                isTimerRunning = true;
                Debug.Log($"{stageName}: Timer resumed!");
            }
        }
        
        public void ResetTimer()
        {
            currentTime = 0f;
            isTimerRunning = false;
            stageCompleted = false;
            UpdateTimerDisplay();
            Debug.Log($"{stageName}: Timer reset!");
        }
        
        public void HideTimerUI()
        {
            if (timerText != null)
            {
                timerText.gameObject.SetActive(false);
            }
            if (highScoreText != null)
            {
                highScoreText.gameObject.SetActive(false);
            }
            Debug.Log($"{stageName}: Timer UI hidden!");
        }
        
        public void ShowTimerUI()
        {
            if (timerText != null)
            {
                timerText.gameObject.SetActive(true);
            }
            if (highScoreText != null)
            {
                highScoreText.gameObject.SetActive(true);
            }
            Debug.Log($"{stageName}: Timer UI shown!");
        }
        
        private void UpdateTimerDisplay()
        {
            if (timerText != null)
            {
                string formattedTime = FormatTime(currentTime);
                // Use string interpolation to ensure single line formatting
                timerText.text = $"Timer : {formattedTime}";
            }
            else
            {
                Debug.LogError("StageTimer: Timer Text is null! Please assign the Score GameObject to Timer Text in the Inspector.");
            }
        }
        
        private void DisplayHighScore()
        {
            float highScore = GetHighScore();
            
            if (highScoreText != null)
            {
                if (highScore > 0)
                {
                    highScoreText.text = "Best: " + FormatTime(highScore);
                }
                else
                {
                    highScoreText.text = "Best: --:--";
                }
            }
        }
        
        private void CheckAndSaveHighScore()
        {
            float currentHighScore = GetHighScore();
            
            // If no high score exists, or current time is better (lower)
            if (currentHighScore == 0f || currentTime < currentHighScore)
            {
                SaveHighScore(currentTime);
                Debug.Log($"{stageName}: NEW HIGH SCORE! {FormatTime(currentTime)}");
                
                // Show new high score message
                if (highScoreText != null)
                {
                    StartCoroutine(ShowNewHighScoreEffect());
                }
            }
            else
            {
                Debug.Log($"{stageName}: Time: {FormatTime(currentTime)} (Best: {FormatTime(currentHighScore)})");
            }
            
            DisplayHighScore();
        }
        
        private IEnumerator ShowNewHighScoreEffect()
        {
            Color originalColor = highScoreText.color;
            
            // Flash effect for new high score
            for (int i = 0; i < 6; i++)
            {
                highScoreText.color = Color.yellow;
                yield return new WaitForSeconds(0.2f);
                highScoreText.color = originalColor;
                yield return new WaitForSeconds(0.2f);
            }
        }
        
        private string FormatTime(float time)
        {
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
        
        // Public methods for accessing timer data
        public float GetCurrentTime() => currentTime;
        public bool IsRunning() => isTimerRunning;
        public bool IsCompleted() => stageCompleted;
        
        // High score management
        public float GetHighScore()
        {
            return PlayerPrefs.GetFloat(highScoreKey, 0f);
        }
        
        public void SaveHighScore(float time)
        {
            PlayerPrefs.SetFloat(highScoreKey, time);
            PlayerPrefs.Save();
        }
        
        public void ClearHighScore()
        {
            PlayerPrefs.DeleteKey(highScoreKey);
            PlayerPrefs.Save();
            DisplayHighScore();
            Debug.Log($"{stageName}: High score cleared!");
        }
        
        // Static methods for easy access
        public static void StopCurrentTimer()
        {
            if (Instance != null)
            {
                Instance.StopTimer();
            }
        }
        
        public static void HideCurrentTimerUI()
        {
            if (Instance != null)
            {
                Instance.HideTimerUI();
            }
        }
        
        public static void ShowCurrentTimerUI()
        {
            if (Instance != null)
            {
                Instance.ShowTimerUI();
            }
        }
        
        public static float GetCurrentStageTime()
        {
            return Instance != null ? Instance.GetCurrentTime() : 0f;
        }
    }
}