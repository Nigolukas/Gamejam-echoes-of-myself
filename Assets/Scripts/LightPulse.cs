using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class LightPulse : MonoBehaviour
{
    [SerializeField] private float minIntensity = 0.5f;
    [SerializeField] private float maxIntensity = 1.5f;
    [SerializeField] private float speed = 2f;   // pulsos por segundo aproximadamente
    [SerializeField] private bool useUnscaledTime = false;

    private Light2D light2D;

    void Awake()
    {
        light2D = GetComponent<Light2D>();
    }

    void Update()
    {
        float time = useUnscaledTime ? Time.unscaledTime : Time.time;

        // Sin(...) va de -1 a 1; lo paso a 0..1 para interpolar entre min y max
        float t = (Mathf.Sin(time * speed) + 1f) * 0.5f;
        light2D.intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
    }
}