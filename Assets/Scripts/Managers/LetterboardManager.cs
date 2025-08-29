using UnityEngine;
using TMPro;

public enum LetterboardType
{
    ALPHABET, COLOURS, QWERTY, NULL
}

public class LetterboardManager : MonoBehaviour
{
    // private string currentInput = "";

    private GameObject gameManagerObject;
    private GameManager gameManager;

    private AudioManager audioManager;
    private AudioSource audioSource;

    public TextMeshPro letterboardText;

    public LetterboardType letterboardType;

    public void Awake()
    {
        gameManagerObject = GameObject.FindGameObjectWithTag("GameManager");
        gameManager = gameManagerObject.GetComponent<GameManager>();
        audioManager = gameManagerObject.GetComponent<AudioManager>();
    }

    void Start()
    {
        
    }

    public void Speak(string text)
    {
        // Parse punctuation if necessary.
        switch (text)
        {
            case "'":
                text = "APOSTROPHE";
                break;
            case "!":
                text = "EXCLAMATION";
                break;
            case "?":
                text = "QUESTION";
                break;
            case ".":
                text = "PERIOD";
                break;
        }

         // Get the relevant clip and play it.
        AudioClip clip = AudioManager.Instance.GetClip(text);
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void AppendCharacter(string key)
    {
        letterboardText.text += key;
        // speak letter aloud
    }

    public void Backspace()
    {
        letterboardText.text = letterboardText.text.Remove(letterboardText.text.Length - 1);
    }

    public void AddSpace()
    {
        letterboardText.text += " ";
    }

    public void EnterPressed()
    {
        // speak word aloud

        // clear the entire input line?
    }

    
}
