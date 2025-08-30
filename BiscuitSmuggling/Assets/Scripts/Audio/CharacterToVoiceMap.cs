using FMODUnity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterToVoiceMap", menuName = "Custom/Audio/Character to Voice Map")]
public class CharacterToVoiceMap : ScriptableObject
{
    [System.Serializable]
    public class Entry
    {
        public string CharacterName;
        public EventReference Voice;
    }

    [SerializeField]
    private Entry[] m_Entries;
    [SerializeField]
    private EventReference m_DefaultEvent;

    private IReadOnlyDictionary<string, EventReference> _map;

    public EventReference NameToEvent(string name)
    {
        _map ??= LoadMap();
        return _map.GetValueOrDefault(name, m_DefaultEvent);
    }

    public Dictionary<string, EventReference> LoadMap()
    {
        return m_Entries.ToDictionary(x => x.CharacterName.Trim().ToLower(), x => x.Voice);
    }
}
