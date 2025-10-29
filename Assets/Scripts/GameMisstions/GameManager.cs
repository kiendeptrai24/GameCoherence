using UnityEngine;
using System;
using JetBrains.Annotations;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private IMission currentMission;
    public IMissionDisplay CurrentMissionDisplay { get; private set; }

    public event Action<IMissionDisplay> OnMissionStarted;

    private bool gameStarted = false;

    private void Awake()
    {
        Instance = this;
    }
    private IEnumerator Start() {
        yield return null;
        StartGame(new TimerMission());
    }

    public void StartGame(IMission mission)
    {
        currentMission = mission;
        CurrentMissionDisplay = mission as IMissionDisplay;

        currentMission.StartMission();
        gameStarted = true;
        if(CurrentMissionDisplay == null)
            Debug.LogError("CurrentMissionDisplay is null");
        OnMissionStarted?.Invoke(CurrentMissionDisplay);
    }

    private void Update()
    {
        if (!gameStarted || currentMission == null) return;
        currentMission.UpdateMission();
        if (currentMission.CompleteMission())
        {
            Debug.Log("Mission Complete!");
            gameStarted = false;
        }
    }
}
