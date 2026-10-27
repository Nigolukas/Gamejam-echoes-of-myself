using System.Threading;
using UnityEngine;

[System.Serializable]
public class DialogueLines
{
    public string[] lines;
}

public class LoopManager : MonoBehaviour
{
    [SerializeField] private int loop = -1;
    [SerializeField] private GameObject[] loopObjects;
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
        loopObjects[loop].SetActive(true);
        gameManager.StartDialogueSequence(linesLoop[loop].lines);
    }
}