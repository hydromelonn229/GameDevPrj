using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cainos.PixelArtTopDown_Basic
{
    //door that opens when specific altar(s) are activated
    //never opens without the required altar(s) being propped
    
    public class AltarActivatedDoor : MonoBehaviour
    {
        [Header("Door GameObjects")]
        public GameObject closedDoorGameObject;  // The closed door sprite/collider
        public GameObject openedDoorGameObject;  // The opened door sprite (walkable)
        
        [Header("Door Animation")]
        public Animator animator;
        
        [Header("Altar Requirements")]
        public List<int> requiredAltarIds = new List<int>(); // Which altars need to be activated
        [Tooltip("If true, ALL specified altars must be activated. If false, only ONE altar needs to be activated.")]
        public bool requireAllAltars = true;
        
        [Header("Dialogue Settings")]
        public DialogueManager dialogueManager;
        
        [Header("Dialogue Checkboxes")]
        [Space(10)]
        public bool showFirstApproachDialogue = true;
        [TextArea(2, 4)]
        public string firstApproachMessage = "This ancient door seems to be sealed by mystical forces. Perhaps an altar needs to be activated...";
        
        [Space(5)]
        public bool showNoKeyDialogue = true;
        [TextArea(2, 4)]
        public string noKeyMessage = "The door remains sealed. The altar's power is needed to unlock this barrier.";
        
        [Space(5)]
        public bool showDoorOpenDialogue = true;
        [TextArea(2, 4)]
        public string doorOpenMessage = "The altar's energy flows through the door! The ancient seal is broken!";
        
        [Header("Audio Settings")]
        public AudioSource audioSource;
        public AudioClip doorOpenSound;
        public AudioClip doorLockedSound;
        
        [Header("Interaction Settings")]
        public string playerTag = "Player";
        public float interactionCooldown = 1f;
        
        // Private variables
        private bool isOpen = false;
        private bool hasShownFirstApproach = false;
        private bool hasShownOpenDialogue = false;
        private float lastInteractionTime = 0f;
        
        void Start()
        {
            // Auto-find components if not assigned
            if (animator == null)
                animator = GetComponent<Animator>();
                
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
                
            if (dialogueManager == null)
                dialogueManager = FindObjectOfType<DialogueManager>();
            
            // Initialize door state (closed by default)
            SetDoorVisual(false);
            
            // Subscribe to AltarManager events for efficient monitoring
            AltarManager.OnAltarAwakened += OnAltarStateChanged;
            AltarManager.OnAltarDeactivated += OnAltarStateChanged;
            
            // Check initial altar state
            CheckAltarStatus();
        }
        
        void OnDestroy()
        {
            // Unsubscribe from events to prevent memory leaks
            AltarManager.OnAltarAwakened -= OnAltarStateChanged;
            AltarManager.OnAltarDeactivated -= OnAltarStateChanged;
        }
        
        void Update()
        {
            // Event-based system handles most changes, but keep a backup check
            // This is useful for edge cases or if events fail
            CheckAltarStatus();
        }
        
        // Event handler called whenever any altar state changes
        private void OnAltarStateChanged(int altarId)
        {
            // Only respond if this altar affects our door
            if (requiredAltarIds.Contains(altarId))
            {
                Debug.Log($"AltarActivatedDoor: Altar {altarId} state changed - checking door status");
                CheckAltarStatus();
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(playerTag) && Time.time > lastInteractionTime + interactionCooldown)
            {
                lastInteractionTime = Time.time;
                HandlePlayerInteraction();
            }
        }
        
        private void HandlePlayerInteraction()
        {
            if (isOpen)
            {
                // Door is already open, no need for dialogue
                return;
            }
            
            // Check if altars are activated
            bool altarsActivated = AreRequiredAltarsActivated();
            
            if (altarsActivated)
            {
                // Altars are activated, open the door
                OpenDoor();
            }
            else
            {
                // Altars not activated, show appropriate dialogue
                if (!hasShownFirstApproach && showFirstApproachDialogue)
                {
                    ShowDialogue(firstApproachMessage);
                    hasShownFirstApproach = true;
                }
                else if (showNoKeyDialogue)
                {
                    ShowDialogue(noKeyMessage);
                }
                
                // Play locked sound
                PlaySound(doorLockedSound);
            }
        }
        
        private void CheckAltarStatus()
        {
            bool altarsActivated = AreRequiredAltarsActivated();
            
            if (!isOpen && altarsActivated)
            {
                // Altars were just activated, open the door automatically
                OpenDoor();
            }
            else if (isOpen && !altarsActivated)
            {
                // Altars were deactivated, close the door automatically
                CloseDoor();
            }
        }
        
        private bool AreRequiredAltarsActivated()
        {
            if (requiredAltarIds.Count == 0)
            {
                Debug.LogWarning("AltarActivatedDoor: No altar IDs specified!");
                return false;
            }
            
            if (requireAllAltars)
            {
                // Check if ALL specified altars are activated
                foreach (int altarId in requiredAltarIds)
                {
                    if (!AltarManager.IsAltarAwakened(altarId))
                    {
                        return false; // If any altar is not activated, return false
                    }
                }
                return true; // All altars are activated
            }
            else
            {
                // Check if ANY of the specified altars are activated
                foreach (int altarId in requiredAltarIds)
                {
                    if (AltarManager.IsAltarAwakened(altarId))
                    {
                        return true; // If any altar is activated, return true
                    }
                }
                return false; // No altars are activated
            }
        }
        
        private void OpenDoor()
        {
            if (isOpen) return; // Already open
            
            isOpen = true;
            
            // Switch door GameObjects
            SetDoorVisual(true);
            
            // Play door animation
            if (animator != null)
            {
                animator.SetBool("Open", true);
            }
            
            // Show door open dialogue
            if (showDoorOpenDialogue && !hasShownOpenDialogue)
            {
                ShowDialogue(doorOpenMessage);
                hasShownOpenDialogue = true;
            }
            
            // Play door open sound
            PlaySound(doorOpenSound);
            
            Debug.Log($"AltarActivatedDoor: Door opened! Required altars activated: {string.Join(", ", requiredAltarIds)}");
        }
        
        private void CloseDoor()
        {
            if (!isOpen) return; // Already closed
            
            isOpen = false;
            
            // Switch door GameObjects back to closed
            SetDoorVisual(false);
            
            // Play door animation
            if (animator != null)
            {
                animator.SetBool("Open", false);
            }
            
            // Reset dialogue states when door closes so they can be shown again when reopened
            hasShownOpenDialogue = false;
            // Don't reset first approach dialogue - player has already seen it
            
            // Play door close sound (reuse doorLockedSound or add doorCloseSound)
            PlaySound(doorLockedSound);
            
            Debug.Log($"AltarActivatedDoor: Door closed! Required altars deactivated: {string.Join(", ", requiredAltarIds)}");
        }
        
        private void SetDoorVisual(bool doorOpen)
        {
            // Show/hide closed door GameObject
            if (closedDoorGameObject != null)
            {
                closedDoorGameObject.SetActive(!doorOpen); // Hide when door is open
            }
            
            // Show/hide opened door GameObject
            if (openedDoorGameObject != null)
            {
                openedDoorGameObject.SetActive(doorOpen); // Show when door is open
            }
            
            Debug.Log($"AltarActivatedDoor: Door visual set to {(doorOpen ? "OPEN" : "CLOSED")}");
        }
        
        private void ShowDialogue(string message)
        {
            if (dialogueManager != null && !string.IsNullOrEmpty(message))
            {
                dialogueManager.ShowDialogue(message);
            }
        }
        
        private void PlaySound(AudioClip clip)
        {
            if (audioSource != null && clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }
        
        // Public methods for external control
        public bool IsOpen()
        {
            return isOpen;
        }
        
        public void ForceOpen()
        {
            OpenDoor();
        }
        
        public void ForceClose()
        {
            CloseDoor();
        }
        
        public void SetRequiredAltars(List<int> altarIds)
        {
            requiredAltarIds = altarIds;
        }
        
        public void SetRequiredAltars(params int[] altarIds)
        {
            requiredAltarIds = new List<int>(altarIds);
        }
        
        // Reset dialogue states (useful for testing)
        public void ResetDialogueStates()
        {
            hasShownFirstApproach = false;
            hasShownOpenDialogue = false;
        }
        
        // Get current altar status for debugging
        public string GetAltarStatus()
        {
            string status = "Required Altars: ";
            foreach (int altarId in requiredAltarIds)
            {
                bool isActivated = AltarManager.IsAltarAwakened(altarId);
                status += $"[{altarId}: {(isActivated ? "Active" : "Inactive")}] ";
            }
            return status;
        }
    }
}