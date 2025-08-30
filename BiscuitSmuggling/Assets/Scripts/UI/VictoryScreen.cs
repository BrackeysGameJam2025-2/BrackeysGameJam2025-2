public class VictoryScreen : SingletonMonoBehaviour<VictoryScreen>
{
    public void Show()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }
}
