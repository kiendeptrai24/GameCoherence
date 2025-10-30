using UnityEngine;
using System;
using TMPro;

public class TimerMission : MonoBehaviour, IMission, IMissionDisplay
{
    [SerializeField][TextArea] private string missionDescription;

    public LayerMask whatIsBox;
    public int boxToComplete = 3;
    public int currentboxInCorrectLocation = 0;

    public string Title => "complete all boxes for " + duration + "s";
    public float duration = 30f;
    private float startTime;
    private bool started;
    private bool isCompleted;

    public bool IsCompleted => isCompleted;
    public float Progress
    {
        get
        {
            if (!started) return 0f;
            return Mathf.Clamp01((Time.time - startTime) / duration);
        }
    }

    public float CountdownTimer => Mathf.Abs(Time.time - startTime - duration);

    public string Description => missionDescription;

    public event Action<IMissionDisplay> OnUpdated;
    public event Action<IMissionDisplay> OnCompleted;

    public void StartMission()
    {
        started = true;
        isCompleted = false;
        startTime = Time.time;
        OnUpdated?.Invoke(this);
    }

    public void UpdateMission()
    {
        if (!started || isCompleted) return;
        OnUpdated?.Invoke(this);

        if (Progress >= 1f)
        {
            DoneGame();
        }
    }

    public bool CompleteMission()
    {
        return isCompleted;
    }
    private void DoneGame()
    {
        isCompleted = true;
        OnCompleted?.Invoke(this);
    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.layer.ToString());
        if (((1 << other.gameObject.layer) & whatIsBox.value) != 0)
        {
            currentboxInCorrectLocation++;
            if (currentboxInCorrectLocation == boxToComplete)
            {
                DoneGame();
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        Debug.Log(other.gameObject.layer.ToString());
        if (((1 << other.gameObject.layer) & whatIsBox.value) != 0)
        {
            currentboxInCorrectLocation--;
        }
    }

}
