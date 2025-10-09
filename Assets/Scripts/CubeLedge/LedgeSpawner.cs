using System.Collections.Generic;
using UnityEngine;

public class LedgeSpawner : MonoBehaviour
{
    public List<GameObject> spawnedLedges = new();
    public Transform centralSpawnLocation;
    public float portalSpacing = 0.5f;

    private readonly Dictionary<string, Ledge> _ledges = new();
    

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

    public bool TryGet(string id, out Ledge l) => _ledges.TryGetValue(id, out l);
}
