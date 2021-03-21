using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISkillPanel : MonoBehaviour
{
    public GameObject itemPrefab;
    const int SkillSlotCount = 12;
    private List<UISkillPanelItem> _skillItemList;
    private Dictionary<int, int> _skillIdToItemIndex;

    void Awake()
    {
        if (itemPrefab == null) {
            Debug.LogError("UISkillPanel 未绑定 itemPrefab!");
        } else {
            var gridTrans = MyUtility.GetComponent<Transform>(transform, "Grid");
            _skillItemList = new List<UISkillPanelItem>();
            _skillIdToItemIndex = new Dictionary<int, int>();

            for (int i = 0; i < SkillSlotCount; i ++)
            {
                var obj = GameObject.Instantiate<GameObject>(itemPrefab, gridTrans);
                var item = obj.GetComponent<UISkillPanelItem>();
                item.SetClickCallBack += OnSelectSkill;
                _skillItemList.Add(item);
            }
            itemPrefab.SetActive(false);
        }
    }

    public void AddSkill(SkillData skillData)
    {
        int skillId = skillData.id;
        if (_skillIdToItemIndex.ContainsKey(skillId)) {
            Debug.Log("重复添加skillId= " + skillId);
            return;
        }
        int index = _skillIdToItemIndex.Count;
        _skillItemList[index].Init(skillData);
        _skillIdToItemIndex[skillId] = index;
    }

    public void OnSelectSkill(int skillId)
    {
        if (PlayerController.Instance == null || PlayerController.Instance.character == null) {
            return;
        }
        var character = PlayerController.Instance.character;
        if (character.IsDead()) return;
        var skillData = character.CharacterData.GetSkillData(skillId);
        if (skillData.id == 0) {
            return;
        }

        if (skillData.RangeType == RangeType.None) {
            character.IssueSkill(skillData.id);
            Debug.Log("直接触发skillId= " + skillId);
            return;
        }

        PlayerController.Instance.indicatorController.SetData(skillData);
        Debug.Log("开始引导技能 skillId= " + skillId);
    }
}
