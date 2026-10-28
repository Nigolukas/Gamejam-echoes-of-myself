using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{
    // Este script maneja los paneles de Pausa

    [Header("Paneles")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject confirmationPanel;
    [SerializeField] private GameObject controlsPanel;


    public void ShowConfirmation()
    {
        pausePanel.SetActive(false);
        confirmationPanel.SetActive(true);
    }

    public void CancelConfirmation()
    {
        confirmationPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void ShowControls()
    {
        pausePanel.SetActive(false);
        controlsPanel.SetActive(true);
    }

    public void BackToPause()
    {
        controlsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }
}
