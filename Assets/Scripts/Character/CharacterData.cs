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
    OnAttack,
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
    public WuXing WuXing;
    public CharacterState CharacterState;
#region Character Attribute
    public int strength;           // 力量
    public int intellect;          // 智力
    public int endurance;          // 耐力
    public int agility;            // 敏捷
    public int physicalResistance; // 物理抗性
    public int magicResistance;    // 魔法抗性
#endregion

    private Dictionary<int, SkillData> skillIdToSkillDataDict;
    private Dictionary<BuffType, BuffData> buffTypeToBuffDataDict;

    public CharacterData(MyCharacterScriptableObject.MyCharacterScriptableObjectData myCharacterScriptableObjectData)
    {
        name = myCharacterScriptableObjectData.Name;
        var hp_ = myCharacterScriptableObjectData.HP;
        var mp_ = myCharacterScriptableObjectData.MP;
        var HP = hp_? new MyRangeInt(hp_.maxValue, hp_.minValue) : default(MyRangeInt);
        var MP = mp_? new MyRangeInt(mp_.maxValue, mp_.minValue) : default(MyRangeInt);
        moveSpeed = myCharacterScriptableObjectData.MoveSpeed;
        rotateSmoothTime = myCharacterScriptableObjectData.RotateSmoothTime;
        rotateSpeedMovement = myCharacterScriptableObjectData.RotateSpeedMovement;
        moveSpeedDampTime =  myCharacterScriptableObjectData.MoveSpeedDampTime;
        health = new HealthData(HP, MP);
        WuXing = myCharacterScriptableObjectData.WuXing;
        strength = myCharacterScriptableObjectData.Strength;
        intellect = myCharacterScriptableObjectData.Intellect;
        agility = myCharacterScriptableObjectData.Agility;
        endurance = myCharacterScriptableObjectData.Endurance;
        physicalResistance = myCharacterScriptableObjectData.PhysicalResistance;
        magicResistance = myCharacterScriptableObjectData.MagicResistance;
        skillIdToSkillDataDict = new Dictionary<int, SkillData>();
        buffTypeToBuffDataDict = new Dictionary<BuffType, BuffData>();
        var skillDatas = myCharacterScriptableObjectData.skillDatas;
        if (skillDatas != null)
        {
            for (int i = 0; i < skillDatas.Values.Length; i ++)
            {
                var skillConfig = skillDatas.Values[i];
                var skillData =  new SkillData(skillConfig);
                LearnSkill(skillData);
            }
        }
        canMove = true;
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

    public List<SkillData> GetAllSkillData()
    {
        List<SkillData> res = new List<SkillData>(16);
        foreach (var item in skillIdToSkillDataDict)
        {
            res.Add(item.Value);
        }
        return res;
    }
#endregion

#region buff
    public BuffData GetBuffData(BuffType buffType)
    {
        BuffData res;
        if (buffTypeToBuffDataDict == null)
        {
            Debug.LogError("[GetBuffData] buffTypeToBuffDataDict == null");
            return null;
        }
        if (buffTypeToBuffDataDict.TryGetValue(buffType, out res))
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
        if (buffTypeToBuffDataDict == null)
            buffTypeToBuffDataDict = new Dictionary<BuffType, BuffData>();
        for (int i = 0; i < buffDataList.Count; i++)
        {
            var addBuffData = buffDataList[i];
            var buffType = addBuffData.type;
            var buffData = GetBuffData(buffType);
            if (buffData == null)
            {
                buffTypeToBuffDataDict.Add(buffType, addBuffData);
            } else if (buffData.canStackUp)
            {
                buffData.StackUpBuff(addBuffData);
            }
        }
    }

    public void TakeOffBuff(BuffType buffType)
    {
        BuffData buffData = GetBuffData(buffType);
        if (buffData != null)
        {
            buffData.KillSelf();
        }
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
        if (buffTypeToBuffDataDict == null || buffTypeToBuffDataDict.Count == 0) return;
        List<BuffType> removeBuffList = null;
        foreach (var item in buffTypeToBuffDataDict)
        {
            var buffType = item.Key;
            var buffData = item.Value;
            if (buffData.IsObsolete()) buffData.KillSelf();
            if (!buffData.isEnable)
            {
                if (removeBuffList == null) removeBuffList = new List<BuffType>(16);
                removeBuffList.Add(buffType);
                continue;
            }
            if (buffData.BuffMode == BuffMode.PerSingleSecond && buffData.CanHandle())
                buffData.Handle();
        }
        if (removeBuffList != null)
            RemoveBuffsInner(removeBuffList);
    }

    void RemoveBuffsInner(List<BuffType> buffIdList)
    {
        if (buffIdList == null)
        {
            Debug.LogError("[CharacterData] RemoveBuffsInner 传入buffIdList == null");
            return;
        }
        for (int i = 0; i < buffIdList.Count; i++)
        {
            var buffType = buffIdList[i];
            BuffData buffData = GetBuffData(buffType);
            Debug.Log($"remove inner {buffData}");
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
