
using Coherence.Toolkit;
using UnityEngine;
using UnityEngine.UI;

public class EndGameUI : MonoBehaviour
{
    [SerializeField] private Button exit;
    private void Start() {
        var bridge = FindAnyObjectByType<CoherenceBridge>();

        exit.onClick.AddListener(() => {
            if(bridge)
            {
                bridge.Disconnect();
            }});
    
    }   
}