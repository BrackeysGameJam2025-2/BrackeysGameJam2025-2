using UnityEngine;

public class Player : SingletonMonoBehaviour<Player>
{
    public static Transform Transform => Instance.transform;

    private void OnEnable()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
