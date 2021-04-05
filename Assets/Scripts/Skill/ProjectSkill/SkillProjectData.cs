using UnityEngine;

public class SkillProjectData : SkillBehaviourData
{
    public Vector3 endPos;

    public SkillProjectData(Character owner_, SkillData skillData_) : base(owner_, skillData_) { }
    public SkillProjectData(Character owner_, SkillData skillData_, Vector3 endPos_) : base(owner_, skillData_)
    {
        endPos = endPos_;
    }

    public Vector3 GetDir()
    {
        if (!owner) return Vector3.zero;
        return (endPos - owner.transform.position).normalized;
    }
}
