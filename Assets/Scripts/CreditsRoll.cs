using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditsRoll : MonoBehaviour
{
    [Header("Contenido")]
    [TextArea(2, 6)]
    [SerializeField] private string[] lines;

    [Header("Estilo")]
    [SerializeField] private TMP_FontAsset font;          // opcional
    [SerializeField] private float fontSize = 48f;
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private float textWidth = 1400f;
    [SerializeField] private float entrySpacing = 60f;    // espacio entre textos

    [Header("Movimiento")]
    [SerializeField] private float startDelay = 1f;
    [SerializeField] private float scrollSpeed = 80f;     // unidades de canvas por segundo
    [Range(0f, 1f)]
    [SerializeField] private float stopPosition = 0.5f;   // dónde se queda el último texto (0 = sale por completo, 0.5 = centro)
    [SerializeField] private float holdTime = 2f;

    [Header("Final")]
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private string nextScene;

    private RectTransform canvasRect;
    private RectTransform textRect;
    private Image fadeImage;

    void Start()
    {
        BuildUI();
        StartCoroutine(Run());
    }

    void BuildUI()
    {
        // Canvas
        var canvasGO = new GameObject("CreditsCanvas");
        canvasGO.transform.SetParent(transform);
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasRect = canvasGO.GetComponent<RectTransform>();

        // Fondo negro
        var bg = CreateImage("Background", canvasGO.transform, Color.black);

        // Texto de créditos
        var textGO = new GameObject("CreditsText");
        textGO.transform.SetParent(canvasGO.transform, false);
        var tmp = textGO.AddComponent<TextMeshProUGUI>();
        if (font != null) tmp.font = font;
        tmp.text = string.Join("\n", lines);
        tmp.fontSize = fontSize;
        tmp.color = textColor;
        tmp.alignment = TextAlignmentOptions.Top;
        tmp.paragraphSpacing = entrySpacing;
        tmp.raycastTarget = false;

        textRect = tmp.rectTransform;
        textRect.anchorMin = textRect.anchorMax = new Vector2(0.5f, 1f);
        textRect.pivot = new Vector2(0.5f, 1f);
        float height = tmp.GetPreferredValues(tmp.text, textWidth, 0f).y;
        textRect.sizeDelta = new Vector2(textWidth, height);

        // Capa de fundido (siempre encima de todo)
        fadeImage = CreateImage("Fade", canvasGO.transform, new Color(0f, 0f, 0f, 0f));
    }

    Image CreateImage(string objName, Transform parent, Color color)
    {
        var go = new GameObject(objName);
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.color = color;
        img.raycastTarget = false;

        var rt = img.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
        return img;
    }

    IEnumerator Run()
    {
        //yield return null; // espera un frame para que el Canvas calcule su tamaño

        float canvasH = canvasRect.rect.height;
        float textH = textRect.rect.height;

        float startY = -canvasH;                        // el texto empieza justo debajo de la pantalla
        float endY = textH - canvasH * stopPosition;    // el último texto se detiene aquí
        float y = startY;
        textRect.anchoredPosition = new Vector2(0f, y);

        yield return new WaitForSecondsRealtime(startDelay);

        while (y < endY)
        {
            y += scrollSpeed * Time.unscaledDeltaTime;
            textRect.anchoredPosition = new Vector2(0f, y);
            yield return null;
        }

        yield return new WaitForSecondsRealtime(holdTime);

        // Fundido a negro
        for (float e = 0f; e < fadeDuration; e += Time.unscaledDeltaTime)
        {
            fadeImage.color = new Color(0f, 0f, 0f, e / fadeDuration);
            yield return null;
        }
        fadeImage.color = Color.black;

        if (string.IsNullOrEmpty(nextScene))
        {
            Debug.LogWarning("CreditsRoll: no hay escena asignada en 'Next Scene'.");
            yield break;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(nextScene);
    }
}