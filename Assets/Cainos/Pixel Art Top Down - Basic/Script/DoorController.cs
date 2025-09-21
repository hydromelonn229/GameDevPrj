using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cainos.PixelArtTopDown_Basic;

namespace Cainos.PixelArtTopDown_Basic
{
    public class DoorController : MonoBehaviour
{
    [Header("Door Settings")]
    public GameObject closedDoorObject; // The closed door GameObject
    public GameObject openDoorObject;   // The open door GameObject
    
    [Header("Interaction")]
    public float interactRange = 1.5f;
    
    [Header("Audio")]
    public DoorAudio doorAudio;         // Reference to door audio script
    
    [Header("Dialogue Settings")]
    public bool enableDialogue = true;
    [TextArea(2, 4)]
    public string firstApproachDialogue = "This door seems to be locked. You might need a key to open it.";
    [TextArea(2, 4)]
    public string noKeyDialogue = "The door is locked tight. You need to find a key to open it.";
    [TextArea(2, 4)]
    public string doorOpenDialogue = "The key works! The door creaks open, revealing what lies beyond...";
    public DialogueManager dialogueManager;
    
    [Header("Highlighting Settings")]
    public bool enableHighlighting = true;
    public Color highlightColor = Color.cyan;
    [Range(0.1f, 2f)]
    public float outlineWidth = 0.1f;
    
    [Header("Advanced Glow Settings")]
    public bool useAdvancedGlow = true;
    [Range(0.5f, 3f)]
    public float glowIntensity = 1.5f;
    [Range(1f, 5f)]
    public float glowPulseSpeed = 2f;
    public bool enablePulse = true;
    
    private bool isDoorOpen = false;
    private bool hasKey = false;
    private bool hasShownFirstApproachDialogue = false;
    private bool hasShownOpenDialogue = false;
    private bool wasPlayerNearby = false;
    
    // Highlighting system variables
    private bool isPlayerNearby = false;
    private bool isHighlighted = false;
    private GameObject outlineObject;
    private SpriteRenderer doorSpriteRenderer;
    private Color originalColor;
    private Material originalMaterial;
    
    // Advanced glow system variables
    private Material glowMaterial;
    private float glowTimer = 0f;

    void Start()
    {
        // Start with closed door
        SetDoorClosed();
        
        // If no door audio is assigned, try to find one on this GameObject
        if (doorAudio == null)
        {
            doorAudio = GetComponent<DoorAudio>();
            
            // If still no DoorAudio component, add one automatically
            if (doorAudio == null)
            {
                doorAudio = gameObject.AddComponent<DoorAudio>();
            }
        }
        
        // Auto-find DialogueManager if not assigned
        if (dialogueManager == null)
        {
            dialogueManager = FindObjectOfType<DialogueManager>();
        }
        
        // Initialize glow system - get sprite renderer from the closed door
        if (closedDoorObject != null)
        {
            doorSpriteRenderer = closedDoorObject.GetComponent<SpriteRenderer>();
            if (doorSpriteRenderer != null)
            {
                originalColor = doorSpriteRenderer.color;
                originalMaterial = doorSpriteRenderer.material;
                
                if (useAdvancedGlow)
                {
                    CreateGlowMaterial();
                }
            }
        }
    }

    void Update()
    {
        // Check if player has key by looking for inventory key
        CheckPlayerHasKey();
        
        bool isPlayerCurrentlyNearby = IsPlayerNearby();
        
        // Handle first approach dialogue (only when player doesn't have key)
        if (enableDialogue && isPlayerCurrentlyNearby && !wasPlayerNearby && !hasShownFirstApproachDialogue && !hasKey)
        {
            ShowFirstApproachDialogue();
            hasShownFirstApproachDialogue = true;
        }
        
        // Update player nearby state for next frame
        wasPlayerNearby = isPlayerCurrentlyNearby;
        
        // Handle door highlighting when player is nearby
        if (isPlayerCurrentlyNearby && !isPlayerNearby && !isDoorOpen)
        {
            OnPlayerEnterRange();
        }
        else if (!isPlayerCurrentlyNearby && isPlayerNearby)
        {
            OnPlayerExitRange();
        }
        
        // Handle glow pulsing animation
        if (isHighlighted && useAdvancedGlow && enablePulse && glowMaterial != null)
        {
            glowTimer += Time.deltaTime * glowPulseSpeed;
            float pulseValue = Mathf.Sin(glowTimer) * 0.5f + 0.5f; // 0 to 1
            float currentIntensity = glowIntensity * (0.7f + pulseValue * 0.3f); // Vary intensity
            
            // Update glow material properties
            if (glowMaterial.HasProperty("_GlowIntensity"))
            {
                glowMaterial.SetFloat("_GlowIntensity", currentIntensity);
            }
            else if (glowMaterial.HasProperty("_Brightness"))
            {
                glowMaterial.SetFloat("_Brightness", currentIntensity);
            }
            else
            {
                // Fallback to color multiplication for basic sprite shader
                Color glowColor = highlightColor * currentIntensity;
                glowColor.a = originalColor.a;
                glowMaterial.SetColor("_Color", glowColor);
            }
        }
        
        // Check if player is near and can interact
        if (isPlayerCurrentlyNearby && !isDoorOpen)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (hasKey)
                {
                    // Player has key, open the door
                    HandleDoorOpening();
                }
                else
                {
                    // Player doesn't have key, show dialogue
                    ShowNoKeyDialogue();
                }
            }
        }
    }

    void CheckPlayerHasKey()
    {
        // Check if KeyOverlay instance exists and inventory key is active
        KeyOverlay keyOverlay = FindObjectOfType<KeyOverlay>();
        hasKey = keyOverlay != null && keyOverlay.IsInventoryKeyVisible();
    }

    bool IsPlayerNearby()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return false;
        
        float distance = Vector2.Distance(transform.position, player.transform.position);
        return distance <= interactRange;
    }

    void HandleDoorOpening()
    {
        // Mark that we've shown approach dialogue if we haven't already
        // (This prevents future approach dialogues from showing)
        if (!hasShownFirstApproachDialogue)
        {
            hasShownFirstApproachDialogue = true;
        }
        
        // Always open the door - the OpenDoor method handles the opening dialogue
        OpenDoor();
    }

    void OpenDoor()
    {
        isDoorOpen = true;
        
        // Show door open dialogue (always show when door is opened with key)
        if (enableDialogue && !hasShownOpenDialogue)
        {
            ShowDoorOpenDialogue();
            hasShownOpenDialogue = true;
        }
        
        // Play door opening sound
        if (doorAudio != null)
        {
            Debug.Log("Playing door open sound...");
            doorAudio.PlayDoorOpenSoundOneShot();
        }
        else
        {
            Debug.LogWarning("DoorAudio component is null!");
        }
        
        // Hide closed door and show open door
        if (closedDoorObject != null)
        {
            closedDoorObject.SetActive(false);
        }
        
        if (openDoorObject != null)
        {
            openDoorObject.SetActive(true);
        }
    }

    void SetDoorClosed()
    {
        isDoorOpen = false;
        
        // Show closed door and hide open door
        if (closedDoorObject != null)
            closedDoorObject.SetActive(true);
        if (openDoorObject != null)
            openDoorObject.SetActive(false);
    }

    // Show dialogue when player approaches door for the first time
    private void ShowFirstApproachDialogue()
    {
        if (dialogueManager != null)
        {
            dialogueManager.ShowDialogue(firstApproachDialogue);
            Debug.Log($"Door first approach: {firstApproachDialogue}");
        }
        else
        {
            Debug.LogWarning("DoorController: DialogueManager not found! Cannot show approach dialogue.");
            Debug.Log($"Door approach: {firstApproachDialogue}");
        }
    }
    
    // Show dialogue when door is opened
    private void ShowDoorOpenDialogue()
    {
        if (dialogueManager != null)
        {
            dialogueManager.ShowDialogue(doorOpenDialogue);
            Debug.Log($"Door opened: {doorOpenDialogue}");
        }
        else
        {
            Debug.LogWarning("DoorController: DialogueManager not found! Cannot show door open dialogue.");
            Debug.Log($"Door opened: {doorOpenDialogue}");
        }
    }
    
    // Show dialogue when player tries to open door without key
    private void ShowNoKeyDialogue()
    {
        if (enableDialogue && dialogueManager != null)
        {
            dialogueManager.ShowDialogue(noKeyDialogue);
            Debug.Log($"No key dialogue: {noKeyDialogue}");
        }
        else if (enableDialogue)
        {
            Debug.LogWarning("DoorController: DialogueManager not found! Cannot show no key dialogue.");
            Debug.Log($"No key dialogue: {noKeyDialogue}");
        }
    }
    
    // Public method to set custom dialogue messages
    public void SetDialogueMessages(string approachMessage, string noKeyMessage, string openMessage)
    {
        firstApproachDialogue = approachMessage;
        noKeyDialogue = noKeyMessage;
        doorOpenDialogue = openMessage;
    }
    
    // Overload for backwards compatibility
    public void SetDialogueMessages(string approachMessage, string openMessage)
    {
        firstApproachDialogue = approachMessage;
        doorOpenDialogue = openMessage;
    }
    
    // Public method to reset dialogue states (for testing)
    public void ResetDialogueStates()
    {
        hasShownFirstApproachDialogue = false;
        hasShownOpenDialogue = false;
        Debug.Log("DoorController: Dialogue states reset - both approach and open dialogues will show again");
    }
    
    // Public method to check if door is open
    public bool IsDoorOpen()
    {
        return isDoorOpen;
    }
    
    // Player proximity detection methods
    public void OnPlayerEnterRange()
    {
        if (!isPlayerNearby && !isDoorOpen) // Only highlight if door is not opened
        {
            isPlayerNearby = true;
            StartHighlighting();
        }
    }
    
    public void OnPlayerExitRange()
    {
        if (isPlayerNearby)
        {
            isPlayerNearby = false;
            StopHighlighting();
        }
    }
    
    private void StartHighlighting()
    {
        if (doorSpriteRenderer != null && enableHighlighting && !isHighlighted)
        {
            isHighlighted = true;
            
            if (useAdvancedGlow && glowMaterial != null)
            {
                StartAdvancedGlow();
            }
            else
            {
                CreateOutlineEffect();
            }
        }
    }

    private void StopHighlighting()
    {
        if (isHighlighted)
        {
            isHighlighted = false;
            
            if (useAdvancedGlow && glowMaterial != null)
            {
                StopAdvancedGlow();
            }
            else
            {
                RemoveOutlineEffect();
            }
        }
    }
    
    private void CreateOutlineEffect()
    {
        if (outlineObject == null && doorSpriteRenderer != null)
        {
            outlineObject = new GameObject("DoorOutline");
            outlineObject.transform.SetParent(doorSpriteRenderer.transform);
            outlineObject.transform.localPosition = Vector3.zero;
            outlineObject.transform.localScale = Vector3.one + Vector3.one * outlineWidth;

            SpriteRenderer outlineRenderer = outlineObject.AddComponent<SpriteRenderer>();
            outlineRenderer.sprite = doorSpriteRenderer.sprite;
            outlineRenderer.color = highlightColor;
            outlineRenderer.sortingLayerName = doorSpriteRenderer.sortingLayerName;
            outlineRenderer.sortingOrder = doorSpriteRenderer.sortingOrder - 1;
        }
        else if (outlineObject != null)
        {
            outlineObject.SetActive(true);
        }
    }

    private void RemoveOutlineEffect()
    {
        if (outlineObject != null)
        {
            outlineObject.SetActive(false);
        }
    }
    
    // Create a glow material for advanced glow effect
    private void CreateGlowMaterial()
    {
        if (originalMaterial == null) return;
        
        // Create a new material instance for glow
        glowMaterial = new Material(originalMaterial);
        
        // Try to set glow properties (these depend on your shader)
        glowMaterial.SetColor("_Color", highlightColor);
        
        // For basic sprite shader, we can use color multiplication
        Color glowColor = highlightColor * glowIntensity;
        glowColor.a = originalColor.a; // Preserve alpha
        glowMaterial.SetColor("_Color", glowColor);
    }
    
    // Start advanced glow effect
    private void StartAdvancedGlow()
    {
        if (doorSpriteRenderer != null && glowMaterial != null)
        {
            doorSpriteRenderer.material = glowMaterial;
            glowTimer = 0f;
        }
    }
    
    // Stop advanced glow effect
    private void StopAdvancedGlow()
    {
        if (doorSpriteRenderer != null && originalMaterial != null)
        {
            doorSpriteRenderer.material = originalMaterial;
            doorSpriteRenderer.color = originalColor;
        }
    }
}
}
