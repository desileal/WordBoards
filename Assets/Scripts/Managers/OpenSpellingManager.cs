using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using UnityEngine;

public class OpenSpellingManager : MonoBehaviour
{
    // TODO: is this a list of transforms or game objects?
    [SerializeField]
    private List<Transform> blockPositions = new();

    [SerializeField] 
    private List<string> letters = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

}
