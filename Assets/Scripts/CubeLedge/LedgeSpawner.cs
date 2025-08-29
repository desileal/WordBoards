using System.Collections.Generic;
using UnityEngine;

public class LedgeSpawner : MonoBehaviour
{
    public List<GameObject> spawnedLedges = new();
    public Transform centralSpawnLocation;
    public float portalSpacing = 0.5f;
    

    public void SpawnLedges(string dictationWord)
    {
        if (dictationWord == null)
        {
            Debug.LogError("No dictation word available for spelling task.");
            return;
        }

        foreach (char c in dictationWord)
        {
            // create new ledge and set string ledgeId to c to match the cube id
        }
    }

    public void ClearLedges()
    {
        foreach (var ledge in spawnedLedges)
        {
            Destroy(ledge);
        }
        spawnedLedges.Clear();
        LedgeManager.Instance.ClearLedges();
        // CentralEventSystem.Instance.InvokeOnLedgesCleared();
    }
}
