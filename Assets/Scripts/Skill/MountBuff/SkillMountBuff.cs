using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMountBuff : MonoBehaviour
{
    private SkillMountBuffData SkillMountBuffData;
    public SkillProjectData SkillProjectData { get; protected set; }
    protected bool isEnable;

    public virtual void Init(SkillMountBuffData skillMountBuffData_)
    {
        SkillMountBuffData = skillMountBuffData_;
        //isEnable = true; 需要子类设置isEnable  // tmp
    }

    public virtual void Update()
    {
        if (!isEnable) return;
        if (CheckIsEnable())
        {
            isEnable = false;
            GameObject.Destroy(gameObject);
        }
    }

    public BuffData GetBuffData()
    {
        return SkillMountBuffData.buffData;
    }

    public Character GetOwner()
    {
        return SkillMountBuffData.owner;
    }

    public bool CheckIsEnable()
    {
        var owner = GetOwner();
        var buffData = GetBuffData();
        if (owner.IsDead() || buffData.IsObsolete() || owner.CharacterData.GetBuffData(buffData.id) == null) return false;
        return true;
    }
}
