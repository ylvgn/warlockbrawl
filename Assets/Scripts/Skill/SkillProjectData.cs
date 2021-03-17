using UnityEngine;

public class SkillProjectData
{
    public Character owner;
    public SkillData skillData;
    public Vector3 dir;
    public SkillProjectData(Character owner_, SkillData skillData_, Vector3 dir_)
    {
        owner = owner_;
        skillData = skillData_;
        dir = dir_;
    }
}
