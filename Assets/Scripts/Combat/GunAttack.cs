using UnityEngine;

public class GunAttack : MonoBehaviour
{
    public Transform firePoint;          // muzzle position
    public LayerMask targetLayers;
    public SkillData skill;
    public int skillLevel = 1;

    public GameObject owner;

    private float nextFireTime;

    public bool CanFire()
    {
        return skill != null && Time.time >= nextFireTime;
    }

    public void FireAt(Transform target)
    {
        if (skill == null || firePoint == null || target == null) return;
        if (!CanFire()) return;

        nextFireTime = Time.time + skill.fireCooldown;

        Vector3 dir = (target.position + Vector3.up * 1.1f) - firePoint.position;
        float range = skill.gunRange;

        if (Physics.Raycast(firePoint.position, dir.normalized, out RaycastHit hit, range))
        {
            if (((1 << hit.collider.gameObject.layer) & targetLayers) != 0)
            {
                IDamageable d = hit.collider.GetComponentInParent<IDamageable>();
                if (d != null)
                {
                    float dmg = skill.GetDamage(skillLevel);
                    d.TakeDamage(new DamageInfo(dmg, hit.point, dir.normalized, owner, skill.skillName));
                }
            }
        }

        // Later we can add muzzle flash, tracer, sound, recoil
    }

    void OnDrawGizmosSelected()
    {
        if (firePoint == null || skill == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(firePoint.position, firePoint.position + firePoint.forward * 2f);
    }
}
