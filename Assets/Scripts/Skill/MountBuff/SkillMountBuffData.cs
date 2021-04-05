public enum SkillMountBuffAnimType // 动画表现
{
    Once,
    Always,
}

public class SkillMountBuffData : SkillBehaviourData
{
    public BuffData buffData { get; protected set; }
    public SkillMountBuffAnimType SkillMountBuffAnimType { get; protected set; }
    public SkillMountBuffData(Character owner_, SkillData skillData_) : base(owner_, skillData_) { }
    public SkillMountBuffData(Character owner_, SkillData skillData_, BuffData buffData_, SkillMountBuffAnimType skillMountBuffAnimType_ = SkillMountBuffAnimType.Once) : base(owner_, skillData_)
    {
        SetBuffData(buffData_);
        SetSkillMountBuffAnimType(skillMountBuffAnimType_);
    }

    public void SetBuffData(BuffData buffData_)
    {
        buffData = buffData_;
    }

    public void SetSkillMountBuffAnimType(SkillMountBuffAnimType skillMountBuffAnimType_)
    {
        SkillMountBuffAnimType = skillMountBuffAnimType_;
    }
}
