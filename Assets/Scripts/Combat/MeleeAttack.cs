using UnityEngine;

[DisallowMultipleComponent]
public class MeleeAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform owner;

    [Tooltip("Optional: if set, hit will be centered in the direction of this target (fixes 'hits air' issue).")]
    [SerializeField] private Transform target;

    [Tooltip("Optional: a transform in front of the character. If set and target is null, we will use this position.")]
    [SerializeField] private Transform hitPoint;

    [Header("Hit Settings")]
    [SerializeField] private float range = 1.2f;
    [SerializeField] private float forwardOffset = 0.9f;
    [SerializeField] private float verticalOffset = 1.0f;
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float hitCooldown = 0.25f;

    private float nextHitTime;

    void Reset()
    {
        owner = transform;
        hitPoint = transform;
    }

    public void SetTarget(Transform t) => target = t;

    /// <summary>
    /// Call this from an Animation Event at the exact hit frame.
    /// </summary>
    public void DoHit()
    {
        if (Time.time < nextHitTime) return;
        nextHitTime = Time.time + hitCooldown;

        Vector3 center = GetHitCenter();
        Collider[] hits = Physics.OverlapSphere(center, range, targetLayers, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < hits.Length; i++)
        {
            if (!hits[i]) continue;

            // Don't hit self
            if (owner && (hits[i].transform == owner || hits[i].transform.IsChildOf(owner)))
                continue;

            Health h = hits[i].GetComponentInParent<Health>();
            if (h != null)
            {
                h.TakeDamage(damage);
            }
        }
    }

    private Vector3 GetHitCenter()
    {
        Transform o = owner ? owner : transform;

        // Preferred: use target direction (stable even if model forward is flipped)
        if (target)
        {
            Vector3 dir = target.position - o.position;
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.0001f) dir = o.forward;
            dir.Normalize();

            return o.position + Vector3.up * verticalOffset + dir * forwardOffset;
        }

        // Fallback: use hitPoint transform if provided
        if (hitPoint)
            return hitPoint.position;

        // Final fallback: use owner's forward
        return o.position + Vector3.up * verticalOffset + o.forward * forwardOffset;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(GetHitCenter(), range);
    }
#endif
}
