using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//when something get into the altar, make the runes glow and awaken the corresponding pillar
namespace Cainos.PixelArtTopDown_Basic
{

    public class PropsAltar : MonoBehaviour
    {
        [Header("Rune Settings")]
        public List<SpriteRenderer> runes;
        public float lerpSpeed;

        [Header("Altar Settings")]
        public int altarId = 0; // Unique ID for this altar (0, 1, 2, etc.)
        public bool stayAwakenedOnce = true; // If true, altar stays awakened after first activation
        
        [Header("Dialogue Settings")]
        public bool showDialogueOnAwaken = true; // Show dialogue when altar awakens
        public DialogueManager dialogueManager;
        
        [Header("Dialogue Mode")]
        [Tooltip("Lock dialogue to show only the fixed message below, ignoring dynamic order-based messages")]
        public bool lockToFixedDialogue = false;
        [TextArea(2, 4)]
        public string fixedDialogueMessage = "This altar has been activated.";
        
        [Header("Dynamic Dialogue Messages")]
        [TextArea(2, 4)]
        public string firstAltarMessage = "The first ancient altar awakens! Its runes glow with mystical energy...";
        [TextArea(2, 4)]
        public string secondAltarMessage = "Another altar responds! The magic grows stronger as two altars are now awakened...";
        [TextArea(2, 4)]
        public string thirdAltarMessage = "The final altar is awakened! All three altars now pulse with incredible power!";
        
        private bool hasShownDialogue = false; // Track if this altar has already shown dialogue
        
        private Color curColor;
        private Color targetColor;
        private bool isAwakened = false;
        private bool hasBeenAwakened = false;

        private void Awake()
        {
            if (runes.Count > 0)
            {
                targetColor = runes[0].color;
            }
            
            // Auto-find DialogueManager if not assigned
            if (dialogueManager == null)
            {
                dialogueManager = FindObjectOfType<DialogueManager>();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Activate altar when any object enters
            ActivateAltar();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            // Only deactivate if the altar shouldn't stay awakened
            if (!stayAwakenedOnce || !hasBeenAwakened)
            {
                DeactivateAltar();
            }
        }

        private void ActivateAltar()
        {
            targetColor.a = 1.0f;
            
            if (!isAwakened)
            {
                isAwakened = true;
                hasBeenAwakened = true;
                
                // Notify the AltarManager that this altar has been awakened
                // Only count this altar in the chain if it's NOT using fixed dialogue
                AltarManager.AwakenAltar(altarId, !lockToFixedDialogue);
                
                // Show dialogue when altar awakens (only if it hasn't shown dialogue before)
                if (showDialogueOnAwaken && !hasShownDialogue)
                {
                    ShowDynamicAwakenDialogue();
                    hasShownDialogue = true; // Mark that this altar has shown its dialogue
                }
            }
        }

        private void DeactivateAltar()
        {
            targetColor.a = 0.0f;
            
            if (isAwakened)
            {
                isAwakened = false;
                
                // Notify the AltarManager that this altar has been deactivated
                // Only count this altar in the chain if it's NOT using fixed dialogue
                AltarManager.DeactivateAltar(altarId, !lockToFixedDialogue);
            }
        }

        private void Update()
        {
            curColor = Color.Lerp(curColor, targetColor, lerpSpeed * Time.deltaTime);

            foreach (var r in runes)
            {
                if (r != null)
                {
                    r.color = curColor;
                }
            }
        }
        
        // Public method to manually awaken the altar (useful for testing or scripted events)
        public void ForceAwaken()
        {
            ActivateAltar();
        }
        
        // Public method to check if this altar is currently awakened
        public bool IsAwakened()
        {
            return isAwakened;
        }
        
        // Show dialogue based on dialogue mode (fixed or dynamic)
        private void ShowDynamicAwakenDialogue()
        {
            if (dialogueManager == null)
            {
                Debug.LogWarning($"Altar {altarId}: DialogueManager not found! Cannot show awaken dialogue.");
                return;
            }

            string messageToShow = "";

            if (lockToFixedDialogue)
            {
                // Use fixed dialogue message (Scene 2 style)
                messageToShow = fixedDialogueMessage;
                Debug.Log($"Altar {altarId} awakened (Fixed Mode): {messageToShow}");
            }
            else
            {
                // Use dynamic dialogue based on awakened count (Scene 1 style)
                int awakenedCount = AltarManager.GetAwakenedAltarCount();
                
                switch (awakenedCount)
                {
                    case 1:
                        messageToShow = firstAltarMessage;
                        break;
                    case 2:
                        messageToShow = secondAltarMessage;
                        break;
                    case 3:
                        messageToShow = thirdAltarMessage;
                        break;
                    default:
                        messageToShow = firstAltarMessage; // Fallback
                        break;
                }
                Debug.Log($"Altar {altarId} awakened (Dynamic Mode - {awakenedCount} total): {messageToShow}");
            }

            dialogueManager.ShowDialogue(messageToShow);
        }
        
        // Public method to set custom dialogue messages
        public void SetDialogueMessages(string first, string second, string third)
        {
            firstAltarMessage = first;
            secondAltarMessage = second;
            thirdAltarMessage = third;
        }
        
        // Public method to manually trigger dynamic dialogue
        public void TriggerDynamicDialogue()
        {
            if (showDialogueOnAwaken && !hasShownDialogue)
            {
                ShowDynamicAwakenDialogue();
                hasShownDialogue = true;
            }
        }
        
        // Public method to reset dialogue state (for testing)
        public void ResetDialogueState()
        {
            hasShownDialogue = false;
        }
        
        // Public method to check if this altar has shown dialogue
        public bool HasShownDialogue()
        {
            return hasShownDialogue;
        }
    }
}
