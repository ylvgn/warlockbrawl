/* https://en.wikipedia.org/wiki/Wuxing_(Chinese_philosophy)
Inter-promoting
* Wood feeds Fire
* Fire produces Earth (ash, lava)
* Earth bears Metal (geological processes produce minerals)
* Metal collects Water (water vapor condenses on metal)
* Water nourishes Wood
-------------------------------------
Weakening
* Wood depletes Water
* Water rusts Metal
* Metal impoverishes Earth (overmining or over-extraction of the earth's minerals)
* Earth smothers Fire
* Fire burns Wood (forest fires)
*/

public struct DamgeData
{
    public int physicalDamage;
    public int magicalDamage;

    public DamgeData(CharacterData ownerData, SkillData skillData, CharacterData enemyData)
    {
        float fire = skillData.fire;
        float ice = skillData.ice;
        float thunder = skillData.thunder;
        float defense = enemyData.endurance;

        if (enemyData.WuXing == WuXing.Fire) {
            defense *= 1.2f;
        } else if (enemyData.WuXing == WuXing.Water) {
            defense *= 1.2f;
        } else if (enemyData.WuXing == WuXing.Earth) {
            defense *= 1.2f;
        }

        float basicMagicDamage = fire + ice + thunder;
        float basicPhysicalDamage = skillData.basicDamage;
        physicalDamage = (int)(basicMagicDamage * (1 + (ownerData.magic - enemyData.magicResistance - defense * 0.1) / 100));
        magicalDamage = (int)(basicPhysicalDamage * (1 + (ownerData.strength - enemyData.physicalResistance - defense * 0.1) / 100));
    }

    public int CalcDamage() {
        return physicalDamage + magicalDamage;
    }
}

