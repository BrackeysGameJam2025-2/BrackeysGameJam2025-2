using UnityEngine;

[CreateAssetMenu(fileName = "ItemMetadata", menuName = "Scriptable Objects/ItemMetadata")]
public class ItemMetadata : ScriptableObject
{
    [SerializeField]
    private string m_Name;
    [SerializeField]
    private int m_MaxStacks;
}
