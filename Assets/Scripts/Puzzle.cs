using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class Puzzle : MonoBehaviour
{
    [SerializeField] private GameObject key;
    [SerializeField] private GameObject chain;
    [SerializeField] private GameObject platform;
    [SerializeField] private GameObject rotation1;
    [SerializeField] private GameObject rotation2;

    [SerializeField] private float speed = 2f;
    [SerializeField] private float rotationSpeed = 180f;

    private Vector3 platformStart, platformEnd;
    private Vector3 keyStart, keyEnd;

    [SerializeField] private Transform keyEndPoint;
    [SerializeField] private Transform platformEndPoint;

    private bool atEnd = false;
    private bool isMoving = false;

    public bool onButton = false;

    void Start()
    {
        platformStart = platform.transform.position;
        platformEnd = platformEndPoint.position;
        if(key != null)
            keyStart = key.transform.position;
        if (keyEndPoint != null)
            keyEnd = keyEndPoint.position;
    }

    public void ActivateMechanism()
    {
        if (isMoving) return;
        isMoving = true;
        StartCoroutine(MovePlatform());
    }

    IEnumerator MovePlatform()
    {
        if(chain != null)
            chain.SetActive(true);
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
        if (atEnd == true && chain != null)
            chain.SetActive(false);
        atEnd = !atEnd;
        isMoving = false;
    }

    public void ActivateMechanismButton()
    {
        if (isMoving) return;
        StartCoroutine(MovePlatformButton());
    }


    IEnumerator MovePlatformButton()
    {
        isMoving = true;

        while (true)
        {
            Vector3 platformTarget = onButton ? platformEnd : platformStart;
            float dir = onButton ? 1f : -1f;

            if (platform.transform.position == platformTarget)
                break;

            platform.transform.position = Vector3.MoveTowards(
                platform.transform.position, platformTarget, speed * Time.deltaTime);

            rotation1.transform.Rotate(0f, 0f, rotationSpeed * dir * Time.deltaTime);
            rotation2.transform.Rotate(0f, 0f, -rotationSpeed * dir * Time.deltaTime);

            yield return null;
        }

        isMoving = false;
    }

}