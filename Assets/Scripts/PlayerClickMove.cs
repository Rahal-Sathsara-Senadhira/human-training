using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerClickMove : MonoBehaviour
{
    [Header("Click Ground")]
    public LayerMask groundMask;   // set to Ground
    public Camera cam;             // optional, auto uses Camera.main

    [Header("Animation")]
    public Animator animator;      // auto find if empty
    public string speedParam = "Speed";

    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (cam == null) cam = Camera.main;
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // Click to move
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 999f, groundMask))
            {
                agent.isStopped = false;
                agent.SetDestination(hit.point);
            }
        }

        // Update animation
        if (animator != null)
        {
            float normalized = 0f;
            if (agent.speed > 0.01f)
                normalized = Mathf.Clamp01(agent.velocity.magnitude / agent.speed);

            animator.SetFloat(speedParam, normalized);
        }
    }
}
