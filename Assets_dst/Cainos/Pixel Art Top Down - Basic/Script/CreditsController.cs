using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class CreditsController : MonoBehaviour
{
    [Header("Scroll Settings")]
    public ScrollRect scrollRect;
    public float scrollSpeed = 0.2f;
    public bool autoScroll = true;
    public bool allowManualScrolling = false;  // Set to false to prevent manual scrolling
    
    [Header("Audio")]
    public AudioSource buttonAudioSource;
    public AudioClip buttonClickClip;
    public AudioSource backgroundMusicSource;  // For credits background music
    
    [Header("Scene Settings")]
    public string mainMenuSceneName = "Main Menu";
    
    private bool isScrolling = true;
    
    void Start()
    {
        // Start from the top
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 1f;
            
            // Disable manual scrolling if not allowed
            if (!allowManualScrolling)
            {
                scrollRect.horizontal = false;
                scrollRect.vertical = false;
                scrollRect.scrollSensitivity = 0f;
            }
        }
        
        // Start background music if assigned
        if (backgroundMusicSource != null && !backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.Play();
        }
        
        if (autoScroll)
        {
            StartCoroutine(AutoScrollCredits());
        }
    }
    
    void Update()
    {
        // Only allow manual interaction if enabled
        if (allowManualScrolling)
        {
            // Allow manual scrolling with mouse wheel or arrow keys
            if (Input.GetAxis("Mouse ScrollWheel") != 0 || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
            {
                isScrolling = false; // Stop auto-scroll if user interacts
            }
        }
        
        // Press Escape or Space to go back to main menu
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space))
        {
            GoBackToMainMenu();
        }
    }
    
    private IEnumerator AutoScrollCredits()
    {
        yield return new WaitForSeconds(1f); // Wait a bit before starting
        
        while (isScrolling && scrollRect != null && scrollRect.verticalNormalizedPosition > 0f)
        {
            scrollRect.verticalNormalizedPosition -= scrollSpeed * Time.deltaTime;
            yield return null;
        }
        
        // Wait at the end, then return to main menu
        yield return new WaitForSeconds(3f);
        GoBackToMainMenu();
    }
    
    public void GoBackToMainMenu()
    {
        StartCoroutine(PlaySoundAndGoBack());
    }
    
    private IEnumerator PlaySoundAndGoBack()
    {
        // Fade out background music
        if (backgroundMusicSource != null && backgroundMusicSource.isPlaying)
        {
            StartCoroutine(FadeOutMusic(1f));
        }
        
        if (buttonAudioSource != null && buttonClickClip != null)
        {
            buttonAudioSource.PlayOneShot(buttonClickClip);
            yield return new WaitForSeconds(buttonClickClip.length);
        }
        
        SceneManager.LoadScene(mainMenuSceneName);
    }
    
    private IEnumerator FadeOutMusic(float fadeTime)
    {
        float startVolume = backgroundMusicSource.volume;
        
        while (backgroundMusicSource.volume > 0)
        {
            backgroundMusicSource.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }
        
        backgroundMusicSource.Stop();
        backgroundMusicSource.volume = startVolume; // Reset for next time
    }
    
    public void ToggleAutoScroll()
    {
        isScrolling = !isScrolling;
        if (isScrolling)
        {
            StartCoroutine(AutoScrollCredits());
        }
    }
    
    public void ToggleManualScrolling()
    {
        allowManualScrolling = !allowManualScrolling;
        
        if (scrollRect != null)
        {
            if (allowManualScrolling)
            {
                // Enable manual scrolling
                scrollRect.vertical = true;
                scrollRect.scrollSensitivity = 1f;
            }
            else
            {
                // Disable manual scrolling
                scrollRect.vertical = false;
                scrollRect.scrollSensitivity = 0f;
            }
        }
    }
}