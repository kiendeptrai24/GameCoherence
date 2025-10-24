using Coherence;
using Coherence.Toolkit;
using TMPro;
using UnityEngine;

public class Cube : MonoBehaviour
{
    CoherenceClientConnection myConnection;
    public CoherenceSync _counterSync;
    void Start()
    {
        var bridge = FindAnyObjectByType<CoherenceBridge>();
        _counterSync = GetComponent<CoherenceSync>();
        myConnection = bridge.ClientConnections.GetMine();

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        transform.Translate(move * Time.deltaTime * 8, Space.World);
        transform.forward = Vector3.Lerp(transform.forward, move, Time.deltaTime * 8);

    }
    public void SendCommand()
    {
        Debug.Log("SendCommand");
    }
    public void OnChatMessage(string message)
    {
        Debug.Log($"Received chat message: {message}");
    }
}
