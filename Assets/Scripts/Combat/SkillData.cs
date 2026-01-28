using UnityEngine;

public enum SkillType { Melee, Gun }

[CreateAssetMenu(menuName = "Combat/Skill Data")]
public class SkillData : ScriptableObject
{
    public string skillName = "Punch";
    public SkillType type = SkillType.Melee;

    [Header("Damage by Level (index 0 = Level 1)")]
    public float[] damageByLevel = new float[] { 10f, 15f, 22f };

    [Header("Melee")]
    public float meleeRange = 1.6f;
    public float hitRadius = 0.35f;

    [Header("Gun")]
    public float gunRange = 20f;
    public float fireCooldown = 0.4f;

    public float GetDamage(int level)
    {
        level = Mathf.Clamp(level, 1, damageByLevel.Length);
        return damageByLevel[level - 1];
    }
}
