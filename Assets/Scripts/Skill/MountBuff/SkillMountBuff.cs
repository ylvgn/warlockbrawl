using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMountBuff : MonoBehaviour
{
    public SkillMountBuffData SkillMountBuffData { get; protected set; }
    protected bool isEnable;

    public virtual void Init(SkillMountBuffData skillMountBuffData_)
    {
        SkillMountBuffData = skillMountBuffData_;
    }

    public virtual void Update()
    {
        if (!isEnable) return;
        if (!CheckIsEnable())
        {
            Debug.Log($"{SkillMountBuffData.buffData.type} 已过期" );
            GameObject.Destroy(gameObject);
            isEnable = false;
        }
    }

    public T GetBuffData<T>() where T : BuffData
    {
        return SkillMountBuffData.buffData as T;
    }

    public Character GetOwner()
    {
        return SkillMountBuffData.owner;
    }

    public virtual bool CheckIsEnable()
    {
        var owner = GetOwner();
        var buffData = GetBuffData<BuffData>();
        if (owner.IsDead() || buffData.IsObsolete() || owner.CharacterData.GetBuffData(buffData.type) == null) return false;
        return true;
    }
}
