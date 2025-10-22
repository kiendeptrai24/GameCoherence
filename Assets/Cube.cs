using Coherence.Toolkit;
using UnityEngine;

public class Cube : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    CoherenceSync sync;
    void Start()
    {
        sync = GetComponent<CoherenceSync>();
    }

    // Update is called once per frame
    void Update()
    {
        if(sync.HasInputAuthority)
        {
            
        }
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        transform.Translate(move * Time.deltaTime * 8, Space.World);
        transform.forward = Vector3.Lerp(transform.forward, move, Time.deltaTime * 8);
        
    }
}
