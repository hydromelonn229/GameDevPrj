using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowPillar : MonoBehaviour
{
    [Header("Glow Settings")]
    public List<GameObject> glowObjects;
    
    [Header("Altar Connection")]
    public int pillarId = 0;
    
    private List<Cainos.PixelArtTopDown_Basic.SpriteColorAnimation> spriteColorAnimations;
    private bool isGlowing = false;

    void Start()
    {
        // Get all SpriteColorAnimation components from glow objects
        spriteColorAnimations = new List<Cainos.PixelArtTopDown_Basic.SpriteColorAnimation>();
        
        foreach (var glowObj in glowObjects)
        {
            if (glowObj != null)
            {
                var animation = glowObj.GetComponent<Cainos.PixelArtTopDown_Basic.SpriteColorAnimation>();
                if (animation != null)
                {
                    spriteColorAnimations.Add(animation);
                    // Initially disable the animation
                    animation.enabled = false;
                }
                
                // Also disable the glow GameObject itself initially
                glowObj.SetActive(false);
            }
        }
        
        // Subscribe to altar events
        AltarManager.OnAltarAwakened += OnAltarAwakened;
        AltarManager.OnAltarDeactivated += OnAltarDeactivated;
    }

    private void OnAltarAwakened(int altarId)
    {
        if (altarId == pillarId)
        {
            ActivateGlow();
        }
    }
    
    private void OnAltarDeactivated(int altarId)
    {
        if (altarId == pillarId)
        {
            DeactivateGlow();
        }
    }
    
    public void ActivateGlow()
    {
        isGlowing = true;
        
        // Enable glow objects and their animations
        foreach (var glowObj in glowObjects)
        {
            if (glowObj != null)
            {
                // Enable the GameObject first
                glowObj.SetActive(true);
                
                // Then enable the animation
                var animation = glowObj.GetComponent<Cainos.PixelArtTopDown_Basic.SpriteColorAnimation>();
                if (animation != null)
                {
                    animation.enabled = true;
                }
            }
        }
    }
    
    public void DeactivateGlow()
    {
        isGlowing = false;
        
        // Disable glow objects and their animations
        foreach (var glowObj in glowObjects)
        {
            if (glowObj != null)
            {
                // Disable the animation first
                var animation = glowObj.GetComponent<Cainos.PixelArtTopDown_Basic.SpriteColorAnimation>();
                if (animation != null)
                {
                    animation.enabled = false;
                }
                
                // Then disable the GameObject
                glowObj.SetActive(false);
            }
        }
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        AltarManager.OnAltarAwakened -= OnAltarAwakened;
        AltarManager.OnAltarDeactivated -= OnAltarDeactivated;
    }
}
