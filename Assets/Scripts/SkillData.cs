using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillData
{
    public enum RangeType
    {
        Circle,
        DirectLine,
        ArcShaped,
    }
    public enum SkillEffect
    {
        Poisoning,
        SlowDown,
    }

    public int id;
    public float coolDownTime;
    public bool isCoolDowned;
    public float maxRadius;
    public string damageFormula;
    public RangeType rangeType;
    public SkillEffect skillEffect;
}
