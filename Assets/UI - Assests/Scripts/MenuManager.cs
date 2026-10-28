using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Aquí controlaremos la interfaz
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject controlsPanel;
    public GameObject credictsPanel;
    public GameObject storyPanel;
    
    private void Start()
    {
        ShowMainMenu();

    }
   public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        controlsPanel.SetActive(false);
        credictsPanel.SetActive(false);
        storyPanel.SetActive(false);
    }
    
    public void ShowControls()
    {
        mainMenuPanel.SetActive(false);
        controlsPanel.SetActive(true);
        credictsPanel.SetActive(false);
        storyPanel.SetActive(false);
    }

    public void ShowCredicts()
    {
        mainMenuPanel.SetActive(false);
        controlsPanel.SetActive(false);
        credictsPanel.SetActive(true);
        storyPanel.SetActive(false);
    }

    public void ShowStory()
    {
        mainMenuPanel.SetActive(false);
        controlsPanel.SetActive(false);
        credictsPanel.SetActive(false);
        storyPanel.SetActive(true);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
