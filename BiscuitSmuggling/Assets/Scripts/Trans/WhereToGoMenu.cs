using UnityEngine;

public class WhereToGoMenu : SingletonMonoBehaviour<WhereToGoMenu>
{
    [SerializeField]
    private GameObject m_Overlay;

    public void Show()
    {
        PauseMenu.Instance.SetPaused();
        m_Overlay.SetActive(true);
        InteractLabel.Instance.Hide();
    }

    public void Hide()
    {
        PauseMenu.Instance.SetUnpaused();
        m_Overlay.SetActive(false);
        InteractLabel.Instance.Show();
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
