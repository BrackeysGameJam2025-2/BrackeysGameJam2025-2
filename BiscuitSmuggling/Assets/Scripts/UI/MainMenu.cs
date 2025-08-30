using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void StartGame()
    {
        Debug.Log("TODO: Implement StartGame.");
        // UnityEngine.SceneManagement.SceneManager.LoadScene("???");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
