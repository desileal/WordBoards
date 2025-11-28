using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using NUnit.Framework;
using OVR.OpenVR;
using TMPro;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

/// <summary>
/// 
/// </summary>
/// TODO: add extra blocks per word as the system progresses 
/// Display the word being spelled or prompt it at the beginning of the step?
public class StepManager : MonoBehaviour
{
    
    [SerializeField] public GameObject pokeBlockPrefab;
    [SerializeField] public GameObject grabBlockPrefab;
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
    private string _currentCorrectLetter;
    // index in ledgeGameObjects of the ledge that was last collided with
    private int _lastCollidedLedge = 0;

    // position where interaction block game objects will spawn from
    private Vector3 interactionBlocksAnchor;
    // position where ledge game objects will spawn from
    private Vector3 ledgesAnchor;
    // rotation that all the objects will be set to
    private Quaternion objectsRotation;
    // distance between each block and ledge
    [SerializeField] private float blockOffset;

    // individual game objects for each cube in the scene that has a CubeInteraction script attached
    private List<GameObject> interactionGameObjects = new();
    // individual game objects for each ledge in the scene
    private List<GameObject> ledgeGameObjects = new();

    // How fast users are allowed to poke (seconds between accepted pokes)
    [SerializeField] private float pokeCooldown = 0.15f;
    private float _nextAllowedPokeTime = 0f;

    CentralEventSystem CES;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CES = CentralEventSystem.Instance;

        if (CES != null)
        {
            CES.OnPlayerCubePoke += CheckPlayerPokeSelection;
            CES.OnPlayerGrabRelease += CheckPlayerGrabSelection;
            CES.OnNextStep += DestroyInteractionBlocks;
            CES.OnNextStep += DestroyBlockLedges;
            CES.OnSetStepWord += SetStepParameters;
            CES.OnSetCubeSpawnAnchor += SetInteractionBlocksAnchor;
            CES.OnSetLedgeSpawnAnchor += SetLedgesAnchor;
            CES.OnSetRotationAnchor += SetObjectRotation;
            CES.OnLedgeCollision += UpdateLastCollidedLedge;
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

    private void SetObjectRotation(Quaternion rotation)
    {
        objectsRotation = rotation;
    }

    private void SetStepParameters(string stepWord)
    {
        Debug.Log($"***** Setting parameters for stepWord {stepWord}");
        _stepWord = stepWord;
        _wordLetters = new string[_stepWord.Length];
        for(int i = 0; i < _stepWord.Length; i++)
        {
            _wordLetters[i] = stepWord[i].ToString();
            //Debug.Log($"***** Appended {c} to _wordLetters *****");
        }
        _currentLetterIndex = 0;
        _lettersSpelled = new string[_wordLetters.Length];
        _currentCorrectLetter = _wordLetters[_currentLetterIndex];
        SpawnLedgeObjects();
        SpawnInteractionBlockObjects();
    }

    private void DestroyInteractionBlocks()
    {
        foreach(GameObject block in interactionGameObjects)
        {
            Destroy(block);
        }
    }

    private void DestroyBlockLedges()
    {
        foreach(GameObject ledge in ledgeGameObjects)
        {
            Destroy(ledge);
        }
    }

    private void ClearInteractionBlocksLists()
    {
        interactionGameObjects.Clear();
    }

    private void ClearBlockLedgesLists()
    {
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

            ledge.GetComponent<Ledge>().letterIndex = i;
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
            // blocks are defined depending on Interaction from SessionManager
            GameObject block = InstantiateInteractiveBlock(letter, localXPos, i);
                        
            // add game objects to list
            interactionGameObjects.Add(block);
            
            i++;
        }
    }

    /// <summary>
    /// Instantiates each block depending on the current InteractionType of the system
    /// </summary>
    /// <param name="letter"> letter displayed on the block </param>
    /// <param name="xPos"> x position where the block is spawned </param>
    /// <param name="index"> the element index where the block is in the list of interactionGameObjects </param>
    /// <returns></returns>
    private GameObject InstantiateInteractiveBlock(string letter, float xPos, int index)
    {
        GameObject cube = new();
        switch(sessionManager.currentInteractionType)
        {
            case(InteractionType.Poke):
                cube = Instantiate(pokeBlockPrefab, new Vector3(xPos, interactionBlocksAnchor.y, interactionBlocksAnchor.z), Quaternion.identity);
                cube.GetComponent<PokeBlock>().targetID = letter;
                cube.GetComponent<PokeBlock>().letterText.text = letter;
                cube.GetComponent<PokeBlock>().listIndex = index;
                break;
            case (InteractionType.Grab):
                cube = Instantiate(grabBlockPrefab, new Vector3(xPos, interactionBlocksAnchor.y, interactionBlocksAnchor.z), Quaternion.identity);
                cube.GetComponent<GrabBlock>().targetID = letter;
                cube.GetComponent<GrabBlock>().letterText.text = letter;
                cube.GetComponent <GrabBlock>().listIndex = index;
                break;
        }
        return cube;
    }

    // randomize order of letters in current step
    // add more letters/random blocks through phase progression
    private string[] ShuffleArray()
    {
        string[] shuffled = new string[_wordLetters.Length];
        for (int i = 0; i < _wordLetters.Length; i++)
        {
            shuffled[i] = _wordLetters[i];
        }
        for (int t = 0; t < shuffled.Length; t++)
        {
            string tmp = shuffled[t];
            int r = Random.Range(t, shuffled.Length);
            shuffled[t] = shuffled[r];
            shuffled[r] = tmp;
        }
        return shuffled;
    }

    /// <summary>
    /// Updates the index of the ledge that a grab block last collided with. Used to track if user made correct 
    /// placement of the block for the current letter being spelled.
    /// </summary>
    /// <param name="i"> index of the ledge that grab block last collided with </param>
    private void UpdateLastCollidedLedge (int i)
    {
        _lastCollidedLedge = i;
    }

    /// <summary>
    /// verify if cube poked or grab collision matches the next letter in the task
    /// </summary>
    /// <param name="letter"> letter selected by user (poke or grab) </param>
    /// <param name="i"> listIndex where the interaction block is in interactionGameObjects </param>
    private void CheckPlayerPokeSelection(string letter, int i)
    {
        // If we somehow get a poke after finishing the word, just ignore it
        if (_wordLetters == null || _currentLetterIndex >= _wordLetters.Length)
        {
            Debug.Log("Received poke but step is already complete. Ignoring.");
            return;
        }

        // Global cooldown so one physical poke can't trigger multiple letters
        if (Time.time < _nextAllowedPokeTime)
        {
            Debug.Log("Poke ignored due to cooldown.");
            return;
        }

        if (letter != _currentCorrectLetter)
        {
            Debug.Log($"Incorrect letter, {letter}, selected for next letter, {_currentCorrectLetter}");
        }
        else
        {
            // Set cooldown for the next poke
            _nextAllowedPokeTime = Time.time + pokeCooldown;

            // Safety: make sure indices are valid before indexing into lists
            if (i < 0 || i >= interactionGameObjects.Count)
            {
                Debug.LogError(
                    $"Poke cube index {i} is out of range. interactionGameObjects.Count = {interactionGameObjects.Count}");
                return;
            }

            if (_currentLetterIndex < 0 || _currentLetterIndex >= ledgeGameObjects.Count)
            {
                Debug.LogError(
                    $"_currentLetterIndex {_currentLetterIndex} is out of range. ledgeGameObjects.Count = {ledgeGameObjects.Count}");
                return;
            }

            Debug.Log($"***** Snapping letter {letter} to ledge at index {i} *****");
            // invoke on snap letter to ledge with ledge transform at current LetterIndex
            //Transform ledgeTransform = ledgeGameObjects.ElementAt(_currentLetterIndex).GetComponent<Transform>();
            Transform ledgeTransform = ledgeGameObjects[_currentLetterIndex].transform;
            // TODO - will this be invoked on ALL blocks?
            // OR don't invoke an event but call the method specific to the GrabBlock
            interactionGameObjects[i].GetComponent<PokeBlock>().SnapToLedge(ledgeTransform);
            //CES.InvokeOnSnapBlockToLedge(ledgeTransform);
            UpdateLettersSpelled(letter);
        }
    }

    // TODO - clean this up
    /// <summary>
    /// 
    /// </summary>
    /// <param name="letter"></param>
    /// <param name="cubeListIndex"></param>
    /// <param name="ledgeIndex"></param>
    private void CheckPlayerGrabSelection(string letter, int cubeListIndex)
    {
        if (letter != _currentCorrectLetter)
        {
            Debug.Log($"Incorrect letter, {letter}, selected for next letter, {_currentCorrectLetter}");
            Debug.Log($"Calling ReturnToStartPosition for GrabBlock of letter {letter} at index {cubeListIndex}.");
            // call reset to home with transform from interaction block at _currentLetterIndex
            interactionGameObjects.ElementAt(cubeListIndex).GetComponent<GrabBlock>().ReturnToStartPosition();
        }
        // TODO - is there a way to yield of the ledge index has not been updated yet?
        // should be updated before block is released
        else if (_lastCollidedLedge != _currentLetterIndex)
        {
            Debug.Log($"Grab block for letter, {letter}, placed on wrong ledge.");
            Debug.Log($"Calling ReturnToStartPosition for GrabBlock of letter {letter} at index {cubeListIndex}.");
            // call reset to home with transform from interaction block at _currentLetterIndex
            interactionGameObjects.ElementAt(cubeListIndex).GetComponent<GrabBlock>().ReturnToStartPosition();
        }
        else
        {
            Debug.Log($"***** Snapping letter {letter} to ledge at index {cubeListIndex} *****");
            // invoke on snap letter to ledge with ledge transform at current LetterIndex
            Transform ledgeTransform = ledgeGameObjects.ElementAt(_lastCollidedLedge).GetComponent<Transform>();
            // TODO - will this be invoked on ALL blocks?
            // OR don't invoke an event but call the method specific to the GrabBlock
            interactionGameObjects.ElementAt(cubeListIndex).GetComponent<GrabBlock>().SnapToLedge(ledgeTransform);
            //CES.InvokeOnSnapBlockToLedge(ledgeTransform);
            UpdateLettersSpelled(letter);
        }
    }

    //  Updates the Task with the letters that have been spelled
    private void UpdateLettersSpelled(string s)
    {
        if (_lettersSpelled != null && _currentLetterIndex >= 0 && _currentLetterIndex < _lettersSpelled.Length)
        {
            _lettersSpelled[_currentLetterIndex] = s;
        }

        _currentLetterIndex++;
        if (_currentLetterIndex < _lettersSpelled.Length)
        {
            _currentCorrectLetter = _wordLetters[_currentLetterIndex];
            CES.InvokeOnNextStepTask(_currentCorrectLetter); // this invokes a function in session manager to send updated session status
        }
        else
        {
            CES.InvokeOnStepComplete();
        }
    }

    // Calculates the starting x position of each interactive block and ledge 
    // Determined dynamically by the number of objects being spawned
    // TODO: if adding extra letters separate functions for ledges and cubes since there will be more block objects
    private float CalculateStartXPos()
    {
        if (_wordLetters.Length <= 1)
            return 0;

        float x = (float)(ledgesAnchor.x - (0.5*_wordLetters.Length*blockOffset));
        Debug.Log($"***** Calculated start x position for {_stepWord} to be {x}");
        return x;
    }

}
