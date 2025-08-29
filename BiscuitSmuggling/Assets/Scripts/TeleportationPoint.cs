using UnityEngine;

public class TeleportationPoint : MonoBehaviour
{
    public static TeleportationPoint Instance { get; private set; }

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError($"There multiple instance of {this} point {this.gameObject}");
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public Vector3 GetPosition()
    {
        return this.transform.position;
    }
}
