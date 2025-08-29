using UnityEngine;

public class Player : SingletonMonoBehaviour<Player>
{
    public static Transform Transform => Instance.transform;
}
