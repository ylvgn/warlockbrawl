using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorBarrierBuffData : BuffData
{
    private SkillProject otherSkillProject;

    public ArmorBarrierBuffData(Character owner_, float duringTime_) : base(BuffType.ArmorBarrier, owner_, duringTime_)
    {
        BuffMode = BuffMode.Static;
    }

    public void SetHandleInfo(SkillProject otherSkillProject_)
    {
        otherSkillProject = otherSkillProject_;
    }

    public override void Handle()
    {
        if (base.CanHandle())
        {
            if (otherSkillProject == null)
            {
                Debug.LogError("[ArmorBarrierBuffData] Handle之前必须先赋值 SetHandleInfo ");
                return;
            }
            var orignalDir = otherSkillProject.GetDir();
            otherSkillProject.ReSetData(owner as Character, new Vector3(-orignalDir.x, orignalDir.y, -orignalDir.z));
            KillSelf();
        }
    }
}
