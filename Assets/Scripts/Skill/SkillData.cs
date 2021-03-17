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
    public float maxRadius;     // 伤害范围
    public float maxRange;      // 最大射程
    public float spellingTime;  // 施放间隔时间
    public float durationTime;  // 施放持续时间
    public float delayTime;     // 施放延迟时间
    public RangeType RangeType; // indicator形式
    public int basicDamage;
    public string resPath;

#region Attribute
    // attr
    public int fire;
    public int ice;
    public int thunder;
#endregion

    public SkillData() {}
    public SkillData(int id_, string name_, string resPath_, float coolDownTime_, float maxRange_ = 0, RangeType RangeType_ = RangeType.None)
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

        // ----------- temp
        basicDamage = 10;
        maxRadius = 1;
        flySpeed = 10;
        fire = 0;
        ice = 0;
        thunder = 0;
        durationTime = 0;
        delayTime = 0;
        spellingTime = 0;
    }

    public bool isInstance() {
        return flySpeed == 0 && (RangeType == RangeType.None || RangeType == RangeType.Point);
    }

    public bool isSpelling() {
        return spellingTime > 0;
    }
}


