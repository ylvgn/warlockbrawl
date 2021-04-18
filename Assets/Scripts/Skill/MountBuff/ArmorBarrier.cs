using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorBarrier : SkillMountBuff
{
    public override void Init(SkillMountBuffData skillMountBuffData_)
    {
        base.Init(skillMountBuffData_);
        var owner = GetOwner();
        transform.SetParent(owner.transform);
        var skillData = skillMountBuffData_.skillData;
        float height = owner.characterController.height;
        transform.localPosition = new Vector3(0, height * 0.5f, 0);
        if (SkillMountBuffData.buffData == null)
        {
            var buffData = ResManager.Instance.BuildBuffData<ArmorBarrierBuffData>(BuffType.ArmorBarrier, owner);
            owner.PutOnBuff(buffData);
            SkillMountBuffData.SetBuffData(buffData);
        }
        isEnable = true;
    }

    public override bool CheckIsEnable()
    {
        return base.CheckIsEnable() && GetBuffData<ArmorBarrierBuffData>().CanHandle();
    }

    private void OnTriggerEnter(Collider other)
    {
        var character = other.GetComponent<Character>();
        if (character) return;
        var skillProject = other.GetComponent<SkillProject>();
        if (skillProject)
        {
            var otherOwner = skillProject.GetOwner() as Character;
            if (otherOwner != GetOwner())
            {
                var buffData = GetBuffData<ArmorBarrierBuffData>();
                buffData.SetHandleInfo(skillProject);
                buffData.Handle();
            }
        }
    }

}
