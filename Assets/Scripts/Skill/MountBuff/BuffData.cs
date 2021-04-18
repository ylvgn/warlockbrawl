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
    public GameObject caster { get; private set; } // tmp
    public IAttackable target { get; protected set; }
    public string name { get; protected set; }
    public float duringTime {get; protected set; }
    public float intervalTime { get; protected set; }
    public float startTime { get; protected set; }
    public float delayTime { get; protected set; }
    public BuffMode BuffMode { get; protected set; }
    public bool canStackUp { get; protected set; }
    public bool isEnable { get; protected set; }
    public int damage { get; protected set; }

    public BuffData(MyBuffScriptableObject.Buff scriptableObject, IAttackable target_)
    {
        type = scriptableObject.type;
        name = scriptableObject.name;
        BuffMode = scriptableObject.BuffMode;
        duringTime = scriptableObject.duringTime;
        if (duringTime <= 0) {
            Debug.LogError($"[BuffData] 传入duringTime必须是正数！duringTime={duringTime}");
        }
        intervalTime = scriptableObject.intervalTime;
        if (BuffMode == BuffMode.PerSingleSecond && intervalTime <= 0) {
            Debug.LogError($"[BuffData] 传入intervalTime必须为正数！intervalTime={intervalTime}");
        }
        delayTime = Mathf.Min(scriptableObject.delayTime, intervalTime);
        startTime = Time.realtimeSinceStartup + delayTime;
        damage = scriptableObject.damage;
        target = target_;
        isEnable = true;
    }

    public virtual bool CanHandle()
    {
        if (target == null)
            Debug.LogError($"[CanHandle] 未初始化完成: {type}");
        if (!isEnable) return false;
        if (IsObsolete()) return false;
        return true;
    }

    public virtual bool IsObsolete()
    {
        return Time.realtimeSinceStartup > startTime + duringTime;
    }

    public virtual void StackUpBuff(BuffData buffData)
    {
        if (!canStackUp) return;
        if (IsObsolete()) {
            duringTime = buffData.duringTime;
            intervalTime = buffData.intervalTime;
            startTime = Time.realtimeSinceStartup;
        }
        isEnable = true;
    }

    public virtual void KillSelf()
    {
        isEnable = false;
    }

    public virtual void ResetData(BuffData buffData)
    {
        if (type == BuffType.None) type = buffData.type;
        if (type != buffData.type) {
            Debug.LogError(string.Format("[BuffData] ResetData 传入的buffId前后不一致 before_buff_type={0} now_buff_type={1}", type, buffData.type));
            return;
        }

        if (buffData.target == null)
        {
            Debug.LogError("[BuffData] ResetData 传入的target == null");
            return;
        }

        target = buffData.target;
        duringTime = buffData.duringTime;
        intervalTime = buffData.intervalTime;
        startTime = Time.realtimeSinceStartup;
        isEnable = true;
    }

    public abstract void Handle();

    public override string ToString()
    {
        return $"buff:{name} target:{target} isEnable:{isEnable}";
    }
}
