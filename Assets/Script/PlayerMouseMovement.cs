namespace Kien
{
    using UnityEngine;
    using UnityEngine.AI;

    [RequireComponent(typeof(NavMeshAgent))]
    public class PlayerMouseMovement : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera; // Camera để chuyển đổi tọa độ màn hình thành tọa độ thế giới
        [SerializeField] private LayerMask groundLayer; // Layer của mặt đất để raycast
        [SerializeField] private float raycastDistance = 100f; // Khoảng cách tối đa của raycast
        private ParticleSystem particle;
        public GameObject prefab;

        private NavMeshAgent navMeshAgent;

        private void Awake()
        {
            // Lấy component NavMeshAgent
            navMeshAgent = GetComponent<NavMeshAgent>();
            if (navMeshAgent == null)
            {
                Debug.LogError("NavMeshAgent component is required on this GameObject.", this);
                enabled = false;
                return;
            }

            // Nếu không gán camera, tìm MainCamera
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
            particle = Instantiate(prefab, Vector3.zero, Quaternion.identity).GetComponent<ParticleSystem>();
            particle.Stop();
        }

        private void Update()
        {
            // Kiểm tra nhấp chuột trái
            if (Input.GetMouseButtonDown(0))
            {
                MoveToMousePosition();
            }
        }

        private void MoveToMousePosition()
        {
            // Tạo ray từ vị trí chuột trên màn hình
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            // Raycast để tìm điểm giao với mặt đất
            if (Physics.Raycast(ray, out hit, raycastDistance, groundLayer))
            {
                particle.transform.position = hit.point;
                particle.Play();
                // Đặt điểm đến cho NavMeshAgent
                navMeshAgent.SetDestination(hit.point);
            }
            else
            {
                Debug.LogWarning("No valid ground position clicked.", this);
            }
        }
    }
}