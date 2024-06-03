using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        Debug.Log("Play button pressed");
        SceneManager.LoadScene("Game"); // Asigură-te că numele scenei este exact "Game"
    }

    public void QuitGame()
    {
        Debug.Log("Quit button pressed");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }


    public void OpenOptions()
    {
        Debug.Log("Options button pressed");
        // Logica pentru a deschide meniul de opțiuni
    }
}
