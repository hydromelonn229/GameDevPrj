using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cainos.PixelArtTopDown_Basic
{
    public class SignController : MonoBehaviour
    {
        [Header("Sign Settings")]
        public SpriteRenderer signSpriteRenderer;
        public string dialogueText = "Welcome to the adventure! Press F to interact with objects.";
        
        [Header("Highlighting")]
        public bool enableHighlighting = true;
        public Color highlightColor = Color.yellow;
        public float outlineWidth = 0.1f;
        public Material outlineMaterial; // Optional: assign an outline material
        
        [Header("Advanced Glow Settings")]
        public bool useAdvancedGlow = true;
        public float glowIntensity = 2.0f;
        public float glowPulseSpeed = 3.0f;
        public bool enablePulse = true;
        
        private Color originalColor;
        private bool isHighlighted = false;
        private bool isPlayerNearby = false;
        private GameObject outlineObject;
        private SpriteRenderer outlineRenderer;
        
        // Advanced glow variables
        private Material glowMaterial;
        private Material originalMaterial;
        private float glowTimer = 0f;
        
        [Header("Dialogue Manager Reference")]
        public DialogueManager dialogueManager;

        void Start()
        {
            // Auto-detect SpriteRenderer if not assigned
            if (signSpriteRenderer == null)
            {
                signSpriteRenderer = GetComponent<SpriteRenderer>();
            }
            
            // Store original color and material for highlighting
            if (signSpriteRenderer != null)
            {
                originalColor = signSpriteRenderer.color;
                originalMaterial = signSpriteRenderer.material;
                
                // Create glow material if using advanced glow
                if (useAdvancedGlow)
                {
                    CreateGlowMaterial();
                }
            }
            
            // Find DialogueManager if not assigned
            if (dialogueManager == null)
            {
                dialogueManager = FindObjectOfType<DialogueManager>();
            }
        }

        // Public method to check if sign can be interacted with
        public bool IsSignInteractable()
        {
            return true; // Signs are always interactable (unlike chests that need altar conditions)
        }

        // Public method to interact with sign from outside (e.g., by player interaction)
        public void TryInteractWithSign()
        {
            if (IsSignInteractable())
            {
                ShowDialogue();
            }
        }

        // Called when player enters interaction range
        public void OnPlayerEnterRange()
        {
            isPlayerNearby = true;
            if (enableHighlighting && !isHighlighted)
            {
                StartHighlighting();
            }
        }

        // Called when player exits interaction range
        public void OnPlayerExitRange()
        {
            isPlayerNearby = false;
            if (isHighlighted)
            {
                StopHighlighting();
            }
        }

        private void StartHighlighting()
        {
            if (signSpriteRenderer != null && enableHighlighting && !isHighlighted)
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
            }
        }

        private void CreateOutlineEffect()
        {
            if (outlineObject == null)
            {
                // Create a duplicate sprite for the outline effect
                outlineObject = new GameObject("SignOutline");
                outlineObject.transform.SetParent(transform);
                outlineObject.transform.localPosition = Vector3.zero;
                outlineObject.transform.localScale = Vector3.one * (1f + outlineWidth);
                
                outlineRenderer = outlineObject.AddComponent<SpriteRenderer>();
                outlineRenderer.sprite = signSpriteRenderer.sprite;
                outlineRenderer.color = highlightColor;
                outlineRenderer.sortingOrder = signSpriteRenderer.sortingOrder - 1;
                
                // Apply outline material if available
                if (outlineMaterial != null)
                {
                    outlineRenderer.material = outlineMaterial;
                }
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

        private void ShowDialogue()
        {
            if (dialogueManager != null)
            {
                dialogueManager.ShowDialogue(dialogueText);
                Debug.Log($"Sign interaction: {dialogueText}");
            }
            else
            {
                Debug.LogWarning("DialogueManager not found! Please assign one or ensure it exists in the scene.");
                Debug.Log($"Sign says: {dialogueText}");
            }
        }

        // Public getter for dialogue text (useful for editor or external scripts)
        public string GetDialogueText()
        {
            return dialogueText;
        }

        // Public method to change dialogue text at runtime
        public void SetDialogueText(string newText)
        {
            dialogueText = newText;
        }

        // Public method to check if player is nearby (for debugging)
        public bool IsPlayerNearby()
        {
            return isPlayerNearby;
        }

        // Create a glow material for advanced glow effect
        private void CreateGlowMaterial()
        {
            if (originalMaterial == null) return;
            
            // Create a new material instance for glow
            glowMaterial = new Material(originalMaterial);
            
            // Try to set glow properties (these depend on your shader)
            glowMaterial.SetColor("_Color", highlightColor);
            
            // If you have a specific glow shader, uncomment and modify these:
            // glowMaterial.SetFloat("_GlowIntensity", glowIntensity);
            // glowMaterial.SetColor("_GlowColor", highlightColor);
            
            // For basic sprite shader, we can use color multiplication
            Color glowColor = highlightColor * glowIntensity;
            glowColor.a = originalColor.a; // Preserve alpha
            glowMaterial.SetColor("_Color", glowColor);
        }
        
        // Start advanced glow effect
        private void StartAdvancedGlow()
        {
            if (signSpriteRenderer != null && glowMaterial != null)
            {
                signSpriteRenderer.material = glowMaterial;
                glowTimer = 0f;
            }
        }
        
        // Stop advanced glow effect
        private void StopAdvancedGlow()
        {
            if (signSpriteRenderer != null && originalMaterial != null)
            {
                signSpriteRenderer.material = originalMaterial;
                signSpriteRenderer.color = originalColor;
            }
        }

        void OnDestroy()
        {
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
