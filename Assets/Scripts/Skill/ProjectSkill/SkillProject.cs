using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillProject : MonoBehaviour, IDamage
{
    public SkillProjectData SkillProjectData { get; protected set; }
    protected Vector3 StartPos { get; set; }
    protected Character Owner { get; set; }

    public virtual void Init(SkillProjectData skillProjectData_)
    {
        SkillProjectData = skillProjectData_;
        Owner = GetOwner() as Character; // tmp
        StartPos = Owner.transform.position;
    }

    public SkillData getSkillData() {
        return SkillProjectData.skillData;
    }

    public Vector3 getDir() {
        return SkillProjectData.GetDir();
    }

    public Vector3 getEndPos()
    {
        return SkillProjectData.endPos;
    }

    public abstract void OnCollisionEnter(Collision collision);

    public IAttackable GetOwner()
    {
        return SkillProjectData.owner;
    }

    public abstract DamgeData GetDamage(IAttackable other);
}
