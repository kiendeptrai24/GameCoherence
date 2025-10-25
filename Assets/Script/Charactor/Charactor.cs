using Coherence.Toolkit;
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
    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        m_movement = GetComponent<IMovement>();
        CoherenceBridgeStore.TryGetBridge(gameObject.scene, out _coherenceBridge);
        _coherenceBridge.onLiveQuerySynced.AddListener(OnLiveQuerySynced);
    }
    protected virtual void Start()
    {
    }
    private void OnLiveQuerySynced(CoherenceBridge arg0)
    {
        CreateCinemachineCamera();
    }

    private void CreateCinemachineCamera()
    {
        if (_cmCameraPrefab == null) return;
        CinemachineCamera cinemachineCamera = Instantiate(_cmCameraPrefab);
        cinemachineCamera.Target.TrackingTarget = cinemachineCamera.Target.LookAtTarget = transform;
    }
}