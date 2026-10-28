using Unity.VisualScripting;
using UnityEngine;

public class ActivateOnTrigger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject turnOn;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        turnOn.SetActive(true);
    }

}
