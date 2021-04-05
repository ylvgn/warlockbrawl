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
        transform.localPosition = Vector3.zero;
        isEnable = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        var skillProject = other.GetComponent<SkillProject>();
        if (skillProject)
        {
            Debug.Log("hhhhhhhhhhhhhh"); // 先提交，太困了。。
        }
    }
}
