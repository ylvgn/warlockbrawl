using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropHPBuffData : BuffData
{
    private int currentHandleTimes;
    public int damage {get; private set;} 
    public DropHPBuffData(int buffId, Character owner_, float duringTime_, float intervalTime_, int damage_) : base(buffId, owner_, duringTime_, intervalTime_)
    {
        if (intervalTime_ <= 0)
            Debug.LogError("[DropHPBuffData] 传入intervalTime必须为正数！ intervalTime= " + intervalTime_);
        name = "掉血buff";
        BuffType = BuffType.PerSingleSecond;
        damage = damage_;
        canOverlay = true;
        currentHandleTimes = 0;
    }

    public override bool CanHandle()
    {
        bool ok = currentHandleTimes * intervalTime + startTime <= Time.realtimeSinceStartup;
        return base.CanHandle() && ok;
    }

    public override void Handle() {
        if (CanHandle()) {
            currentHandleTimes ++;
            owner.TakeDamege(damage);
        }
    }

    // 叠加伤害
    public override void OverlayBuff(BuffData buffData)
    {
        base.OverlayBuff(buffData);
        DropHPBuffData buff = buffData as DropHPBuffData;
        damage += buff.damage;
        startTime = buff.startTime;
        duringTime = buff.duringTime;
        currentHandleTimes = 0;
    }
}
