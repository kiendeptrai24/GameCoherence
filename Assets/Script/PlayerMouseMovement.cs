namespace Kien
{
    using UnityEngine;
    using UnityEngine.AI;

    [RequireComponent(typeof(NavMeshAgent))]
    public class PlayerMouseMovement : MonoBehaviour, IMovement
    {
        [SerializeField] private Camera mainCamera; 
        [SerializeField] private LayerMask groundLayer; 
        [SerializeField] private float raycastDistance = 100f; 
        private ParticleSystem particle;
        public GameObject effect;
        public NavMeshAgent navMeshAgent;
        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            if (navMeshAgent == null)
            {
                Debug.LogError("NavMeshAgent component is required on this GameObject.", this);
                enabled = false;
                return;
            }

            if (mainCamera == null)
            {
                mainCamera = Camera.main;
                if (mainCamera == null)
                {
                    Debug.LogError("No Main Camera found in the scene.", this);
                    enabled = false;
                    return;
                }
            }
            particle = Instantiate(effect, Vector3.zero, Quaternion.identity).GetComponent<ParticleSystem>();
            particle.Stop();
        }
        private void Update() => Move();
        private void MoveToMousePosition()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, raycastDistance, groundLayer))
            {
                particle.transform.position = hit.point;
                particle.Play();
                navMeshAgent.SetDestination(hit.point);
            }
            else
            {
                Debug.LogWarning("No valid ground position clicked.", this);
            }
        }
        public void Move()
        {
            if (Input.GetMouseButtonDown(0))
            {
                MoveToMousePosition();
            }
        }
        public Vector3 GetVelocity() => navMeshAgent.velocity;
        public float GetSpeed() => navMeshAgent.speed;
        public Vector3 GetPosition() => transform.position;

    }
}