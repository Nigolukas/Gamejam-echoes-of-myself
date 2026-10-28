using UnityEngine;

public class UIAudioManager : MonoBehaviour
{
    // Control de sonidos en botones

    [Header("Sonidos de la interfaz")]
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;


    [Header("Reproductor de audio botones")]
    [SerializeField] private AudioSource audioSource;

    // Reproduce sonido al pasar el cursor sobre el botón
    public void PlayHover()
    {
        if (hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    // Reproduce sonido al dar click a un botón
    public void PlayClick()
    {
        if (clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}
