public class InteractLabel : SingletonMonoBehaviour<InteractLabel>
{
    public void Show()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void Hide()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
