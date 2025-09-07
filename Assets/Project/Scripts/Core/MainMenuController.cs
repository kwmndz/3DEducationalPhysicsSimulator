using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void LoadSandbox()
    {
        SceneManager.LoadScene("Sandbox");
    }

    public void LoadDemos()
    {
        Debug.Log("coming soon!!");
    }

    public void LoadHelp()
    {
        Debug.Log("comeing later!");
    }

    public void QuitGame()
    {
        // Quits in only in build
        Application.Quit();
        Debug.Log("why would you ever quit tho :()");
    }
}
