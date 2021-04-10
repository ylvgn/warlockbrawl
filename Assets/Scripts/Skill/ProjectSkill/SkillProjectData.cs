using UnityEngine;

public class SkillProjectData : SkillBehaviourData
{
    public Vector3 dir;
    public Vector3 endPos;

    public SkillProjectData(Character owner_, SkillData skillData_) : base(owner_, skillData_) { }
    public SkillProjectData(Character owner_, SkillData skillData_, Vector3 endPos_) : base(owner_, skillData_)
    {
        endPos = endPos_;
        SetDir(endPos_ - owner.transform.position);
    }

    public Vector3 GetDir()
    {
        return dir;
    }

    public Vector3 GetEndPos()
    {
        return endPos;
    }

    public void SetDir(Vector3 dir_)
    {
        dir = dir_.normalized;
    }
}
