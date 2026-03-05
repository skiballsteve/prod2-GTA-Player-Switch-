using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Pause Menu Properties")]
    public GameObject pauseMenuPanel;
    private bool isPaused = false;

    [Header("Pause Menu Ref")]
    public CinemachineBrain brain;

    void Start()
    {
        brain = Camera.main.GetComponent<CinemachineBrain>();

        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
    }

    void Update()
    {
        // Toggle pause with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
                brain.enabled = true;

            }
            else
            {
                Pause();
                brain.enabled = false;

            }
        }
    }

    public void Pause()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }

        Time.timeScale = 0f;
        isPaused = true;
        brain.enabled = false;

        // Optional: Hide cursor during gameplay, show in menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Game paused");
    }

    public void Resume()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        Time.timeScale = 1f;
        isPaused = false;
        brain.enabled = true;


        // Optional: Lock cursor during gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("Game resumed");
    }

    public void Restart()
    {
        Debug.Log("Restarting level...");
        Time.timeScale = 1f;
        brain.enabled = true;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu()
    {
        Debug.Log("Loading main menu...");
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
