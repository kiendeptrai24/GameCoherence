using System;

public interface IMissionDisplay
{
    string Title { get; }
    float Progress { get; } // 0..1
    bool IsCompleted { get; }

    // Các sự kiện cho UI nghe
    event Action<IMissionDisplay> OnUpdated;
    event Action<IMissionDisplay> OnCompleted;
}
