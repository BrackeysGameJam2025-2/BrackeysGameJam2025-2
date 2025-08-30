using UnityEngine;
using UnityEngine.SceneManagement;

public static class TransitionManager
{
    public static bool IsDay()
    {
        Debug.LogWarning("IMPLEMENT IsDay!!!!");
        return Random.Range(0, 2) == 0;
    }

    private static void Transition(string location)
    {
        string daytime = IsDay() ? "Day" : "Night";
        string sceneName = $"{location}{daytime}";
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        PauseMenu.Instance.SetUnpaused();
    }

    public static void GoToOffice()
    {
        Transition("Office");
    }

    public static void GoToCheckpoint()
    {
        Transition("Checkpoint");
    }

    public static void GoToBar()
    {
        Transition("Bar");
    }

    public static void GoToWarehouse()
    {
        Transition("Warehouse");
    }
}
