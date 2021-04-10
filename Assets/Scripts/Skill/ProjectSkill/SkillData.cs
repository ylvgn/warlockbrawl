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
    public float maxRadius;             // 伤害范围
    public float maxRange;              // 最大射程
    public float spellingIntervalTime;  // 施放间隔时间
    public float durationTime;          // 施放持续时间
    public RangeType RangeType;         // indicator形式
    public int basicDamage;
    public string resPath;

#region Attribute
    // attr
    public int fire;
    public int ice;
    public int thunder;
#endregion

    public SkillData() {}
    public SkillData(
        int id_,
        string name_,
        string resPath_,
        float coolDownTime_,
        float maxRange_ = 0,
        float maxRadius_ = 0,
        float flySpeed_ = 0,
        RangeType RangeType_ = RangeType.None,
        float durationTime_ = 0,
        float spellingIntervalTime_ = 0
        )
    {
        if (coolDownTime_ <= 0) {
            Debug.LogError("coolDownTime <= 0!!");
        }

        if (resPath_.Trim() == "") {
            Debug.LogError("未传入资源路径");
        }

        id = id_;
        name = name_;
        coolDownTime = coolDownTime_;
        maxRange = maxRange_;
        RangeType = RangeType_;
        resPath = resPath_;
        isCoolDowning = false;
        maxRadius = maxRadius_;
        durationTime = durationTime_;
        spellingIntervalTime = spellingIntervalTime_;
        flySpeed = flySpeed_;

        // ----------- temp
        basicDamage = 10;        
        fire = 0;
        ice = 0;
        thunder = 0;
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


