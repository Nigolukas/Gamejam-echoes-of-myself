using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

[System.Serializable]
public class DialogueLines
{
    public string[] lines;
}

public class LoopManager : MonoBehaviour
{
    public int keysGot = 0;
    [SerializeField] private int loop = 0;
    [SerializeField] private GameObject[] loopObjects;
    [SerializeField] private GameObject[] deleteLoopObjects;
    [SerializeField] private DialogueLines[] linesLoop;

    GameManager gameManager;

    void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        
    }

    private void Start()
    {
        RestartLoop();
    }

    public void RestartLoop()
    {
        loop++;
        Debug.Log("loop: " + loop);
        if(loop < loopObjects.Length && keysGot >= 1)
        {
            loopObjects[loop].SetActive(true);
            if(deleteLoopObjects[loop] != null)
                deleteLoopObjects[loop].SetActive(false);
            gameManager.StartDialogueSequence(linesLoop[loop].lines);
            keysGot = 0;
        }
        else
        {
            loop--;
            gameManager.StartDialogueSequence(linesLoop[loop].lines);
        }

    }
}