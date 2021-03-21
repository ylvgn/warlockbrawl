using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WuXing
{ 
    Wood,
    Earth,
    Water,
    Fire,
    Metal,
}

public enum CharacterState
{
    Idle,
    Moving,
    OnAttack,
    Dead,
}

[System.Serializable]
public class CharacterData
{
    public string name;
    public bool isAlive;
    public bool canMove;
    public int HP;
    public int MP;
    public int maxHP = 100;
    public int maxMP = 100;
    public float moveSpeed = 5f;
    public float rotateSmoothTime = 1f;
    public float rotateSpeedMovement = 0.5f;
    public float moveSpeedDampTime = 0.1f;

    public WuXing WuXing = WuXing.Earth;
    public CharacterState CharacterState;

#region Character Attitude
    public int strength;           // 力量
    public int magic;              // 魔法
    public int endurance;          // 耐力
    public int agility;            // 敏捷
    public int physicalResistance; // 物理抗性
    public int magicResistance;    // 魔法抗性
#endregion

    private Dictionary<int, SkillData> skillIdToSkillDataDict;

    public CharacterData() {}
    public CharacterData(string name_)
    {
        name = name_;
        HP = maxHP;
        MP = maxMP;
        skillIdToSkillDataDict = new Dictionary<int, SkillData>();
    }

    public void LearnSkill(SkillData skillData)
    {
        int skillId = skillData.id;
        if (skillIdToSkillDataDict.ContainsKey(skillId)){
            Debug.Log("已经学习过 skillid= " + skillId);
            return;
        }
        skillIdToSkillDataDict[skillId] = skillData;
    }

    public SkillData GetSkillData(int skillId)
    {
        SkillData res;
        if (skillIdToSkillDataDict.TryGetValue(skillId, out res)) {
            return res;
        }

        Debug.LogError(string.Format("找不到skillId={0}的数据", skillId));
        return res;
    }

}
