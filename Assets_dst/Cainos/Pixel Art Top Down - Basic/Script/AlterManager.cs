using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltarManager : MonoBehaviour
{
    // Events that other scripts can subscribe to
    public static System.Action<int> OnAltarAwakened;
    public static System.Action<int> OnAltarDeactivated;
    
    [Header("Altar System Settings")]
    public static Dictionary<int, bool> altarStates = new Dictionary<int, bool>();
    public static Dictionary<int, bool> altarCountsInChain = new Dictionary<int, bool>(); // Track which altars count for dialogue chain
    
    /// <summary>
    /// Call this method when an altar becomes awakened
    /// </summary>
    /// <param name="altarId">The ID of the altar that was awakened</param>
    /// <param name="countsInChain">Whether this altar should be counted in the dialogue chain (false for fixed dialogue altars)</param>
    public static void AwakenAltar(int altarId, bool countsInChain = true)
    {
        if (!altarStates.ContainsKey(altarId))
        {
            altarStates[altarId] = false;
        }
        
        // Track whether this altar counts in the chain
        altarCountsInChain[altarId] = countsInChain;
        
        if (!altarStates[altarId])
        {
            altarStates[altarId] = true;
            OnAltarAwakened?.Invoke(altarId);
            Debug.Log($"Altar {altarId} has been awakened! (Counts in chain: {countsInChain})");
        }
    }
    
    /// <summary>
    /// Backward compatibility method - counts in chain by default
    /// </summary>
    /// <param name="altarId">The ID of the altar that was awakened</param>
    public static void AwakenAltar(int altarId)
    {
        AwakenAltar(altarId, true);
    }
    
    /// <summary>
    /// Call this method when an altar becomes deactivated
    /// </summary>
    /// <param name="altarId">The ID of the altar that was deactivated</param>
    /// <param name="countsInChain">Whether this altar should be counted in the dialogue chain</param>
    public static void DeactivateAltar(int altarId, bool countsInChain = true)
    {
        if (!altarStates.ContainsKey(altarId))
        {
            altarStates[altarId] = false;
        }
        
        // Track whether this altar counts in the chain
        altarCountsInChain[altarId] = countsInChain;
        
        if (altarStates[altarId])
        {
            altarStates[altarId] = false;
            OnAltarDeactivated?.Invoke(altarId);
            Debug.Log($"Altar {altarId} has been deactivated! (Counts in chain: {countsInChain})");
        }
    }
    
    /// <summary>
    /// Backward compatibility method - counts in chain by default
    /// </summary>
    /// <param name="altarId">The ID of the altar that was deactivated</param>
    public static void DeactivateAltar(int altarId)
    {
        DeactivateAltar(altarId, true);
    }
    
    /// <summary>
    /// Check if a specific altar is currently awakened
    /// </summary>
    /// <param name="altarId">The ID of the altar to check</param>
    /// <returns>True if the altar is awakened, false otherwise</returns>
    public static bool IsAltarAwakened(int altarId)
    {
        return altarStates.ContainsKey(altarId) && altarStates[altarId];
    }
    
    /// <summary>
    /// Get the number of currently awakened altars that count in the dialogue chain
    /// </summary>
    /// <returns>The count of awakened altars that are part of the dialogue chain</returns>
    public static int GetAwakenedAltarCount()
    {
        int count = 0;
        foreach (var kvp in altarStates)
        {
            int altarId = kvp.Key;
            bool isAwakened = kvp.Value;
            
            // Only count altars that are awakened AND count in the chain
            if (isAwakened && altarCountsInChain.ContainsKey(altarId) && altarCountsInChain[altarId])
            {
                count++;
            }
        }
        return count;
    }
    
    /// <summary>
    /// Get the total number of awakened altars (including fixed dialogue ones)
    /// </summary>
    /// <returns>The total count of all awakened altars</returns>
    public static int GetTotalAwakenedAltarCount()
    {
        int count = 0;
        foreach (var state in altarStates.Values)
        {
            if (state) count++;
        }
        return count;
    }
    
    /// <summary>
    /// Check if all registered altars are awakened
    /// </summary>
    /// <returns>True if all altars are awakened, false otherwise</returns>
    public static bool AreAllAltarsAwakened()
    {
        foreach (var state in altarStates.Values)
        {
            if (!state) return false;
        }
        return altarStates.Count > 0;
    }
}