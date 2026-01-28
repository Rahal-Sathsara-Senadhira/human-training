using UnityEngine;

[System.Serializable]
public struct DamageInfo
{
    public float amount;
    public Vector3 point;
    public Vector3 direction;
    public GameObject source;   // who caused it
    public string skillName;    // punch / rifle / sword

    public DamageInfo(float amount, Vector3 point, Vector3 direction, GameObject source, string skillName)
    {
        this.amount = amount;
        this.point = point;
        this.direction = direction;
        this.source = source;
        this.skillName = skillName;
    }
}
