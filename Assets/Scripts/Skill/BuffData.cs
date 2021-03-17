using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffType
{
    Static,
    PerSingleSecond,
}

public abstract class BuffData
{
    public readonly int id;
    public string name {get; protected set;}
    protected Character owner;
    public float duringTime {get; protected set;}
    protected float intervalTime;
    public float startTime {get; protected set; }
    public BuffType BuffType {get; protected set; }
    public bool canOverlay {get; protected set;} // 是否可叠加buff (tmp)
    public bool isEnable {get; protected set; }

    public BuffData(int buffId, Character owner_, float duringTime_, float intervalTime_ = 0f)
    {
        if (duringTime_ <= 0)
            Debug.LogError("[BuffData} 传入duringTime 必须是正数！duringTime=" + duringTime_);
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
        if (owner == null || owner.IsDead()) {
            return false;
        }
        if (IsObsolete()) {
            return false;
        }
        return true;
    }

    public virtual bool IsObsolete()
    {
        return Time.realtimeSinceStartup > startTime + duringTime;
    }

    public virtual void OverlayBuff(BuffData buffData)
    {
        if (!canOverlay) return;
        isEnable = true;
    }

    public virtual void KillSelf() {
        isEnable = false;
    }

    public abstract void Handle();
}
