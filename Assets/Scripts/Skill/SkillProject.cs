using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillProject : MonoBehaviour
{
    public SkillProjectData SkillProjectData { get; protected set; }
    protected Vector3 StartPos { get; set; }

    public virtual void Init(SkillProjectData skillProjectData_)
    {
        SkillProjectData = skillProjectData_;
        StartPos = getOwner().transform.position;
    }

    public Character getOwner() {
        return SkillProjectData.owner;
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
}
