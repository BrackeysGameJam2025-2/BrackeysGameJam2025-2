using FMOD.Studio;
using FMODUnity;
using System;
using UnityEngine;

public sealed class ClankerHivemind : SingletonMonoBehaviour<ClankerHivemind>
{
    private int _patrolingClankers = 0;
    private int _searchingClankers = 0;
    private int _chasingClankers = 0;

    private EventInstance _soundtrackInstance;
    private EventInstance _sirenInstance;
    private float _alarmTimer;
    private float _refreshTimer;

    private PatrolNPC[] _clankers;

    [SerializeField, Min(0f)]
    private float m_TimeForAlarm = 15f;
    [SerializeField, Min(0f)]
    private float m_AlarmRefreshPlayerPositionTime = 3f;
    [SerializeField]
    private EventReference m_Soundtrack;
    [SerializeField]
    private EventReference m_SirenSfx;

    public Vector3 LastPlayerPosition { get; private set; }

    public bool WasAlarmStarted { get; private set; } = false;

    public event Action AlarmStarted;

    private void Start()
    {
        _sirenInstance = RuntimeManager.CreateInstance(m_SirenSfx);
        _soundtrackInstance = RuntimeManager.CreateInstance(m_Soundtrack);
        _soundtrackInstance.start();
        SetSoundtrackIntensity(0f);

        _clankers = FindObjectsByType<PatrolNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var clanker in _clankers)
        {
            clanker.SawPlayer += OnClankerSawPlayer;
            clanker.StateChanged += OnClankerStateChanged;
        }
        _patrolingClankers = _clankers.Length;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _sirenInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        _sirenInstance.release();
        _soundtrackInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        _soundtrackInstance.release();
    }

    private void Update()
    {
        if (WasAlarmStarted)
        {
            _refreshTimer += Time.deltaTime;
            if (_refreshTimer > m_AlarmRefreshPlayerPositionTime)
            {
                _refreshTimer = 0f;
                LastPlayerPosition = Player.Transform.position;
            }
        }

        if (_chasingClankers > 0)
        {
            _alarmTimer += Time.deltaTime;

            if (_alarmTimer >= m_TimeForAlarm) SetAlarm();
        }
        else
        {
            _alarmTimer = 0f;
        }
    }

    private void OnClankerSawPlayer(PatrolNPC sender, Vector3 lastPlayerPosition)
    {
        LastPlayerPosition = lastPlayerPosition;
    }

    private void OnClankerStateChanged(PatrolNPC sender, AIState oldState)
    {
        switch (oldState)
        {
            case AIState.Chase:
                _chasingClankers--;
                break;
            case AIState.Patrol:
                _patrolingClankers--;
                break;
            case AIState.Search:
                _searchingClankers--;
                break;
        }

        switch (sender.CurrentState)
        {
            case AIState.Chase:
                _chasingClankers++;
                break;
            case AIState.Patrol:
                _patrolingClankers++;
                break;
            case AIState.Search:
                _searchingClankers++;
                break;
        }

        if (_chasingClankers > 0)
        {
            SetSoundtrackIntensity(1f);
        }
        else if (_searchingClankers > 0)
        {
            SetSoundtrackIntensity(0.5f);
        }
        else
        {
            SetSoundtrackIntensity(0f);
        }
    }

    private void SetSoundtrackIntensity(float intensity)
    {
        _soundtrackInstance.setParameterByName("SuspicionBlend", intensity);
    }

    public void SetAlarm()
    {
        if (WasAlarmStarted) return;

        WasAlarmStarted = true;

        foreach (var clanker in _clankers)
        {
            clanker.IsAlarmed = true;
        }

        _sirenInstance.start();

        AlarmStarted?.Invoke();
    }

    public void Busted()
    {
        _sirenInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        foreach (var clanker in _clankers)
        {
            clanker.CurrentState = AIState.Frozen;
        }
    }
}
