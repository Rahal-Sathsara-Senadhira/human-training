using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;

    [Tooltip("Optional: rotate only this visual root (recommended). If empty, this GameObject will rotate.")]
    [SerializeField] private Transform visualRoot;

    [Tooltip("If your model faces the wrong way (hits air), set this to 180 (or -180).")]
    [SerializeField] private float visualYawOffset = 0f;

    [Header("Movement / Chase")]
    [SerializeField] private float chaseRange = 6f;
    [SerializeField] private float stopDistance = 1.4f;

    [Header("Attack")]
    [SerializeField] private float attackRange = 1.6f;
    [SerializeField] private float attackCooldown = 1.1f;

    [Header("Animator Params")]
    [SerializeField] private string speedParam = "Speed"; // float, 0..1
    [SerializeField] private string attackTrigger = "Attack"; // trigger

    [Header("Optional: Drive MeleeAttack automatically")]
    [SerializeField] private MeleeAttack meleeAttack;

    private Transform player;
    private float nextAttackTime;

    private int speedHash;
    private int attackHash;

    void Reset()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        meleeAttack = GetComponent<MeleeAttack>();
    }

    void Awake()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!animator) animator = GetComponentInChildren<Animator>();
        if (!meleeAttack) meleeAttack = GetComponent<MeleeAttack>();

        // We rotate manually (important for 2.5D + fixed facing).
        if (agent) agent.updateRotation = false;

        speedHash = Animator.StringToHash(speedParam);
        attackHash = Animator.StringToHash(attackTrigger);

        // If no visualRoot assigned, rotate the whole enemy.
        if (!visualRoot) visualRoot = transform;
    }

    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;

        // If we have a melee component, tell it who to hit.
        if (meleeAttack)
            meleeAttack.SetTarget(playerTransform);
    }

    void Update()
    {
        if (!player || !agent) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist > chaseRange)
        {
            StopMoving();
            return;
        }

        // Chase until close enough
        if (dist > stopDistance && dist > attackRange)
        {
            MoveTowards(player.position);
        }
        else
        {
            StopMoving();

            // Face player even when stopped (so attacks go correct direction)
            FaceTarget(player.position);

            if (dist <= attackRange && Time.time >= nextAttackTime)
            {
                DoAttack();
            }
        }

        UpdateMoveAnimation();
    }

    private void MoveTowards(Vector3 targetWorldPos)
    {
        Vector3 dest = targetWorldPos;
        dest.y = transform.position.y; // keep on our plane

        agent.isStopped = false;
        agent.SetDestination(dest);

        // Face in moving direction or towards player
        if (agent.velocity.sqrMagnitude > 0.01f)
        {
            Vector3 look = transform.position + agent.velocity;
            FaceTarget(look);
        }
        else
        {
            FaceTarget(player.position);
        }
    }

    private void StopMoving()
    {
        if (!agent) return;
        agent.isStopped = true;

        // Avoid spam warnings when not on navmesh.
        if (agent.isOnNavMesh)
            agent.ResetPath();
    }

    private void DoAttack()
    {
        nextAttackTime = Time.time + attackCooldown;

        // Snap face to player right before triggering
        FaceTarget(player.position);

        if (animator)
            animator.SetTrigger(attackHash);
    }

    private void FaceTarget(Vector3 worldPos)
    {
        Vector3 dir = worldPos - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.0001f) return;

        Quaternion lookRot = Quaternion.LookRotation(dir.normalized, Vector3.up);
        // Apply offset if model forward axis is flipped
        lookRot *= Quaternion.Euler(0f, visualYawOffset, 0f);

        if (visualRoot)
            visualRoot.rotation = lookRot;
        else
            transform.rotation = lookRot;
    }

    private void UpdateMoveAnimation()
    {
        if (!animator || !agent) return;

        float v = agent.velocity.magnitude;
        float max = Mathf.Max(0.01f, agent.speed);

        // Blend Tree expects 0..1 (Idle..Run)
        float normalized = Mathf.Clamp01(v / max);

        animator.SetFloat(speedHash, normalized, 0.08f, Time.deltaTime);
    }

    // Call this from animation event (recommended)
    public void AnimEvent_DoMeleeHit()
    {
        if (meleeAttack)
            meleeAttack.DoHit();
    }
}
