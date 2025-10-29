using UnityEngine;
using System;

public class TimerMission : MonoBehaviour, IMission, IMissionDisplay
{
    public string Title => "Survive for " + duration + "s";

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
        Debug.Log("Update");
        OnUpdated?.Invoke(this);

        if (Progress >= 1f)
        {
            isCompleted = true;
            OnCompleted?.Invoke(this);
        }
    }

    public bool CompleteMission()
    {
        return isCompleted;
    }
}
