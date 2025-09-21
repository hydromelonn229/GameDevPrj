using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Cainos.PixelArtTopDown_Basic
{
    public class DialogueManager : MonoBehaviour
    {
        [Header("UI References")]
        public GameObject dialoguePanel;
        public GameObject backgroundShade; // Full-screen dark overlay
        public TextMeshProUGUI dialogueText; // Changed to support TextMeshPro
        public Button closeButton;
        
        [Header("Animation Settings")]
        public float slideSpeed = 10f;
        public AnimationCurve slideAnimation = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        [Header("Dialogue Settings")]
        public float typewriterSpeed = 0.05f;
        public bool enableTypewriterEffect = true;
        
        // Static properties to track dialogue state (similar to ChestAudio pattern)
        private static bool isDialogueShowing = false;
        private static bool isWaitingForKeyConfirmation = false;
        
        // Animation and state variables
        private RectTransform panelRectTransform;
        private Vector2 hiddenPosition;
        private Vector2 shownPosition;
        private Coroutine currentDialogueCoroutine;
        private Coroutine currentAnimationCoroutine;
        
        // Input handling
        private bool canCloseDialogue = false;
        private float lastInteractionTime = 0f;
        private float interactionCooldown = 0.5f; // Prevent rapid toggling

        void Start()
        {
            SetupDialoguePanel();
            SetupCloseButton();
            
            // Start with dialogue hidden
            HideDialogueImmediate();
            
            // Ensure text starts empty
            if (dialogueText != null)
            {
                dialogueText.text = "";
            }
        }

        void Update()
        {
            // Handle input for closing dialogue (F key or Escape)
            // Only allow closing if dialogue is fully shown and typewriter is done
            if (isDialogueShowing && canCloseDialogue && !isWaitingForKeyConfirmation)
            {
                if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Escape))
                {
                    HideDialogue();
                }
            }
        }

        private void SetupDialoguePanel()
        {
            // Auto-find components if not assigned
            if (dialoguePanel == null)
            {
                // Look for a child GameObject named "DialoguePanel" or similar
                dialoguePanel = transform.Find("DialoguePanel")?.gameObject;
                if (dialoguePanel == null)
                {
                    Debug.LogWarning("DialogueManager: No dialogue panel assigned or found!");
                    return;
                }
            }

            // Auto-find background shade if not assigned
            if (backgroundShade == null)
            {
                backgroundShade = transform.Find("BackgroundShade")?.gameObject;
            }

            // If dialoguePanel IS the background shade (same object), don't animate it
            if (dialoguePanel == backgroundShade || backgroundShade == null)
            {
                // Just use simple show/hide without animation
                panelRectTransform = null;
                Debug.Log("DialogueManager: Using simple show/hide mode (no slide animation)");
                return;
            }

            panelRectTransform = dialoguePanel.GetComponent<RectTransform>();
            
            if (panelRectTransform != null)
            {
                // Calculate positions for slide animation (from bottom)
                Canvas parentCanvas = GetComponentInParent<Canvas>();
                if (parentCanvas != null)
                {
                    RectTransform canvasRect = parentCanvas.GetComponent<RectTransform>();
                    float panelHeight = panelRectTransform.rect.height;
                    
                    // Shown position (visible at bottom)
                    shownPosition = new Vector2(0, panelHeight / 2);
                    // Hidden position (below screen)
                    hiddenPosition = new Vector2(0, -panelHeight);
                }
            }

            // Auto-find dialogue text if not assigned
            if (dialogueText == null)
            {
                dialogueText = dialoguePanel.GetComponentInChildren<TextMeshProUGUI>();
                if (dialogueText != null)
                {
                    Debug.Log("DialogueManager: Auto-found Text component: " + dialogueText.name);
                }
                else
                {
                    Debug.LogWarning("DialogueManager: Could not find TextMeshProUGUI component in DialoguePanel children!");
                }
            }
        }

        private void SetupCloseButton()
        {
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(HideDialogue);
            }
        }

        // Public method to show dialogue (called by SignController)
        public void ShowDialogue(string text)
        {
            if (isDialogueShowing) return; // Prevent multiple dialogues
            if (Time.time - lastInteractionTime < interactionCooldown) return; // Prevent rapid toggling

            lastInteractionTime = Time.time;
            isDialogueShowing = true;
            isWaitingForKeyConfirmation = true;
            canCloseDialogue = false;

            // Clear any previous text first
            if (dialogueText != null)
            {
                dialogueText.text = "";
            }

            // Show the panel - use simple show/hide if no animation needed
            if (panelRectTransform == null)
            {
                // Simple show/hide mode
                if (dialoguePanel != null)
                {
                    dialoguePanel.SetActive(true);
                    
                    // Enable components that might be disabled
                    var imageComponent = dialoguePanel.GetComponent<Image>();
                    if (imageComponent != null)
                    {
                        imageComponent.enabled = true;
                        Debug.Log("DialogueManager: Enabled Image component");
                    }
                        
                    if (dialogueText != null)
                    {
                        dialogueText.enabled = true;
                        Debug.Log("DialogueManager: Enabled Text component - " + dialogueText.name);
                    }
                    else
                    {
                        Debug.LogWarning("DialogueManager: dialogueText is null! Cannot enable text component.");
                    }
                }
            }
            else
            {
                // Animation mode
                if (currentAnimationCoroutine != null)
                    StopCoroutine(currentAnimationCoroutine);
                
                currentAnimationCoroutine = StartCoroutine(AnimatePanel(shownPosition, true));
            }

            // Display the text
            if (enableTypewriterEffect && dialogueText != null)
            {
                if (currentDialogueCoroutine != null)
                    StopCoroutine(currentDialogueCoroutine);
                Debug.Log("DialogueManager: Starting typewriter effect with text: " + text);
                currentDialogueCoroutine = StartCoroutine(TypewriterEffect(text));
            }
            else if (dialogueText != null)
            {
                dialogueText.text = text;
                canCloseDialogue = true;
                Debug.Log("DialogueManager: Set text directly: " + text);
            }
            else
            {
                Debug.LogError("DialogueManager: Cannot display text - dialogueText is null!");
            }

            Debug.Log("DialogueManager: Showing dialogue - " + text);
        }

        // Public method to hide dialogue
        public void HideDialogue()
        {
            if (!isDialogueShowing) return;
            if (Time.time - lastInteractionTime < interactionCooldown) return; // Prevent rapid toggling

            lastInteractionTime = Time.time;
            
            // Reset all dialogue states FIRST
            isDialogueShowing = false;
            isWaitingForKeyConfirmation = false;
            canCloseDialogue = false;

            // Hide the panel - use simple show/hide if no animation needed
            if (panelRectTransform == null)
            {
                // Simple show/hide mode
                if (dialogueText != null)
                {
                    dialogueText.text = "";
                    dialogueText.enabled = false; // Disable text component
                }
                
                if (dialoguePanel != null)
                {
                    // Disable image component
                    var imageComponent = dialoguePanel.GetComponent<Image>();
                    if (imageComponent != null)
                        imageComponent.enabled = false;
                        
                    // Keep panel active but components disabled
                    // dialoguePanel.SetActive(false); // Don't need this anymore
                }
            }
            else
            {
                // Animation mode
                if (currentAnimationCoroutine != null)
                    StopCoroutine(currentAnimationCoroutine);
                
                currentAnimationCoroutine = StartCoroutine(AnimatePanel(hiddenPosition, false));
            }

            // Stop any ongoing typewriter effect
            if (currentDialogueCoroutine != null)
            {
                StopCoroutine(currentDialogueCoroutine);
                currentDialogueCoroutine = null;
            }

            Debug.Log("DialogueManager: Hiding dialogue - movement should be restored");
        }

        private void HideDialogueImmediate()
        {
            if (panelRectTransform != null)
            {
                panelRectTransform.anchoredPosition = hiddenPosition;
            }
            
            // Disable components instead of deactivating GameObjects
            if (dialogueText != null)
            {
                dialogueText.text = "";
                dialogueText.enabled = false;
            }
            
            if (dialoguePanel != null)
            {
                var imageComponent = dialoguePanel.GetComponent<Image>();
                if (imageComponent != null)
                    imageComponent.enabled = false;
            }
            
            if (backgroundShade != null)
            {
                backgroundShade.SetActive(false);
            }
            
            isDialogueShowing = false;
            isWaitingForKeyConfirmation = false;
            canCloseDialogue = false;
        }

        private IEnumerator AnimatePanel(Vector2 targetPosition, bool showPanel)
        {
            if (panelRectTransform == null) yield break;

            if (showPanel)
            {
                // Show background shade first
                if (backgroundShade != null)
                {
                    backgroundShade.SetActive(true);
                }
                if (dialoguePanel != null)
                {
                    dialoguePanel.SetActive(true);
                }
            }

            Vector2 startPosition = panelRectTransform.anchoredPosition;
            float elapsedTime = 0f;
            float duration = 1f / slideSpeed;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / duration;
                float easedProgress = slideAnimation.Evaluate(progress);
                
                panelRectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, easedProgress);
                yield return null;
            }

            panelRectTransform.anchoredPosition = targetPosition;

            if (!showPanel)
            {
                if (dialoguePanel != null)
                {
                    dialoguePanel.SetActive(false);
                }
                // Hide background shade last
                if (backgroundShade != null)
                {
                    backgroundShade.SetActive(false);
                }
                // Clear text when panel is fully hidden
                if (dialogueText != null)
                {
                    dialogueText.text = "";
                }
            }

            currentAnimationCoroutine = null;
        }

        private IEnumerator TypewriterEffect(string text)
        {
            dialogueText.text = "";
            
            for (int i = 0; i <= text.Length; i++)
            {
                dialogueText.text = text.Substring(0, i);
                yield return new WaitForSeconds(typewriterSpeed);
            }

            canCloseDialogue = true;
            isWaitingForKeyConfirmation = false; // Allow movement after typewriter finishes
            currentDialogueCoroutine = null;
        }

        // Static methods to check dialogue state (similar to ChestAudio pattern)
        public static bool IsDialogueShowing()
        {
            return isDialogueShowing;
        }

        public static bool IsWaitingForKeyConfirmation()
        {
            return isWaitingForKeyConfirmation;
        }

        // Public methods for external control
        public bool CanCloseDialogue()
        {
            return canCloseDialogue;
        }

        public void SetTypewriterSpeed(float speed)
        {
            typewriterSpeed = speed;
        }

        public void SetSlideSpeed(float speed)
        {
            slideSpeed = speed;
        }

        void OnDestroy()
        {
            // Clean up any running coroutines
            if (currentDialogueCoroutine != null)
                StopCoroutine(currentDialogueCoroutine);
            if (currentAnimationCoroutine != null)
                StopCoroutine(currentAnimationCoroutine);
            
            // Remove button listener
            if (closeButton != null)
                closeButton.onClick.RemoveListener(HideDialogue);
        }
        
        // Public method specifically for transition scenes - bypasses normal restrictions
        public void ForceShowDialogue(string text)
        {
            Debug.Log("DialogueManager: ForceShowDialogue called with text: " + text);
            
            // Force reset cooldown and state
            lastInteractionTime = 0f;
            isDialogueShowing = false;
            isWaitingForKeyConfirmation = false;
            
            // Stop any current coroutines
            if (currentDialogueCoroutine != null)
                StopCoroutine(currentDialogueCoroutine);
            if (currentAnimationCoroutine != null)
                StopCoroutine(currentAnimationCoroutine);
            
            // Force enable the dialogue panel
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(true);
                
                // Enable the panel's Image component
                Image panelImage = dialoguePanel.GetComponent<Image>();
                if (panelImage != null)
                {
                    panelImage.enabled = true;
                    Debug.Log("DialogueManager: Panel image enabled");
                }
            }
            
            // Force enable and set the text
            if (dialogueText != null)
            {
                dialogueText.enabled = true;
                dialogueText.text = text;
                Debug.Log("DialogueManager: Text set and enabled: " + text);
            }
            else
            {
                Debug.LogError("DialogueManager: dialogueText is null!");
            }
            
            // Enable background shade if it exists
            if (backgroundShade != null)
            {
                backgroundShade.SetActive(true);
                Image shadeImage = backgroundShade.GetComponent<Image>();
                if (shadeImage != null)
                {
                    shadeImage.enabled = true;
                }
            }
            
            // Set states
            isDialogueShowing = true;
            canCloseDialogue = true;
            
            Debug.Log("DialogueManager: ForceShowDialogue completed");
        }
    }
}
