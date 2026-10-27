using System;
using System.Collections;
using UnityEngine;

public class Puzzle : MonoBehaviour
{
    [SerializeField] private GameObject key;
    [SerializeField] private GameObject platform;
    [SerializeField] private GameObject rotation1;
    [SerializeField] private GameObject rotation2;

    [SerializeField] private float speed = 2f;
    [SerializeField] private float distance = 3f;
    [SerializeField] private float rotationSpeed = 180f;

    private Vector3 platformStart, platformEnd;
    private Vector3 keyStart, keyEnd;
    private bool atEnd = false;
    private bool isMoving = false;

    void Start()
    {
        platformStart = platform.transform.position;
        platformEnd = platformStart + Vector3.left * distance;

        keyStart = key.transform.position;
        keyEnd = keyStart + Vector3.right * distance;
    }

    public void ActivateMechanism()
    {
        if (isMoving) return;
        isMoving = true;
        StartCoroutine(MovePlatform());
    }

    IEnumerator MovePlatform()
    {
        Vector3 platformTarget = atEnd ? platformStart : platformEnd;
        Vector3 keyTarget = atEnd ? keyStart : keyEnd;
        float dir = atEnd ? -1f : 1f;  

        while (platform.transform.position != platformTarget)
        {
            platform.transform.position = Vector3.MoveTowards(platform.transform.position, platformTarget, speed * Time.deltaTime);
            if(key != null)
                key.transform.position = Vector3.MoveTowards(key.transform.position, keyTarget, speed * Time.deltaTime);

            rotation1.transform.Rotate(0f, 0f, rotationSpeed * dir * Time.deltaTime);
            rotation2.transform.Rotate(0f, 0f, -rotationSpeed * dir * Time.deltaTime);

            yield return null;
        }

        atEnd = !atEnd;
        isMoving = false;
    }
}