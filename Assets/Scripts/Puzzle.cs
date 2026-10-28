using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using System.Collections.Generic;

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

    [Header("Cámara")]
    [SerializeField] private float focusDelay = 0.6f;  
    [SerializeField] private float focusHold = 0.5f;

    [Header("Sonido")]

    [SerializeField] private AudioSource MechanismSound;

    private CameraFocus cameraFocus;
    private bool atEnd = false;
    private bool isMoving = false;

    private readonly HashSet<GameObject> pressers = new HashSet<GameObject>();

    public bool onButton
    {
        get
        {
            pressers.RemoveWhere(g => g == null);   // limpia sombras destruidas
            return pressers.Count > 0;
        }
    }

    public void Press(GameObject who)
    {
        pressers.Add(who);
        ActivateMechanismButton();
    }

    public void Release(GameObject who)
    {
        pressers.Remove(who);
        ActivateMechanismButton();
    }

    private void Awake()
    {
        cameraFocus = FindAnyObjectByType<CameraFocus>();
    }
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
        if(MechanismSound != null) MechanismSound.Play();
        // La cámara va hacia la plataforma y espera a que llegue
        if (cameraFocus != null)
        {
            cameraFocus.FocusOn(platform.transform);
            yield return new WaitForSeconds(focusDelay);
        }

        if (chain != null)
            chain.SetActive(true);

        Vector3 platformTarget = atEnd ? platformStart : platformEnd;
        Vector3 keyTarget = atEnd ? keyStart : keyEnd;
        float dir = atEnd ? -1f : 1f;

        while (platform.transform.position != platformTarget)
        {
            platform.transform.position = Vector3.MoveTowards(platform.transform.position, platformTarget, speed * Time.deltaTime);
            if (key != null)
                key.transform.position = Vector3.MoveTowards(key.transform.position, keyTarget, speed * Time.deltaTime);

            rotation1.transform.Rotate(0f, 0f, rotationSpeed * dir * Time.deltaTime);
            rotation2.transform.Rotate(0f, 0f, -rotationSpeed * dir * Time.deltaTime);

            yield return null;
        }

        if (atEnd == true && chain != null)
            chain.SetActive(false);
        atEnd = !atEnd;

        // Se queda un momento y vuelve al jugador
        if (cameraFocus != null)
        {
            yield return new WaitForSeconds(focusHold);
            cameraFocus.ReturnToPlayer();
        }
        if (MechanismSound != null) MechanismSound.Stop();
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
        if (MechanismSound != null) MechanismSound.Play();
        while (true)
        {
            Vector3 platformTarget = onButton ? platformEnd : platformStart;
            float dir = onButton ? 1f : -1f;

            if (platform.transform.position == platformTarget)
                break;

            platform.transform.position = Vector3.MoveTowards(
                platform.transform.position, platformTarget, speed * Time.deltaTime);
            if(rotation1 != null  && rotation2 != null)
            {
                rotation1.transform.Rotate(0f, 0f, rotationSpeed * dir * Time.deltaTime);
                rotation2.transform.Rotate(0f, 0f, -rotationSpeed * dir * Time.deltaTime);
            }


            yield return null;
        }
        if (MechanismSound != null) MechanismSound.Stop();
        isMoving = false;
    }

}