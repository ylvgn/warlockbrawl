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
    OnAttackNoMove,
    Spelling,
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

#region Character Attribute
    public int strength;           // 力量
    public int magic;              // 魔法
    public int endurance;          // 耐力
    public int agility;            // 敏捷
    public int physicalResistance; // 物理抗性
    public int magicResistance;    // 魔法抗性
#endregion

    private Dictionary<int, SkillData> skillIdToSkillDataDict;
    private Dictionary<int, BuffData> buffIdToBuffDataDict;

    public CharacterData() {}
    public CharacterData(string name_)
    {
        name = name_;
        HP = maxHP;
        MP = maxMP;
        skillIdToSkillDataDict = new Dictionary<int, SkillData>();
        buffIdToBuffDataDict = new Dictionary<int, BuffData>();
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
        if (skillIdToSkillDataDict.TryGetValue(skillId, out res))
            return res;

        Debug.LogError(string.Format("找不到skillId={0}的数据", skillId));
        return res;
    }

#region buff
    public BuffData GetBuffData(int buffId)
    {
        BuffData res;
        if (buffIdToBuffDataDict.TryGetValue(buffId, out res))
            return res;
        return null;
    }

    public void AddBuffs(List<BuffData> buffDataList)
    {
        if (buffDataList == null)
        {
            Debug.LogError("[CharacterData] AddBuffs 传入buffDataList == null");
            return;
        }

        for (int i = 0; i < buffDataList.Count; i++)
        {
            var addBuffData = buffDataList[i];
            int buffId = addBuffData.id;
            var buffData = GetBuffData(buffId);
            if (buffData == null) {
                buffIdToBuffDataDict.Add(buffId, addBuffData);
            } else if (buffData.canOverlay) {
                buffData.OverlayBuff(buffData);
            }
        }
    }

    public void RemoveBuffs(List<int> buffIdList)
    {
        if (buffIdList == null)
        {
            Debug.LogError("[CharacterData] RemoveBuffs 传入buffIdList == null");
            return;
        }
        for (int i = 0; i < buffIdList.Count; i ++)
        {
            int buffId = buffIdList[i];
            BuffData buffData = GetBuffData(buffId);
            if (buffData != null)
                buffIdToBuffDataDict.Remove(buffId);
        }
    }

    // 摘掉buff
    public void TakeOffBuff(int buffId)
    {
        BuffData buffData = GetBuffData(buffId);
        if (buffData != null)
            buffData.KillSelf();
    }

    public void TakeOffAllBuffs()
    {
        foreach (var item in buffIdToBuffDataDict)
            TakeOffBuff(item.Key);
    }

    public void HandleAllBuffs()
    {
        if (buffIdToBuffDataDict.Count == 0) return;

        List<int> removeBuffList = new List<int>();
        foreach (var item in buffIdToBuffDataDict)
        {
            int buffId = item.Key;
            var buffData = item.Value;
            if (buffData.IsObsolete())
            {
                buffData.KillSelf();
                removeBuffList.Add(buffId);
                continue;
            }
            if (buffData.CanHandle())
                buffData.Handle();
        }
        RemoveBuffs(removeBuffList);
    }
#endregion
}
