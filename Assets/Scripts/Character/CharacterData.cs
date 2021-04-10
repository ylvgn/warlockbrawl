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
    public bool canMove;
    public HealthData health;
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
    private Dictionary<BuffType, BuffData> buffTypeToBuffDataDict;

    public CharacterData() {}
    public CharacterData(string name_)
    {
        name = name_;
        health = new HealthData(100, 100);
        skillIdToSkillDataDict = new Dictionary<int, SkillData>();
        buffTypeToBuffDataDict = new Dictionary<BuffType, BuffData>();
    }

#region skill
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
#endregion

#region buff
    public BuffData GetBuffData(BuffType buffType)
    {
        BuffData res;
        if (buffTypeToBuffDataDict.TryGetValue(buffType, out res)) return res;
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
            var buffType = addBuffData.type;
            var buffData = GetBuffData(buffType);
            if (buffData == null)
            {
                buffTypeToBuffDataDict.Add(buffType, addBuffData);
            } else if (buffData.canOverlay)
            {
                buffData.OverlayBuff(buffData);
            }
        }
    }

    public void TakeOffBuff(BuffType buffType)
    {
        BuffData buffData = GetBuffData(buffType);
        if (buffData != null)
            buffData.KillSelf();
    }

    public void TakeOffAllBuffs()
    {
        foreach (var item in buffTypeToBuffDataDict)
        {
            TakeOffBuff(item.Key);
        }
    }

    public void HandleAllBuffs()
    {
        if (buffTypeToBuffDataDict.Count == 0) return;
        List<BuffType> removeBuffList = new List<BuffType>();
        foreach (var item in buffTypeToBuffDataDict)
        {
            var buffType = item.Key;
            var buffData = item.Value;
            if (buffData.IsObsolete())
            {
                buffData.KillSelf();
                removeBuffList.Add(buffType);
                continue;
            }
            if (buffData.CanHandle() && buffData.BuffMode == BuffMode.PerSingleSecond)
                buffData.Handle();
        }
        RemoveBuffsInner(removeBuffList);
    }

    void RemoveBuffsInner(List<BuffType> buffIdList)
    {
        if (buffIdList == null)
        {
            Debug.LogError("[CharacterData] RemoveBuffs 传入buffIdList == null");
            return;
        }
        for (int i = 0; i < buffIdList.Count; i++)
        {
            var buffType = buffIdList[i];
            BuffData buffData = GetBuffData(buffType);
            if (buffData != null)
                buffTypeToBuffDataDict.Remove(buffType);
        }
    }
    #endregion

    public override string ToString()
    {
        return $"name:{name} skillCount:{skillIdToSkillDataDict.Count} buffCount:{buffTypeToBuffDataDict.Count}";
    }
}
