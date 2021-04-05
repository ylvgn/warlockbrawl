using UnityEngine;

public enum BuffType
{
    Static,
    PerSingleSecond,
}

public abstract class BuffData : IDamage
{
    public int id { get; private set; }
    public IAttackable owner { get; protected set; }
    public string name { get; protected set; }
    public float duringTime {get; protected set; }
    public float intervalTime { get; protected set; }
    public float startTime { get; protected set; }
    public BuffType BuffType { get; protected set; }
    public bool canOverlay { get; protected set; } // 是否可叠加buff
    public bool isEnable { get; protected set; }

    public BuffData(int buffId)
    {
        id = buffId;
    }

    public BuffData(int buffId, IAttackable owner_, float duringTime_, float intervalTime_)
    {
        if (duringTime_ <= 0) {
            Debug.LogError("[BuffData} 传入duringTime 必须是正数！duringTime=" + duringTime_);
        }

        if (intervalTime_ <= 0) {
            Debug.LogError("[BuffData} 传入intervalTime 必须是正数！intervalTime=" + intervalTime_);
        }

        id = buffId;
        owner = owner_;
        duringTime = duringTime_;
        intervalTime = intervalTime_;
        startTime = Time.realtimeSinceStartup;
        isEnable = true;
    }

    public virtual bool CanHandle()
    {
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
        if (id == 0) id = buffData.id;
        if (id != buffData.id) {
            Debug.LogError(string.Format("[BuffData] ResetData 传入的buffId前后不一致 before_id={0} now_id={1}", id, buffData.id));
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
    
    public IAttackable GetOwner()
    {
        return owner;
    }

    public abstract DamgeData GetDamage(IAttackable other);
}
