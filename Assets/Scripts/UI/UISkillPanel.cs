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

#if UNITY_EDITOR
    public KeyCode[] MyShortCutKeyCodes = new KeyCode[SkillSlotCount];
#endif

    void Awake()
    {
        if (itemPrefab == null) {
            Debug.LogError("[UISkillPanel] itemPrefab == null");
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
            Debug.LogError($"Duplicate Add skillId={skillId}");
            return;
        }
        int index = _skillIdToItemIndex.Count;
        _skillItemList[index].Init(skillData);
        _skillIdToItemIndex[skillId] = index;
    }

    public void Init(CharacterData characterData)
    {
        var skillDataList = characterData.GetAllSkillData();
        for (int i = 0; i < skillDataList.Count; i++)
            AddSkill(skillDataList[i]);
    }

    public void OnSelectSkill(int skillId)
    {
        if (PlayerController.Instance == null || PlayerController.Instance.character == null)
        {
            Debug.LogError("[OnSelectSkill] PlayerController not init ok");
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
            return;
        }

        PlayerController.Instance.indicatorController.SetData(skillData);
    }

    public int IndexToSkillId(int index)
    {
        if (index >= SkillSlotCount) return 0;
        var item = _skillItemList[index];
        return item.GetSkillId();
    }

#if UNITY_EDITOR
    public void SetShortCutKetCode(int index, KeyCode keyCode)
    {
        if (PlayerController.Instance == null || PlayerController.Instance.character == null)
        {
            Debug.LogError("[SetShortCutKetCode] PlayerController not init ok");
            return;
        }

        var character = PlayerController.Instance.character;
        for (int i = 0; i < SkillSlotCount; i++)
        {
            var skillId = IndexToSkillId(i);
            if (skillId == 0) continue;
            if (MyShortCutKeyCodes[i] == keyCode && keyCode != KeyCode.None)
            {
                var skillData = character.CharacterData.GetSkillData(skillId);
                Debug.LogError($"Set KeyCode Failed : {keyCode} --> {skillData.name} duplicate conflict!");
                return;
            }
        }

        MyShortCutKeyCodes[index] = keyCode;
        var NewKeyCodeSkillData = character.CharacterData.GetSkillData(IndexToSkillId(index));
        MyUtility.MyDebug("{0} Set KeyCode '{1}' Suc!", NewKeyCodeSkillData.name, keyCode);
    }

    void Update()
    {
        if (PlayerController.Instance == null || PlayerController.Instance.character == null)
        {
            Debug.LogError("[Update] PlayerController not init ok");
            return;
        }

        for (int i = 0; i < SkillSlotCount; i ++)
        {
            var keyCode = MyShortCutKeyCodes[i];
            if (keyCode == KeyCode.None) continue;
            var skillId = IndexToSkillId(i);
            if (skillId == 0) continue;
            if (Input.GetKeyDown(keyCode))
            {
                OnSelectSkill(skillId);
            }
        }
    }

    int MyEditingSkillId = 0;
    string newKeyCodeStr;
    private void OnGUI()
    {
        if (PlayerController.Instance == null) return;
        var character = PlayerController.Instance.character;
        if (UnityEditor.Selection.activeGameObject != character.gameObject)
        {
            if (MyEditingSkillId != 0)
            {
                MyEditingSkillId = 0;
                newKeyCodeStr = "";
            }
            return;
        }
        float groupWidth = 200f;
        float groupHeight = Screen.height - GetComponent<RectTransform>().sizeDelta.y;
        int skillCount = _skillIdToItemIndex.Count;
        float posY = 0, height = 20, spacingY = 0;
        float showHeight = skillCount * height + 20 + (skillCount - 1) * spacingY;
        float spacing = Mathf.Max(0, height, spacingY + height);
        GUI.BeginGroup(new Rect(Screen.width - groupWidth, groupHeight - showHeight, groupWidth, showHeight));
        GUI.Box(new Rect(0, 0, groupWidth, showHeight), "SkillShortCutKey Setting");
        posY += 20;
        for (int i = 0; i < SkillSlotCount; i++)
        {
            var curKeyCode = MyShortCutKeyCodes[i];
            var skillId = IndexToSkillId(i);
            if (skillId == 0) continue; // 空技能
            var skillData = character.CharacterData.GetSkillData(skillId);
            string str = curKeyCode == KeyCode.None ? "None" : ((char)(curKeyCode)).ToString().ToUpper();
            if (GUI.Button(new Rect(new Rect(0, posY, groupWidth, height)), $"{skillData.name} : {str}"))
            {
                if (MyEditingSkillId != 0) return;
                MyEditingSkillId = skillId;
            }
            posY += spacing;
        }
        GUI.EndGroup();

        // Edit key code
        if (MyEditingSkillId != 0)
        {
            var skillId = MyEditingSkillId;
            float editWidth = 200;
            float editHeight = 60;
            float editSpacingY = 0;
            Rect boxSize = new Rect(Screen.width / 2 - editWidth / 2, Screen.height / 2 - editHeight / 2, editWidth, editHeight);
            GUI.Box(boxSize, "Please Set Short Cut Key");
            var skillData = character.CharacterData.GetSkillData(skillId);
            editSpacingY += 30;
            newKeyCodeStr = GUI.TextArea(new Rect(boxSize.x, boxSize.y + editSpacingY, boxSize.width, 30), newKeyCodeStr);
            editSpacingY += 30;
            if (GUI.Button(new Rect(boxSize.x, boxSize.y + editSpacingY, boxSize.width, 20), "OK"))
            {
                int index = _skillIdToItemIndex[skillId];
                var curKeyCode = MyShortCutKeyCodes[index];
                var str = newKeyCodeStr.ToLower();
                if (str != curKeyCode.ToString() && str.Length == 1 && str[0] >= 'a' && str[0] <= 'z')
                {
                    KeyCode newKeyCode = (KeyCode)str[0];
                    SetShortCutKetCode(index, newKeyCode);
                }
                MyEditingSkillId = 0;
                newKeyCodeStr = "";
            }
        }
    }
#endif
}
