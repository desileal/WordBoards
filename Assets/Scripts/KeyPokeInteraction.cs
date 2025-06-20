using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Events;
using Meta.XR.BuildingBlocks;

/// <summary>
/// 
/// </summary>
public class KeyPokeInteraction : MonoBehaviour
{
    [SerializeField]
    private string keyValue;

    public LetterboardManager letterboardManager;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        letterboardManager = GetComponent<LetterboardManager>();
    }

    public void KeyPoke(string input)
    {
        
    }
}
