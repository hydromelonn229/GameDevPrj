using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cainos.PixelArtTopDown_Basic;

namespace Cainos.PixelArtTopDown_Basic
{
    public class ChestController : MonoBehaviour
{
    [Header("Chest Settings")]
    public SpriteRenderer chestSpriteRenderer;
    public Sprite closedChestSprite;
    public GameObject shadowObject;
    public Sprite openChestSprite;
    [Header("Chest Visibility")]
    private bool isVisible = false;
    
    [Header("Altar Requirements")]
    public int totalAltarsRequired = 3; // Total number of altars that need to be awakened
    
    [Header("Dialogue Settings")]
    public bool showDialogueOnOpen = true; // Show dialogue when chest opens
    [TextArea(2, 4)]
    public string chestOpenDialogue = "Congratulations! You've awakened all the ancient altars and unlocked the mysterious chest!";
    public DialogueManager dialogueManager;
    
    [Header("Highlighting Settings")]
    public bool enableHighlighting = true;
    public Color highlightColor = Color.yellow;
    [Range(0.1f, 2f)]
    public float outlineWidth = 0.1f;
    
    [Header("Advanced Glow Settings")]
    public bool useAdvancedGlow = true;
    [Range(0.5f, 3f)]
    public float glowIntensity = 1.5f;
    [Range(1f, 5f)]
    public float glowPulseSpeed = 2f;
    public bool enablePulse = true;
    
    private bool isChestOpen = false;
    private bool hasShownOpenDialogue = false; // Track if chest has shown open dialogue
    
    // Highlighting system variables
    private bool isPlayerNearby = false;
    private bool isHighlighted = false;
    private GameObject outlineObject;
    private Color originalColor;
    private Material originalMaterial;
    
    // Advanced glow system variables
    private Material glowMaterial;
    private float glowTimer = 0f;

    void Start()
    {
        // Auto-detect SpriteRenderer if not assigned
        if (chestSpriteRenderer == null)
        {
            chestSpriteRenderer = GetComponent<SpriteRenderer>();
        }
        // Subscribe to altar events
        AltarManager.OnAltarAwakened += CheckAllAltarsAwakened;
        AltarManager.OnAltarDeactivated += CheckAllAltarsAwakened;
        
        // Auto-find DialogueManager if not assigned
        if (dialogueManager == null)
        {
            dialogueManager = FindObjectOfType<DialogueManager>();
        }
        
        // Initialize glow system
        if (chestSpriteRenderer != null)
        {
            originalColor = chestSpriteRenderer.color;
            originalMaterial = chestSpriteRenderer.material;
            
            if (useAdvancedGlow)
            {
                CreateGlowMaterial();
            }
        }
        
        // Ensure chest starts invisible, non-collidable, and closed
        if (chestSpriteRenderer != null)
        {
            chestSpriteRenderer.enabled = false;
            if (closedChestSprite != null)
                chestSpriteRenderer.sprite = closedChestSprite;
        }
        if (shadowObject != null)
        {
            shadowObject.SetActive(false);
        }
        
        // Disable colliders so player can walk through invisible chest
        SetCollidersEnabled(false);
        isVisible = false;
        
        Debug.Log("ChestController: Chest initialized as invisible and non-collidable");
    }
    
    void Update()
    {
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
    }

    private void CheckAllAltarsAwakened(int altarId)
    {
        // Count how many altars are currently awakened
        int awakenedCount = AltarManager.GetAwakenedAltarCount();
        // If all required altars are awakened, make chest visible
        if (awakenedCount >= totalAltarsRequired)
        {
            if (!isVisible && chestSpriteRenderer != null)
            {
                // Make chest visible and interactable
                chestSpriteRenderer.enabled = true;
                if (shadowObject != null) shadowObject.SetActive(true);
                SetCollidersEnabled(true); // Enable colliders so player can't walk through
                isVisible = true;
                Debug.Log("ChestController: Chest is now visible and collidable");
            }
        }
        else
        {
            if (isVisible && chestSpriteRenderer != null)
            {
                // Make chest invisible and non-collidable
                chestSpriteRenderer.enabled = false;
                if (shadowObject != null) shadowObject.SetActive(false);
                SetCollidersEnabled(false); // Disable colliders so player can walk through
                isVisible = false;
                Debug.Log("ChestController: Chest is now hidden and non-collidable");
            }
        }
    }

    // Method to enable/disable all colliders on the chest
    private void SetCollidersEnabled(bool enabled)
    {
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            col.enabled = enabled;
        }
        
        // Also check child objects for colliders
        Collider2D[] childColliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in childColliders)
        {
            col.enabled = enabled;
        }
        
        Debug.Log($"ChestController: Set {colliders.Length + childColliders.Length} colliders to {enabled}");
    }

    // Public method to check if chest is visible and can be interacted with
    public bool IsChestVisible()
    {
        return isVisible;
    }

    // Public method to open chest from outside (e.g., by player interaction)
    public void TryOpenChestByPlayer()
    {
        if (isVisible && !isChestOpen)
        {
            OpenChest();
        }
    }

    private void OpenChest()
    {
        isChestOpen = true;
        
        if (chestSpriteRenderer != null && openChestSprite != null)
        {
            chestSpriteRenderer.sprite = openChestSprite;
        }

        // Play chest open and glow sounds
        var chestAudio = GetComponent<ChestAudio>();
        if (chestAudio != null)
        {
            chestAudio.PlayChestOpenSequence();
        }

        // Show dialogue when chest opens (only once)
        if (showDialogueOnOpen && !hasShownOpenDialogue)
        {
            ShowChestOpenDialogue();
            hasShownOpenDialogue = true;
        }

        Debug.Log("Chest opened! All altars are awakened.");
    }
    
    // Show dialogue when chest opens
    private void ShowChestOpenDialogue()
    {
        if (dialogueManager != null)
        {
            dialogueManager.ShowDialogue(chestOpenDialogue);
            Debug.Log($"Chest opened with dialogue: {chestOpenDialogue}");
        }
        else
        {
            Debug.LogWarning("ChestController: DialogueManager not found! Cannot show chest open dialogue.");
            Debug.Log($"Chest opened: {chestOpenDialogue}");
        }
    }

    private void CloseChest()
    {
        isChestOpen = false;
        
        if (chestSpriteRenderer != null && closedChestSprite != null)
        {
            chestSpriteRenderer.sprite = closedChestSprite;
        }
        
        Debug.Log("Chest closed! Not all altars are awakened.");
    }

    // Public method to manually check chest state (useful for testing)
    public void ForceCheckChestState()
    {
        CheckAllAltarsAwakened(-1); // Pass dummy altar ID
    }
    
    // Public method to manually reset chest to invisible state (for testing)
    public void ForceHideChest()
    {
        if (chestSpriteRenderer != null)
        {
            chestSpriteRenderer.enabled = false;
        }
        if (shadowObject != null)
        {
            shadowObject.SetActive(false);
        }
        SetCollidersEnabled(false);
        isVisible = false;
        Debug.Log("ChestController: Chest manually hidden and made non-collidable");
    }

    // Public getter to check if chest is open
    public bool IsChestOpen()
    {
        return isChestOpen;
    }
    
    // Public method to set custom chest open dialogue
    public void SetChestOpenDialogue(string newDialogue)
    {
        chestOpenDialogue = newDialogue;
    }
    
    // Public method to manually trigger chest dialogue
    public void TriggerChestDialogue()
    {
        if (showDialogueOnOpen && dialogueManager != null)
        {
            dialogueManager.ShowDialogue(chestOpenDialogue);
        }
    }
    
    // Public method to reset dialogue state (for testing)
    public void ResetChestDialogueState()
    {
        hasShownOpenDialogue = false;
    }
    
    // Public method to check if chest has shown dialogue
    public bool HasShownOpenDialogue()
    {
        return hasShownOpenDialogue;
    }
    
    // Player proximity detection methods
    public void OnPlayerEnterRange()
    {
        if (!isPlayerNearby && isVisible && !isChestOpen) // Only highlight if chest is visible and not opened
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
        if (chestSpriteRenderer != null && enableHighlighting && !isHighlighted)
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
        if (outlineObject == null)
        {
            outlineObject = new GameObject("ChestOutline");
            outlineObject.transform.SetParent(transform);
            outlineObject.transform.localPosition = Vector3.zero;
            outlineObject.transform.localScale = Vector3.one + Vector3.one * outlineWidth;

            SpriteRenderer outlineRenderer = outlineObject.AddComponent<SpriteRenderer>();
            outlineRenderer.sprite = chestSpriteRenderer.sprite;
            outlineRenderer.color = highlightColor;
            outlineRenderer.sortingLayerName = chestSpriteRenderer.sortingLayerName;
            outlineRenderer.sortingOrder = chestSpriteRenderer.sortingOrder - 1;
        }
        else
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
        if (chestSpriteRenderer != null && glowMaterial != null)
        {
            chestSpriteRenderer.material = glowMaterial;
            glowTimer = 0f;
        }
    }
    
    // Stop advanced glow effect
    private void StopAdvancedGlow()
    {
        if (chestSpriteRenderer != null && originalMaterial != null)
        {
            chestSpriteRenderer.material = originalMaterial;
            chestSpriteRenderer.color = originalColor;
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        AltarManager.OnAltarAwakened -= CheckAllAltarsAwakened;
        AltarManager.OnAltarDeactivated -= CheckAllAltarsAwakened;
        
        // Clean up any ongoing highlighting
        StopHighlighting();
        
        // Clean up outline object
        if (outlineObject != null)
        {
            DestroyImmediate(outlineObject);
        }
        
        // Clean up glow material
        if (glowMaterial != null)
        {
            DestroyImmediate(glowMaterial);
        }
    }
}
}