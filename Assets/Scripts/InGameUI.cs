using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    public GameObject endGameUI;
    public TextMeshProUGUI titleText;
    public Slider progressBar;
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI completeText;

    private IMissionDisplay currentMission;

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

        titleText.text = mission.Title;
        progressBar.value = 0;
        completeText.gameObject.SetActive(false);

        mission.OnUpdated += OnMissionUpdated;
        mission.OnCompleted += OnMissionCompleted;
    }

    private void OnMissionUpdated(IMissionDisplay mission)
    {
        if (mission == null) Debug.Log("Mission is null");
        progressBar.value = mission.Progress;
        TimeSpan time = TimeSpan.FromSeconds(mission.CountdownTimer);
        progressText.text = $"{time.Minutes:D2}:{time.Seconds:D2}";


        //$"{mission.Progress * 100:F0}%";

    }

    private void OnMissionCompleted(IMissionDisplay mission)
    {
        completeText.gameObject.SetActive(true);
        completeText.text = "✅ Mission Complete!";
        EndGame();
    }
    public void EndGame()
    {
        endGameUI.SetActive(true);
    }
}
