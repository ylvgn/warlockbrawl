using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillProject : MonoBehaviour
{
    private SkillProjectData skillProjectData;
    protected CharacterData OwnerData { get; private set; }
    protected Vector3 StartPos { get; set; }

    public virtual void Init(SkillProjectData skillProjectData_)
    {
        skillProjectData = skillProjectData_;
        OwnerData = skillProjectData.owner.CharacterData;
        StartPos = getOwner().transform.position;
    }

    public Character getOwner() {
        return skillProjectData.owner;
    }

    public SkillData getSkillData() {
        return skillProjectData.skillData;
    }

    public Vector3 getDir() {
        return skillProjectData.dir;
    }

    public abstract void OnCollisionEnter(Collision collision);
}
