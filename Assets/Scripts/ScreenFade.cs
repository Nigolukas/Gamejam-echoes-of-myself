using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private Color fadeColor = new Color(0.5f, 0.2f, 0.9f, 1f);
    [SerializeField] private float fadeInTime = 0.5f;
    [SerializeField] private float holdTime = 0.15f;
    [SerializeField] private float fadeOutTime = 0.6f;

    [Header("Distorsión")]
    [SerializeField] private float zoomAmount = 0.8f;  // cuánto se acerca la cámara
    [SerializeField] private float tiltAngle = 8f;     // cuánto se inclina

    private Camera cam;
    private float baseSize;

    public IEnumerator Transition(Action atPeak)
    {
        cam = Camera.main;
        baseSize = cam.orthographicSize;

        // Entrada: la pantalla se tiñe y la cámara se acerca y gira
        for (float e = 0f; e < fadeInTime; e += Time.unscaledDeltaTime)
        {
            Apply(Mathf.SmoothStep(0f, 1f, e / fadeInTime));
            yield return null;
        }
        Apply(1f);

        // La pantalla está tapada: aquí ocurre el cambio de posición
        atPeak?.Invoke();
        yield return new WaitForSecondsRealtime(holdTime);

        // Salida: todo vuelve a la normalidad
        for (float e = 0f; e < fadeOutTime; e += Time.unscaledDeltaTime)
        {
            Apply(1f - Mathf.SmoothStep(0f, 1f, e / fadeOutTime));
            yield return null;
        }
        Apply(0f);
    }

    private void Apply(float t)
    {
        Color c = fadeColor;
        c.a = t;
        fadeImage.color = c;

        cam.orthographicSize = baseSize - zoomAmount * t;
        cam.transform.rotation = Quaternion.Euler(0f, 0f, tiltAngle * t);
    }
}