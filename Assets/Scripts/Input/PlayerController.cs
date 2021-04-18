using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool isEnable;
    public Character character;
    public UISkillIndicator indicatorController;
    public bool canClickMouse; // 处理UI无法阻挡Raycast问题

    public static PlayerController Instance => _instance;
    static PlayerController _instance = null;

    void Start()
    {
        if (_instance != null) {
            Debug.LogError("重复实例化 PlayerController.instance");
            GameObject.Destroy(_instance); 
        }
        _instance = this;
        character = MyUtility.GetComponent<Character>(transform);
        var healthData = new HealthData(character.CharacterData.health.HP, character.CharacterData.health.MP);
        indicatorController = MyUtility.GetComponentInChildren<UISkillIndicator>(transform);
        ResManager.Instance.BuildHUD(character, healthData);
        canClickMouse = false;
        isEnable = true;
    }

    void Update()
    {
        if (!isEnable) return;
        if (character.IsDead()) {
            indicatorController.CancelSkill();
            isEnable = false;
            return;
        }
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            // 角色移动
            if (Input.GetMouseButton(1))
            {
                if (canClickMouse) {
                    character.Move(hit.point);
                } else {
                    character.StopMove();
                }
            }

            // 技能引导
            indicatorController.Show(hit, character.transform);

            // 发动技能
            if (indicatorController.IsEnable && Input.GetMouseButton(0))
            {
                if (character.CanIssueSkill()) {
                    var skillData = indicatorController.GetCurrentSkillData();
                    SkillProjectData skillProjectData = new SkillProjectData(character, skillData, hit.point);
                    character.IssueSkill(skillProjectData);
                    indicatorController.CancelSkill();
                }
            }
        }

        // 取消技能
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            indicatorController.CancelSkill();
        }

        if (Application.isEditor && character != null)
        {
            Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            if (input == Vector3.zero) return;
            character.JoyStick(input);
        }
    }
}
