using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropHPBuffData : BuffData
{
    private int currentHandleTimes;

    public DropHPBuffData(MyBuffScriptableObject.Buff myScriptableObject, IAttackable target_) : base(myScriptableObject, target_)
    {
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
            target.TakeDamage(damageData.CalcDamage());
        }
    }

    // 叠加掉血buff伤害
    public override void StackUpBuff(BuffData buffData)
    {
        DropHPBuffData buff = buffData as DropHPBuffData;
        if (IsObsolete()) {
            intervalTime = buff.intervalTime;
            startTime = Time.realtimeSinceStartup + delayTime;
            damage = buff.damage;
        } else {
            startTime = buff.startTime;
            Debug.Log($"[StackUpBuff] olddamage={damage}, newdamage={buff.damage + damage}");
            damage += buff.damage;
        }

        duringTime = buff.duringTime;
        currentHandleTimes = 0;
        isEnable = true;
    }
}
