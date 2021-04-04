using UnityEngine;

public class SkillProjectData
{
    public Character owner;
    public SkillData skillData;
    public Vector3 endPos;

    public SkillProjectData(Character owner_, SkillData skillData_, Vector3 endPos_)
    {
        owner = owner_;
        skillData = skillData_;
        endPos = endPos_;
    }

    public SkillProjectData(Character owner_, SkillData skillData_)
    {
        owner = owner_;
        skillData = skillData_;
    }

    public Vector3 GetDir()
    {
        if (!owner) return Vector3.zero;
        return (endPos - owner.transform.position).normalized;
    }
}
