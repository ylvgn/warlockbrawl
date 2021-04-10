using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropHPBuffData : BuffData
{
    public int damage { get; private set; }
    private int currentHandleTimes;

    public DropHPBuffData(Character owner_, float duringTime_, float intervalTime_, int damage_)
        : base(BuffType.DropHP, owner_, duringTime_, BuffMode.PerSingleSecond, intervalTime_)
    {
        if (intervalTime_ <= 0)
            Debug.LogError($"[DropHPBuffData] 传入intervalTime必须为正数！intervalTime={intervalTime_}");
        damage = damage_;
        canOverlay = true;
        currentHandleTimes = 0;
    }

    public override bool CanHandle()
    {
        bool ok = currentHandleTimes * intervalTime + startTime <= Time.realtimeSinceStartup;
        return base.CanHandle() && ok;
    }

    public override void Handle()
    {
        if (CanHandle()) {
            currentHandleTimes ++;
            DamgeData damageData = new DamgeData(damage, damage); // tmp
            owner.TakeDamage(damageData.CalcDamage());
        }
    }

    // 叠加掉血buff伤害
    public override void OverlayBuff(BuffData buffData)
    {
        DropHPBuffData buff = buffData as DropHPBuffData;
        if (IsObsolete()) {
            intervalTime = buff.intervalTime;
            startTime = Time.realtimeSinceStartup;
            damage = buff.damage;
        } else {
            startTime = buff.startTime;
            damage += buff.damage;
        }

        duringTime = buff.duringTime;
        currentHandleTimes = 0;
        isEnable = true;
    }
}
