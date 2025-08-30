public class LoseScreen : SingletonMonoBehaviour<LoseScreen>
{
    public void Show()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }
}
