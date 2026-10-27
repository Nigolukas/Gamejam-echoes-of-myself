using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int keys = 0;
    [SerializeField] private TextMeshProUGUI keyText;

    [SerializeField] private TextMeshProUGUI DialogueText;
    [SerializeField] private GameObject DialogueObject;
    [SerializeField] private float typeSpeed = 0.03f; // segundos por letra

    [SerializeField] private string[] toRead;
    private int currentLine = 0;
    private bool inSequence = false;

    private Coroutine typingRoutine;

    private void Start()
    {
        //StartDialogueSequence(toRead);
    }

    public void AddKey()
    {
        keys++;
        keyText.text = "" + keys;
    }

    public int GetKey()
    {
        return keys;
    }

    public bool TryUnlockDoor(GameObject door)
    {
        Door doorC = door.GetComponent<Door>();
        if (doorC.keysNeeded <= keys)
        {
            door.SetActive(false);
            doorC.portal.SetActive(true);
            return true;
        }
        else
        {
            int missing = doorC.keysNeeded - keys;
            string keysMessage = "Te faltan " + missing + " llaves para desbloquear esta puerta";

            TriggerDialogue tr = door.GetComponent<TriggerDialogue>();
            if (tr != null)
            {
                tr.Interact(keysMessage);
            }
            else
            {
                StartDialogueSequence(new string[] { keysMessage });
            }

            return false;
        }
    }

    public void SetDialogo(string text)
    {
        if (DialogueObject.activeSelf && DialogueText.text == text) return;

        DialogueObject.SetActive(true);
        DialogueText.text = text;
        Time.timeScale = 0;

        if (typingRoutine != null) StopCoroutine(typingRoutine);
        typingRoutine = StartCoroutine(TypeText());
    }

    public void StartDialogueSequence(string[] lines)
    {
        if (inSequence || lines == null || lines.Length == 0) return;

        toRead = lines;
        currentLine = 0;
        inSequence = true;
        SetDialogo(toRead[0]);
    }

    public void SetNextDialogue()
    {
        if (inSequence)
        {
            currentLine++;

            if (currentLine < toRead.Length)
            {
                SetDialogo(toRead[currentLine]);
                return;
            }
        }

        CloseDialogo();
    }

    public void CloseDialogo()
    {
        inSequence = false;
        Time.timeScale = 1;

        if (typingRoutine != null)
        {
            StopCoroutine(typingRoutine);
            typingRoutine = null;
        }
        DialogueObject.SetActive(false);
    }

    IEnumerator TypeText() //animacion de las letras
    {
        DialogueText.maxVisibleCharacters = 0;
        DialogueText.ForceMeshUpdate();
        int total = DialogueText.textInfo.characterCount;

        for (int i = 0; i <= total; i++)
        {
            DialogueText.maxVisibleCharacters = i;
            yield return new WaitForSecondsRealtime(typeSpeed);
        }

        typingRoutine = null;
    }


}