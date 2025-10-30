using System;
using UnityEditor;

public interface IMissionDisplay
{
    string Title { get; }
    float Progress { get; }
    bool IsCompleted { get; }
    float CountdownTimer { get; }

    // Các sự kiện cho UI nghe
    event Action<IMissionDisplay> OnUpdated;
    event Action<IMissionDisplay> OnCompleted;
}
