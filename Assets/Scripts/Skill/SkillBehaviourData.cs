public abstract class SkillBehaviourData
{
    public Character owner;
    public SkillData skillData;
    public SkillBehaviourData(Character owner_, SkillData skillData_)
    {
        if (owner_ == null) MyUtility.MyDebug("[SkillBehaviourData] 传入owner == null");
        if (skillData_ == null) MyUtility.MyDebug("[SkillBehaviourData] 传入skillData == null");

        owner = owner_;
        skillData = skillData_;
    }
}
