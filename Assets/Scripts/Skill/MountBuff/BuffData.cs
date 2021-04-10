using UnityEngine;

public enum BuffMode
{
    Static,
    PerSingleSecond,
}

public enum BuffType
{
    None,
    DropHP,
    ArmorBarrier,
}

[System.Serializable]
public abstract class BuffData
{
    public BuffType type { get; private set; }
    public IAttackable owner { get; protected set; }
    public string name { get; protected set; }
    public float duringTime {get; protected set; }
    public float intervalTime { get; protected set; }
    public float startTime { get; protected set; }
    public BuffMode BuffMode { get; protected set; }
    public bool canOverlay { get; protected set; } // 是否可叠加buff
    public bool isEnable { get; protected set; }

    public BuffData(BuffType type_, IAttackable owner_, float duringTime_, BuffMode buffMode_ = BuffMode.Static, float intervalTime_ = 0)
    {
        if (duringTime_ <= 0) {
            Debug.LogError("[BuffData} 传入duringTime 必须是正数！duringTime=" + duringTime_);
        }

        if (buffMode_ == BuffMode.PerSingleSecond && intervalTime_ <= 0) {
            Debug.LogError("[BuffData} 传入intervalTime 必须是正数！intervalTime=" + intervalTime_);
        }

        type = type_;
        name = type_.ToString();
        owner = owner_;
        BuffMode = buffMode_;
        duringTime = duringTime_;
        intervalTime = intervalTime_;
        startTime = Time.realtimeSinceStartup;
        isEnable = true;
    }

    public virtual bool CanHandle()
    {
        if (owner == null)
        {
            Debug.LogError($"[{type}]未初始化完成");
        }
        if (!isEnable) return false;
        if (IsObsolete()) return false;
        return true;
    }

    public virtual bool IsObsolete()
    {
        return Time.realtimeSinceStartup > startTime + duringTime;
    }

    public virtual void OverlayBuff(BuffData buffData)
    {
        if (!canOverlay) return;
        if (IsObsolete()) {
            duringTime = buffData.duringTime;
            intervalTime = buffData.intervalTime;
            startTime = Time.realtimeSinceStartup;
        }
        isEnable = true;
    }

    public virtual void KillSelf() {
        isEnable = false;
    }

    public virtual void ResetData(BuffData buffData)
    {
        if (type == BuffType.None) type = buffData.type;
        if (type != buffData.type) {
            Debug.LogError(string.Format("[BuffData] ResetData 传入的buffId前后不一致 before_buff_type={0} now_buff_type={1}", type, buffData.type));
            return;
        }

        if (buffData.owner == null)
        {
            Debug.LogError("[BuffData] ResetData 传入的owner == null");
            return;
        }

        owner = buffData.owner;
        duringTime = buffData.duringTime;
        intervalTime = buffData.intervalTime;
        startTime = Time.realtimeSinceStartup;
        isEnable = true;
    }

    public abstract void Handle();
}
