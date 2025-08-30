using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void StartGame()
    {
        Debug.Log("TODO: Check if that's start right.");
        SceneManager.LoadScene("WarehouseDay", LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
