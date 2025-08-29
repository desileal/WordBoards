using System.Collections.Generic;
using UnityEngine;

public class WordLedgeManager : MonoBehaviour
{
    public static WordLedgeManager Instance;
    public Transform ledgeSlotParent;

    private List<string> currentSentence = new List<string>();

    private void Awake()
    {
        Instance = this;
    }

    public void AddToLedge(CubeInteraction cube)
    {
        
        // Move the block to next available ledge slot
        // Update UI or display
    }
}
