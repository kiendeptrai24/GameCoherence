using Coherence;
using Coherence.Toolkit;
using UnityEngine;

public class ChatHandler : MonoBehaviour
{
    [Command]
    public void OnChatMessage(string message)
    {
        Debug.Log($"Received chat message: {message}");
    }
}