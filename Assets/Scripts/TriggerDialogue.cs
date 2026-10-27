using UnityEngine;

public class TriggerDialogue : MonoBehaviour
{
     private GameManager gameManager;
    private bool alreadyPlayed;
    [SerializeField] private bool callOnce;
    [TextArea(2, 5)]
    [SerializeField] private string[] lines;
    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }
    public void Interact(string firstLine = null)
    {
        if (firstLine == null)
        {
            gameManager.StartDialogueSequence(lines);
            return;
        }

        string[] combined = new string[lines.Length + 1];
        combined[0] = firstLine;
        lines.CopyTo(combined, 1);
        gameManager.StartDialogueSequence(combined);
    }
}
