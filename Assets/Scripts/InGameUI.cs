using System;
using MagicPigGames;
using TMPro;
using UnityEngine;

public class InGameUI : MonoBehaviour
{
    public GameObject endGameUI;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI progressSecondText;
    public TextMeshProUGUI questText;
    public TextMeshProUGUI progresspercentText;

    private IMissionDisplay currentMission;
    [SerializeField] private ProgressBarInspectorTest progressBarInspectorTest;


    private void Start()
    {
        // Lắng nghe khi GameManager có mission mới
        GameManager.Instance.OnMissionStarted += BindMission;
    }

    private void BindMission(IMissionDisplay mission)
    {
        if (currentMission != null)
        {
            currentMission.OnUpdated -= OnMissionUpdated;
            currentMission.OnCompleted -= OnMissionCompleted;
        }

        currentMission = mission;

        if (mission == null) return;

        titleText.text = "Tilte: ".ToUpper() + mission.Title;
        questText.gameObject.SetActive(true);
        questText.text = "Quest: ".ToUpper() + mission.Description;

        mission.OnUpdated += OnMissionUpdated;
        mission.OnCompleted += OnMissionCompleted;
    }

    private void OnMissionUpdated(IMissionDisplay mission)
    {
        if (mission == null) Debug.Log("Mission is null");
        progressBarInspectorTest.progress = mission.Progress;
        progresspercentText.text = $"{mission.Progress * 100:F0}%";
        TimeSpan time = TimeSpan.FromSeconds(mission.CountdownTimer);
        progressSecondText.text = $"{time.Minutes:D2}:{time.Seconds:D2}";
        //$"{mission.Progress * 100:F0}%";

    }

    private void OnMissionCompleted(IMissionDisplay mission)
    {
        EndGame();
    }
    public void EndGame()
    {
        endGameUI.SetActive(true);
    }
}
