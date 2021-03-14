using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillPanel : MonoBehaviour
{
    const int N = 12;                  // 技能槽总数量
    private List<SkillPanelItem> list; // 所有Item对象
    private Dictionary<int, int> hash; // 技能id -> Item下标
    private int count = 0;             // 当前拥有的技能数量
    private HashSet<int> set;          // 处于cd中的技能id

    public static event Action<int> OnClickSkill;       // 选择技能
    public static event Action<int> OnCoolDownFinished; // 冷却技能完毕

    void Awake()
    {
        var itemPrefab = Resources.Load<GameObject>("UI/SkillPanelItem");
        var gridTrans = transform.Find("Grid");
        list = new List<SkillPanelItem>();
        hash = new Dictionary<int, int>();
        set = new HashSet<int>();

        for (int i = 0; i < N; i ++)
        {
            var obj = GameObject.Instantiate<GameObject>(itemPrefab, gridTrans);
            var item = obj.GetComponent<SkillPanelItem>();
            list.Add(item);
        }

        // 释放了技能
        Character.OnFiredSkill += (characterData, skillId) =>
        {
            var skillData = characterData.GetSkillData(skillId);
            if (skillData == null)
                return;
            this.SetData(skillId, skillData.coolDownTime);
        };
    }

    public void InitData(int skillId)
    {
        list[count].Init(skillId);
        hash[skillId] = count;
        count++;
    }

    public void SetData(int skillId, float coolDownTime)
    {
        int index = hash[skillId];
        set.Add(skillId);
        list[index].CoolDown(coolDownTime);
    }

    public void OnSelectSkill(int skillId)
    {
        if (skillId == 2 || skillId == 3)
        {
            Debug.Log("<color=#00FF00>目前只写了第一个技能的逻辑~~~~</color> " + skillId); // tmp
            return;
        }

        if (set.Contains(skillId))
        {
            Debug.Log(string.Format("<color=#FFFF00>技能[{0}]还在cd中</color>", skillId));
            return;
        }

        if (OnClickSkill != null)
            OnClickSkill(skillId);
    }

    public void OnCoolDownOK(int skillId)
    {
        set.Remove(skillId);
        if (OnCoolDownFinished != null)
            OnCoolDownFinished(skillId);
    }

    void OnDestroy()
    {
        OnClickSkill = null;
        OnCoolDownFinished = null;
    }
}
