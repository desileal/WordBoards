using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;

public class MarkPokeColliders : MonoBehaviour
{
    public string tagName = "PokeInput";         // optional

    public bool includeControllers = true;
    public float scanInterval = 0.25f;   // how often to look
    public int scanBursts = 20;      // how many times at startup

    private int _layer = -1;
    private readonly HashSet<Collider> _marked = new();

    void Awake()
    {
        StartCoroutine(StartupScan());
    }

    IEnumerator StartupScan()
    {
        for (int i = 0; i < scanBursts; i++)
        {
            ScanOnce();
            yield return new WaitForSeconds(scanInterval);
        }
    }

    void Update()
    {
        // lightweight keep-alive: if hands drop/reacquire tracking later, new capsules get marked
        if (Time.frameCount % 60 == 0) ScanOnce(); // ~1x/sec
    }

    void ScanOnce()
    {
        foreach (var hand in FindObjectsByType<Hand>(FindObjectsSortMode.None))
            MarkUnder(hand.transform);

        foreach (var ovrHand in FindObjectsByType<OVRHand>(FindObjectsSortMode.None))
            MarkUnder(ovrHand.transform);

        if (includeControllers)
            foreach (var helper in FindObjectsByType<OVRControllerHelper>(FindObjectsSortMode.None))
                MarkUnder(helper.transform);
    }

    void MarkUnder(Transform root)
    {
        // prioritize the “Capsules” child if it exists (matches your hierarchy)
        var capsules = root.Find("Capsules");
        var t = capsules ? capsules : root;

        foreach (var col in t.GetComponentsInChildren<Collider>(true))
        {
            if (_marked.Contains(col)) continue;
            if (_layer >= 0) col.gameObject.layer = _layer;
            if (!string.IsNullOrEmpty(tagName)) col.gameObject.tag = tagName;
            _marked.Add(col);
        }
    }
}
