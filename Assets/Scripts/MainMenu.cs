using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    void Start()
    {
        // Ensure time is running
        Time.timeScale = 1f;
    }

    public void PlayGame()
    {
        Debug.Log("Starting game...");
        SceneManager.LoadScene("SwitchScene");  // Change to your game scene name
    }

    public void OpenSettings()
    {
        Debug.Log("Opening settings...");
        // Add settings menu logic here
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
