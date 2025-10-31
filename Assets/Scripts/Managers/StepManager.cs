using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using NUnit.Framework;
using Unity.XR.CoreUtils;
using UnityEngine;

public class StepManager : MonoBehaviour
{
    // contains full word and tracks within that word
    // next step is next word, next task is next letter
    [SerializeField] public GameObject cubeInteractionPrefab;
    [SerializeField] public GameObject ledgePrefab;

    private SessionManager sessionManager;

    // Word that the user is spelling
    private string _stepWord { get; set; }
    // array that is stepped through with each correct letter selected
    private string[] _wordLetters { get; set; }
    // tracks the index of the next letter to be spelled in the _wordLetters array
    private int _currentLetterIndex { get; set; }
    // Current letters that have been spelled
    private string[] _lettersSpelled;
    // Next letter that will invoke correct collision
    private string _nextLetter;

    // position where interaction block game objects will spawn from
    private Vector3 interactionBlocksAnchor;
    // position where ledge game objects will spawn from
    private Vector3 ledgesAnchor;
    [SerializeField] private float blockOffset;

    // individual CubeInteraction classes for each letter
    private List<CubeInteraction> interactionBlocks = new();
    // individual game objects for each cube in the scene that has a CubeInteraction script attached
    private List<GameObject> interactionGameObjects = new();
    // individual Ledge classes for each letter, to be matched with the CubeInteraction game objects
    private List<Ledge> blockLedges = new();
    // individual game objects for each ledge in the scene
    private List<GameObject> ledgeGameObjects = new();

    CentralEventSystem CES;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CES = CentralEventSystem.Instance;

        if (CES != null)
        {
            CES.OnPlayerLetterSelection += UpdateLettersSpelled;
            CES.OnNextStep += DestroyInteractionBlocks;
            CES.OnNextStep += DestroyBlockLedges;
            CES.OnSetStepWord += SetStepTask;
            CES.OnSetCubeSpawnAnchor += SetInteractionBlocksAnchor;
            CES.OnSetLedgeSpawnAnchor += SetLedgesAnchor;
        }

        sessionManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<SessionManager>();
    }

    private void SetInteractionBlocksAnchor(Vector3 anchor)
    {
        interactionBlocksAnchor = anchor;
    }

    private void SetLedgesAnchor(Vector3 anchor)
    {
        ledgesAnchor = anchor;
    }

    private void SetStepTask(string stepWord)
    {
        _stepWord = stepWord;
        _wordLetters = new[] { _stepWord };
        _currentLetterIndex = 0;
        _lettersSpelled = new string[_wordLetters.Length];
        _nextLetter = _wordLetters[_currentLetterIndex];
        SpawnLedgeObjects();
        SpawnInteractionBlockObjects();
    }

    // initiate interaction blocks
    // randomize order
    // add extra letters
    // display word to be spelled
    private void DestroyInteractionBlocks()
    {
        foreach (CubeInteraction block in interactionBlocks)
        {
            Destroy(block);
        }
        foreach(GameObject block in interactionGameObjects)
        {
            Destroy(block);
        }
    }

    private void DestroyBlockLedges()
    {
        foreach(Ledge ledge in blockLedges)
        {
            Destroy(ledge);
        }
        foreach(GameObject ledge in ledgeGameObjects)
        {
            Destroy(ledge);
        }
    }

    private void ClearInteractionBlocksLists()
    {
        interactionBlocks.Clear();
        interactionGameObjects.Clear();
    }

    private void ClearBlockLedgesLists()
    {
        blockLedges.Clear();
        ledgeGameObjects.Clear();
    }


    private void SpawnLedgeObjects ()
    {
        ClearBlockLedgesLists();

        int i = 0;
        float startXPos = CalculateStartXPos();
        foreach (string letter in _wordLetters)
        {
            // spawn game object
            float localXPos = startXPos + (i * blockOffset);
            GameObject ledge = Instantiate(ledgePrefab, new Vector3(localXPos, ledgesAnchor.y, ledgesAnchor.z), Quaternion.identity);

            ledge.GetComponent<Ledge>().targetID = letter;
            // add game objects to list
            ledgeGameObjects.Add(ledge);

            i++;
        }
    }

    // TODO
    // make shuffled array then spawn interaction blocks from that out of order
    private void SpawnInteractionBlockObjects()
    {
        // clear list before adding to it
        ClearInteractionBlocksLists();

        int i = 0;
        float startXPos = CalculateStartXPos();
        string[] shuffledLetters = ShuffleArray();
        foreach (string letter in shuffledLetters)
        {
            // spawn game object at local position
            float localXPos = startXPos + (i * blockOffset);
            GameObject cube = Instantiate(cubeInteractionPrefab, new Vector3(localXPos, interactionBlocksAnchor.y, interactionBlocksAnchor.z), Quaternion.identity);
            
            // TODO - use a constructor and make new CubeInteraction ?
            cube.GetComponent<CubeInteraction>().targetID = letter;
            cube.GetComponent<CubeInteraction>().letterText.SetText(letter);
            cube.GetComponent<CubeInteraction>().interactionType = sessionManager.currentInteractionType;
            // add game objects to list
            interactionGameObjects.Add(cube);
            
            i++;
        }
    }

    // randomize order of blocks that is displayed in UI
    // add more letters/random blocks through phase progression
    private string[] ShuffleArray()
    {
        string[] shuffled = _wordLetters ;
        for (int t = 0; t < shuffled.Length; t++)
        {
            string tmp = shuffled[t];
            int r = Random.Range(t, shuffled.Length);
            shuffled[t] = shuffled[r];
            shuffled[r] = tmp;
        }
        return shuffled;
    }

    // add cube interaction scripts to list
    private void ReferenceSpawnedObjectScripts()
    {

    }


    // verify if cube poked or grab collision matches the next letter in the task


    //  Updates the Task with the letters that have been spelled
    private void UpdateLettersSpelled(string s)
    {
        _lettersSpelled.Append(s);
        _currentLetterIndex++;
        if (_currentLetterIndex <= _lettersSpelled.Length)
        {
            _nextLetter = _wordLetters[_currentLetterIndex];
            CES.InvokeOnNextStepTask(_nextLetter);
        }
        else
        {
            CES.InvokeOnStepComplete();
        }
    }

    private float CalculateStartXPos()
    {
        if (_wordLetters.Length <= 1)
            return 0;

        float x = (float)(ledgesAnchor.x - (0.5*_wordLetters.Length*blockOffset));
        return x;
    }

    

}
