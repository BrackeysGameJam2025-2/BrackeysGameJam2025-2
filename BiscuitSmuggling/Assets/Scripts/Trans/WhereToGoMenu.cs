using UnityEngine;

public class WhereToGoMenu : SingletonMonoBehaviour<WhereToGoMenu>
{
    [SerializeField]
    private GameObject m_Overlay;

    public void Show()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        PauseMenu.Instance.SetPaused();
        m_Overlay.SetActive(true);
    }

    public void Hide()
    {
        /*Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;*/

        PauseMenu.Instance.SetUnpaused();
        m_Overlay.SetActive(false);
    }

    public void GoToOffice()
    {
        TransitionManager.GoToOffice();
    }

    public void GoToCheckpoint()
    {
        TransitionManager.GoToCheckpoint();
    }

    public void GoToBar()
    {
        TransitionManager.GoToBar();
    }
}
