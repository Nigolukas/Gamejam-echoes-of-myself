using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenuUI : MonoBehaviour
{
    // Este script maneja los paneles de Pausa

    //Indica si el juego está en pausa
    private bool isPaused = false;
    
    [Header("Paneles")]
    [SerializeField] private GameObject pauseBackground;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject confirmationPanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject mainMenuConfirmationPanel;
    GameManager gameManager;

    // Actualizar el nombre de la escena correcta
    [SerializeField] private string gameplaySceneName = "SampleScene";

    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }
    private void Start()
    {
        // Restablece el tiempo normal
        Time.timeScale = 1f;
        // Juego comienza sin estar pausado
        isPaused = false;
    }
    
    // Navegación de pantallas
    // Confirmación de reinicio
    public void ShowConfirmation()
    {
        pausePanel.SetActive(false);
        confirmationPanel.SetActive(true);
    }

    // Volver a Pausa desde confirmación
    public void CancelConfirmation()
    {
        confirmationPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    // Controles de juego desde Pausa
    public void ShowControls()
    {
        pausePanel.SetActive(false);
        controlsPanel.SetActive(true);
    }

    // Regresar a Pausa desde controles
    public void BackToPause()
    {
        controlsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    // Reiniciar juego
    public void ConfirmRestart()
    {
        // Tiempo se restaura para evitar inicio en pausa
        Time.timeScale = 1f;
        // Carga la escena inicial
        SceneManager.LoadScene(gameplaySceneName);
    }

    // Regresa al menú principal
    public void ReturnToMainMenu()
    {
        // Restablece el tiempo para que el menú no quede pausado
        Time.timeScale = 1f;

        // Carga la escena del main menu
        SceneManager.LoadScene("MainMenu");
    }

    // Abre el menú de pausa y detiene el tiempo del juego
    public void PauseGame()
    {
        // Pausa
        if (gameManager.onDialogue == true) return;

        isPaused = true;
        
        pausePanel.SetActive(true);
        confirmationPanel.SetActive(false);
        controlsPanel.SetActive(false);

        pauseBackground.SetActive(true);

        // Detiene el tiempo
        Time.timeScale = 0f;
    }

    // Cierra pausa y vuelve al juego
    public void BackToGame()
    {
        // Restaura el tiempo
        Time.timeScale = 1f;
        isPaused = false;

        //Oculta el menú de pausa
        pausePanel.SetActive(false);
        pauseBackground.SetActive(false);
    }

    // Confirmación para volver al menú pirncipal
    public void ShowMainMenuConfirmation()
    {
        pausePanel.SetActive(false);
        mainMenuConfirmationPanel.SetActive(true);
    }

    // Vuelve a pausa desde confirmación
    public void CancelMainMenuConfirmation()
    {
        mainMenuConfirmationPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    private void Update()
    {
        // Comprueba que haya teclado disponible
        if (Keyboard.current == null)
            return;

        // Continua solo si ESC es presionado
        if (!Keyboard.current.escapeKey.wasPressedThisFrame)
            return;

        // Comprueba que los otros paneles no esten activos
        // Evita que ESC se active desde controles o confirmación 
        if (controlsPanel.activeSelf || confirmationPanel.activeSelf)
            return;
        {
            // Si el juego esta en Pausa lo reanuda
            if (isPaused)
            {
                BackToGame();
            }

            // Si el juego esta activo lo pausa
            else
            {
                PauseGame();
            }
        }    
    }
}