using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyOverlay : MonoBehaviour
{
    public GameObject keyImage; // Assign the key image GameObject in Inspector
    public Color fadeColor = new Color(0, 0, 0, 0.7f); // Dark overlay with transparency
    
    private GameObject fadedBackground;
    private GameObject inventoryKeyImage; // Key in top-right corner
    private Canvas canvas;
    private bool isWaitingForPlayerInput = false; // Track if overlay is waiting for F key

    void Awake()
    {
        // Create the canvas if it doesn't exist
        canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        // Create faded background automatically
        CreateFadedBackground();
        CreateInventoryKey();
        HideOverlay();
        HideInventoryKey();
    }

    void Update()
    {
        // Check for F key to dismiss overlay
        if (isWaitingForPlayerInput && Input.GetKeyDown(KeyCode.F))
        {
            DismissOverlay();
        }
    }

    void CreateFadedBackground()
    {
        fadedBackground = new GameObject("FadedBackground");
        fadedBackground.transform.SetParent(canvas.transform, false);
        
        // Add Image component for the fade effect
        Image fadeImage = fadedBackground.AddComponent<Image>();
        fadeImage.color = fadeColor;
        
        // Make it cover the entire screen
        RectTransform rectTransform = fadedBackground.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        
        // Set sorting order to be behind the key
        fadedBackground.transform.SetSiblingIndex(0);
    }

    void CreateInventoryKey()
    {
        if (keyImage == null) return;
        
        // Create inventory key image in top-right corner
        inventoryKeyImage = new GameObject("InventoryKey");
        inventoryKeyImage.transform.SetParent(canvas.transform, false);
        
        // Copy the key image sprite
        Image keyImageComponent = keyImage.GetComponent<Image>();
        if (keyImageComponent != null)
        {
            Image inventoryImage = inventoryKeyImage.AddComponent<Image>();
            inventoryImage.sprite = keyImageComponent.sprite;
            inventoryImage.color = keyImageComponent.color;
        }
        
        // Position in top-right corner
        RectTransform rectTransform = inventoryKeyImage.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(1, 1); // Top-right anchor
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.pivot = new Vector2(1, 1);
        rectTransform.anchoredPosition = new Vector2(-20, -20); // 20 pixels from edges
        rectTransform.sizeDelta = new Vector2(64, 64); // Small size for inventory
    }

    // Call to show the overlay
    public void ShowOverlay()
    {
        if (fadedBackground != null) fadedBackground.SetActive(true);
        if (keyImage != null) keyImage.SetActive(true);
    }

    // Call to show overlay and wait for player input
    public void ShowOverlayWaitForInput()
    {
        ShowOverlay();
        isWaitingForPlayerInput = true;
    }

    // Call to hide the overlay
    public void HideOverlay()
    {
        if (fadedBackground != null) fadedBackground.SetActive(false);
        if (keyImage != null) keyImage.SetActive(false);
        isWaitingForPlayerInput = false;
    }

    // Dismiss overlay and show inventory key
    public void DismissOverlay()
    {
        HideOverlay();
        ShowInventoryKey();
        ChestAudio.OnKeyObtained(); // Notify that key was obtained
    }

    // Show key in inventory (top-right corner)
    public void ShowInventoryKey()
    {
        if (inventoryKeyImage != null) inventoryKeyImage.SetActive(true);
    }

    // Hide key from inventory
    public void HideInventoryKey()
    {
        if (inventoryKeyImage != null) inventoryKeyImage.SetActive(false);
    }

    // Check if inventory key is visible
    public bool IsInventoryKeyVisible()
    {
        return inventoryKeyImage != null && inventoryKeyImage.activeSelf;
    }
}
