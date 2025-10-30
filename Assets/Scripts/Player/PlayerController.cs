using Coherence.Toolkit;


public class PlayerController : Charactor
{
    public PickUp pickUp;
    private GameManager gameManager;
    protected override void Awake()
    {
        base.Awake();
        pickUp = GetComponent<PickUp>();

        gameManager = GameManager.Instance;
        gameManager.OnGameEnd += () =>
        {
            if (!_sync.HasInputAuthority) return;
            Destroy(gameObject);

        };

        pickUp.animAction += (active) =>
        {
            if (active)
                stateMachine.ChangeState<PlantState_Player>();
            else
                stateMachine.ChangeState<IdleState_Player>();
        };
        
        stateMachine = new PlayerStateMachine(this, GetComponent<CoherenceSync>());
    }
    protected override void Start()
    {
        base.Start();
        stateMachine.Init<IdleState_Player>();
    }
    private void Update()
    {
        if (stateMachine != null)
        {
            stateMachine.Update();
        }
    }

}