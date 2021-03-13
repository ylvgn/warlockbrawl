using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillPanel : MonoBehaviour
{
    public static event Action<int> OnClickSkill;       // 选择技能
    public static event Action<int> OnCoolDownFinished; // 冷却技能完毕

    const int N = 12;
    List<SkillPanelItem> list;
    Dictionary<int, int> hash;

    private int count = 0;
    HashSet<int> set;

    void Awake()
    {
        var itemPrefab = Resources.Load<GameObject>("UI/SkillPanelItem");
        var gridTrans = transform.Find("Grid");
        list = new List<SkillPanelItem>();
        hash = new Dictionary<int, int>();

        for (int i = 0; i < N; i ++)
        {
            var obj = GameObject.Instantiate<GameObject>(itemPrefab, gridTrans);
            var item = obj.GetComponent<SkillPanelItem>();
            list.Add(item);
        }

        Character.OnFireSkill += (characterData, skillId) =>
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
        if (set.Contains(skillId))
        {
            Debug.Log("正在cd中");
            return;
        }
        set.Add(skillId);
        int index = hash[skillId];
        list[index].CoolDown(coolDownTime);
    }

    public void OnSelectSkill(int skillId)
    {
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
