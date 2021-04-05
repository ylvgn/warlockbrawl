public interface IDamage
{
    IAttackable GetOwner(); // 伤害来源
    DamgeData GetDamage(IAttackable other); // 击中other
}
