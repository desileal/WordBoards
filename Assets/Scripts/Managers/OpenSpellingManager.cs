using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using UnityEngine;

public enum LetterLayout
{
    AtoZ,
    QWERTY,
    Shuffled
}

// TODO: 
// 
public class OpenSpellingManager : MonoBehaviour
{

    private SessionManager sessionManager;

    [SerializeField] public GameObject pokeBlockPrefab;
    [SerializeField] public GameObject grabBlockPrefab;
    [SerializeField] public OpenSpellingLedge openSpellingLedge;

    private LetterLayout letterLayout;

    // TODO: is this a list of transforms or game objects?
    [SerializeField]
    private List<Transform> blockPositions = new();

    [SerializeField] 
    private List<string> letters = new();

    private List<GameObject> letterBlocks = new();

    [SerializeField]
    private List<GameObject> spelledLetterObjects = new();

    CentralEventSystem CES;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sessionManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<SessionManager>();
        letterLayout = LetterLayout.AtoZ;

        CES = CentralEventSystem.Instance;
        if (CES)
        {
            CES.OnUpdateLedgeLetterTransforms += UpdateBlockPositions;
            CES.OnOpenSpellingLetterSelected += SnapBlockToLedge;
            CES.OnOpenSpellingLetterSelected += RespawnLetterBlock;
        }
    }

    // TODO:
    // check what letter config users want then spawn blocks based on that
    public void StartOpenSpelling()
    {
        SpawnLetterBlocks();
    }

    private void SpawnLetterBlocks()
    {
        int i = 0;
        foreach (string letter in letters)
        {
            // blocks are defined depending on Interaction from SessionManager
            GameObject block = InstantiateInteractiveBlock(letter, blockPositions[i], i);

            // add game objects to list
            letterBlocks.Add(block);

            i++;
        }
    }

    private void RespawnLetterBlock(string letter)
    {
        int i = letters.IndexOf(letter);
        GameObject block = InstantiateInteractiveBlock(letter, blockPositions[i], i);
        letterBlocks[i] = block;
    }

    /// <summary>
    /// Instantiates each block depending on the current InteractionType of the system
    /// </summary>
    /// <param name="letter"> letter displayed on the block </param>
    /// <param name="xPos"> x position where the block is spawned </param>
    /// <param name="index"> the element index where the block is in the list of interactionGameObjects </param>
    /// <returns></returns>
    private GameObject InstantiateInteractiveBlock(string letter, Transform transform, int index)
    {
        GameObject cube = new();
        switch (sessionManager.currentInteractionType)
        {
            case (InteractionType.Poke):
                cube = Instantiate(pokeBlockPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
                PokeBlock poke = cube.GetComponent<PokeBlock>();
                poke.targetID = letter;
                poke.letterText.text = letter;
                poke.listIndex = index;
                break;
            case (InteractionType.Grab):
                cube = Instantiate(grabBlockPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z ), Quaternion.identity);
                GrabBlock grab = cube.GetComponent<GrabBlock>();
                grab.targetID = letter;
                grab.letterText.text = letter;
                grab.listIndex = index;
                grab.openSpellingLedge = openSpellingLedge;
                break;
        }
        return cube;
    }

    private void SnapBlockToLedge(string s)
    {

    }

    private void UpdateBlockPositions()
    {
        for (int i = 0; i < spelledLetterObjects.Count; i++)
        {
            Vector3 blockVector = spelledLetterObjects[i].transform.position;
            spelledLetterObjects[i].GetComponent<Transform>().position = new Vector3(blockVector.x, blockVector.y, blockVector.z);

        }
    }

}
