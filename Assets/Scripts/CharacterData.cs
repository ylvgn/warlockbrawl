using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterData
{
    #region RoleData
    [System.Serializable]
    public class RoleData
    {
        public string roleName;
        public int health;
        public bool isAlive;
        public float moveSpeed = 5f;
        public float rotateSmoothTime = 1f;
        public float rotateSpeedMovement = 0.5f;
        public float moveSpeedDampTime = 0.1f;
        public RoleData() { }

        public RoleData(string roleName, int health)
        {
            this.roleName = roleName;
            this.health = health;
        }
    }
    #endregion

    #region SkillData
    [System.Serializable]
    public class SkillData
    {
        public enum RangeType // 对应不同的 indicator
        {
            None = 0,
            Point = 1,
            ArcShaped = 2,
            Circle = 3,
            DirectLine = 4,
        }

        public enum SkillEffect
        {
            Poisoning,
            SlowDown,
        }

        public int id;
        public float coolDownTime;
        public bool isCoolDowning;
        public float velocity = 10f; // tmp
        public float maxAttackRadius;
        public string damageFormula;
        public RangeType rangeType;
        public SkillEffect[] skillEffects;
        public SkillData() { }

        public SkillData(int id, float coolDownTime, float maxAttackRadius = 0f, RangeType rangeType = RangeType.None, SkillEffect[] skillEffects = null)
        {
            this.id = id;
            this.coolDownTime = coolDownTime;
            this.maxAttackRadius = maxAttackRadius;
            this.rangeType = rangeType;
            this.skillEffects = skillEffects;
        }
    }
    #endregion

    public RoleData roleData { get; set; }
    public Dictionary<int, SkillData> skillDict { get; set; }

    public CharacterData() { }
    public CharacterData(RoleData roleData, List<SkillData> skillList = null)
    {
        this.roleData = roleData;
        skillDict = new Dictionary<int, SkillData>();
        foreach (var skillData in skillList)
        {
            this.skillDict[skillData.id] = skillData;
        }
    }

    public SkillData GetSkillData(int skillId)
    {
        SkillData res = null;
        this.skillDict.TryGetValue(skillId, out res);
        return res;
    }

}
