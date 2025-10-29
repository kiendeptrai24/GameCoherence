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
        Debug.Log("BindMission");
    }

    private void OnMissionUpdated(IMissionDisplay mission)
    {
        Debug.Log("OnMissionUpdated");
        progressBar.value = mission.Progress;
        progressText.text = $"{mission.Progress * 100:F0}%";
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
