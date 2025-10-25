using Coherence.Toolkit;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;


[RequireComponent(typeof(CoherenceSync), typeof(AnimationSync))]
public abstract class Charactor : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _cmCameraPrefab;
    protected CoherenceBridge _coherenceBridge;
    public IStateMachine stateMachine;
    public IMovement m_movement;
    public Animator anim;
    [SerializeField] protected TMP_Text charactorName;
    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        m_movement = GetComponent<IMovement>();
        CoherenceBridgeStore.TryGetBridge(gameObject.scene, out _coherenceBridge);
        CreateCinemachineCamera();
    }
    protected virtual void Start()
    {
    }
    private void CreateCinemachineCamera()
    {
        if (_cmCameraPrefab == null) return;
        CinemachineCamera cinemachineCamera = Instantiate(_cmCameraPrefab);
        cinemachineCamera.Target.TrackingTarget = cinemachineCamera.Target.LookAtTarget = transform;
    }
    public void SetName(string name) => charactorName.text = name;
}