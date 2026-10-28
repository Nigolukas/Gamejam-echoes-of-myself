using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeEnd : MonoBehaviour
{
    [SerializeField] private SimplePlayerMovement player;
    [SerializeField] private Image fadeImage;          // Image blanca a pantalla completa, Raycast Target desmarcado
    [SerializeField] private float fadeDuration = 3f;
    [SerializeField] private float zoomAmount = 2f;    // cuánto se acerca la cámara
    [SerializeField] private string nextScene = "";    // opcional: escena a cargar al terminar

    private GameManager gameManager;
    private bool endActivated = false;

    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (endActivated || !collision.CompareTag("Player")) return;
        endActivated = true;
        StartCoroutine(EndCoroutine());
    }

    IEnumerator EndCoroutine()
    {
        // Espera a que el diálogo realmente empiece (el trigger de diálogo puede ejecutarse después)
        float timeout = 1f;
        while (!gameManager.onDialogue && timeout > 0f)
        {
            timeout -= Time.unscaledDeltaTime;
            yield return null;
        }

        // Espera a que termine el diálogo final
        yield return new WaitUntil(() => !gameManager.onDialogue);

        // El personaje camina solo
        if(player != null)
            player.autoMove = true;

        Camera cam = Camera.main;
        float startSize = cam.orthographicSize;
        float endSize = startSize - zoomAmount;

        // Fundido a blanco con zoom
        for (float e = 0f; e < fadeDuration; e += Time.deltaTime)
        {
            float t = Mathf.SmoothStep(0f, 1f, e / fadeDuration);

            cam.orthographicSize = Mathf.Lerp(startSize, endSize, t);
            fadeImage.color = new Color(1f, 1f, 1f, t);

            yield return null;
        }

        fadeImage.color = Color.white;

        if (!string.IsNullOrEmpty(nextScene))
            SceneManager.LoadScene(nextScene);
    }
}