using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Cainos.PixelArtTopDown_Basic;

public class TransitionScript : MonoBehaviour
{
    [Header("Dialogue Settings")]
    public DialogueManager dialogueManager;  // Reference to DialogueManager component
    public string transitionMessage = "Stage Complete! Moving to next area...";
    
    [Header("Scene Settings")]
    public string nextSceneName = "Scene2";
    public float dialogueDisplayTime = 5f;
    
    [Header("Optional Settings")]
    public AudioSource audioSource;
    public AudioClip transitionSound;
    
    void Start()
    {
        Debug.Log("TransitionScript: Starting");
        
        // Auto-find DialogueManager if not assigned
        if (dialogueManager == null)
        {
            dialogueManager = FindObjectOfType<DialogueManager>();
        }
        
        StartTransition();
    }
    
    public void StartTransition()
    {
        StartCoroutine(TransitionSequence());
    }
    
    private IEnumerator TransitionSequence()
    {
        Debug.Log("TransitionScript: Starting transition sequence");
        
        // Use DialogueManager to force show the dialogue (bypassing restrictions)
        if (dialogueManager != null)
        {
            Debug.Log("TransitionScript: Force showing dialogue through DialogueManager");
            dialogueManager.ForceShowDialogue(transitionMessage);
        }
        else
        {
            Debug.LogError("TransitionScript: DialogueManager not found!");
        }
        
        // Play transition sound if available
        if (audioSource != null && transitionSound != null)
        {
            audioSource.PlayOneShot(transitionSound);
        }
        
        Debug.Log("TransitionScript: Waiting " + dialogueDisplayTime + " seconds");
        // Wait for the specified time
        yield return new WaitForSeconds(dialogueDisplayTime);
        
        // Hide dialogue before switching scenes
        if (dialogueManager != null)
        {
            Debug.Log("TransitionScript: Hiding dialogue");
            dialogueManager.HideDialogue();
        }
        
        Debug.Log("TransitionScript: Loading scene: " + nextSceneName);
        // Load the next scene
        SceneManager.LoadScene(nextSceneName);
    }
    
    // Optional: Allow manual scene switching
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            // Skip waiting and go to next scene immediately
            Debug.Log("TransitionScript: Player skipped transition");
            StopAllCoroutines();
            
            // Hide dialogue before switching
            if (dialogueManager != null)
            {
                dialogueManager.HideDialogue();
            }
            
            SceneManager.LoadScene(nextSceneName);
        }
    }
}