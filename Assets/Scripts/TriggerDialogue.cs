using UnityEngine;

public class TriggerDialogue : MonoBehaviour
{
     private GameManager gameManager;
    private bool alreadyPlayed;
    [SerializeField] private bool autocall = false;
    [SerializeField] private bool specialCall = false;
    [SerializeField] private bool shadowCan = false;
    private int jumps = 0;
    [SerializeField] private bool callOnce;
    private bool called;
    [TextArea(2, 5)]
    [SerializeField] private string[] lines;
    [SerializeField] private string secondaryLines;

    [SerializeField] private GameObject objectToLook;
    [SerializeField] private int lineToChangeFocus;
    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        called = false;
    }
    public void Interact(string firstLine = null)
    {
        Transform target = objectToLook != null ? objectToLook.transform : null;
        if (target != null)
            target.gameObject.SetActive(true);
        if (callOnce == true && called == true)
        {
            if(secondaryLines != null && secondaryLines != "")
                gameManager.SetDialogo(secondaryLines);
            return;
        }

        if (firstLine == null)
        {
            gameManager.StartDialogueSequence(lines, target, lineToChangeFocus);
            called = true;
            return;
        }

        string[] combined = new string[lines.Length + 1];
        combined[0] = firstLine;
        lines.CopyTo(combined, 1);
        gameManager.StartDialogueSequence(combined, target, lineToChangeFocus + 1);
        called = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && shadowCan == false)
        {
            if (autocall == true)
            {
                Interact();
            }
            else if (specialCall == true && jumps == 3)
            {
                Interact();
            }
            else
            {
                jumps++;
            }
        }
        if (collision.CompareTag("Shadow") && shadowCan && autocall == true)
        {
            Interact();
        }

    }
}
