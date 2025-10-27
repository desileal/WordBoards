using System.Collections;
using UnityEngine;

public class GameStartManager : MonoBehaviour
{
    [SerializeField] private SessionManager sessionManager;

    public bool sessionIsReadyToStart = false;

    CentralEventSystem CES;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private IEnumerator Start()
    {
        CES = CentralEventSystem.Instance;

        if(sessionManager == null)
        {
            Debug.LogError("Missing reference to SessionManager in GameStartManager script.");
            yield break;
        }
        if (sessionManager.trainingIsReady)
        {
            SessionStartHandler();
        }
        else
        {
            CES.OnTrainingStart += SessionStartHandler;
        }
    }

    public void SessionStartHandler()
    {

    }
}
