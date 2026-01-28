using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerWASDNavmesh : MonoBehaviour
{
    public float walkSpeed = 3.5f;
    public float runSpeed = 6.5f;
    public float turnSpeed = 15f;

    public Animator animator;
    public string speedParam = "Speed";

    private NavMeshAgent agent;
    private Vector3 moveDir;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false; // we rotate manually
        agent.stoppingDistance = 0f;

        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (animator != null) animator.applyRootMotion = false;
    }

    void Update()
    {
        ReadInput();
        DriveAgent();
        UpdateAnim();
    }

    void ReadInput()
    {
        if (Keyboard.current == null) { moveDir = Vector3.zero; return; }

        float x = 0f, z = 0f;
        if (Keyboard.current.aKey.isPressed) x -= 1f;
        if (Keyboard.current.dKey.isPressed) x += 1f;
        if (Keyboard.current.wKey.isPressed) z += 1f;
        if (Keyboard.current.sKey.isPressed) z -= 1f;

        moveDir = new Vector3(x, 0, z).normalized;
    }

    void DriveAgent()
    {
        bool running = Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed;
        agent.speed = running ? runSpeed : walkSpeed;

        if (moveDir.sqrMagnitude > 0.001f)
        {
            // Push a short destination forward each frame.
            // Agent will slide along NavMesh boundaries (walls) instead of going through.
            Vector3 desired = transform.position + moveDir * 1.0f;

            if (NavMesh.SamplePosition(desired, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                agent.isStopped = false;
                agent.SetDestination(hit.position);
            }

            // Rotate toward input direction
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
        }
        else
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }

    void UpdateAnim()
    {
        if (animator == null) return;

        float normalized = Mathf.Clamp01(agent.velocity.magnitude / runSpeed);
        animator.SetFloat(speedParam, normalized, 0.1f, Time.deltaTime);
    }
}
