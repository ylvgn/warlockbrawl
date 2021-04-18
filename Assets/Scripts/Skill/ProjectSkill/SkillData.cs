using UnityEngine;

public enum RangeType
{
    None,
    Point,
    ArcShaped,
    Circle,
    DirectLine,
}

public class SkillData
{
    public readonly int id;
    public string name;
    public float coolDownTime;
    public bool isCoolDowning;
    public float flySpeed;
    public float maxRadius;                  // 伤害范围
    public float maxRange;                   // 最大射程
    public float spellingIntervalTime;       // 施放间隔时间
    public float durationTime;               // 施放持续时间
    public RangeType RangeType;              // indicator形式
    public int basicDamage;
    public bool canCharacterMove;
    #region Attribute
    public int fire = 0;
    public int ice = 0;
    public int thunder = 0;
    #endregion
    public string resPath;                   // tmp
    public GameObject resPrefab;

    public SkillData(MySkillScriptableObject.MySkillScriptableObjectData scriptableObjectData)
    {
        basicDamage = scriptableObjectData.basicDamage;
        coolDownTime = scriptableObjectData.coolDownTime;
        if (coolDownTime <= 0) {
            Debug.LogError("coolDownTime <= 0 !!");
        }
        durationTime = scriptableObjectData.durationTime;
        flySpeed = scriptableObjectData.flySpeed;
        maxRadius = scriptableObjectData.maxRadius;
        RangeType = scriptableObjectData.RangeType;
        maxRange = scriptableObjectData.maxRange;
        id = scriptableObjectData.skillId;
        name = scriptableObjectData.skillName;
        resPrefab = scriptableObjectData.resPrefab;
        spellingIntervalTime = scriptableObjectData.spellingIntervalTime;
        fire = scriptableObjectData.fire;
        ice = scriptableObjectData.ice;
        thunder = scriptableObjectData.thunder;
        canCharacterMove = true;
    }

    // 单体
    public bool IsInstance() {
        return flySpeed == 0 && (RangeType == RangeType.None || RangeType == RangeType.Point);
    }

    // 飞行弹道
    public bool IsBallistic()
    {
        return flySpeed > 0 && RangeType != RangeType.None && !IsSpelling();
    }

    public bool IsSpelling() {
        return spellingIntervalTime > 0 && durationTime > 0;
    }

    public static SkillBehaviourData MakeBehaviourData(Character owner, SkillData skillData)
    {
        if (skillData == null || owner == null)
        {
            Debug.LogError("[SkillData] MakeBehaviourData 传入参数错误");
            return null;
        }

        if (skillData.flySpeed > 0) return new SkillProjectData(owner, skillData);
        if (skillData.IsInstance()) return new SkillMountBuffData(owner, skillData);
        return null;
    }
}


