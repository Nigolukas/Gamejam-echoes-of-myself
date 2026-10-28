using UnityEngine;

public class Door : MonoBehaviour
{
    public int keysNeeded;
    public GameObject portal;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        portal.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
