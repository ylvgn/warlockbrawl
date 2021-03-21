using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Character character;
    public UISkillIndicator indicatorController;
    private HUD HUD;

    public static PlayerController Instance => _instance;
    static PlayerController _instance = null;

    public bool canClickMouse; // 处理UI无法阻挡Raycast问题

    void Start()
    {
        if (_instance != null) {
            GameObject.Destroy(_instance); 
            Debug.LogError("重复实例化 PlayerController.instance");
        }
        _instance = this;

        character = MyUtility.GetComponent<Character>(transform);
        indicatorController = MyUtility.GetComponentInChildren<UISkillIndicator>(transform);
        HUD = MyUtility.GetComponentInChildren<HUD>(transform);
        canClickMouse = false;
    }

    void Update()
    {
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
                Vector3 dir = (hit.point - character.transform.position).normalized;
                if (character.CanIssueSkill()) {
                    var skillData = indicatorController.GetCurrentSkillData();
                    SkillProjectData skillProjectData = new SkillProjectData(character, skillData, dir);
                    character.IssueSkill(skillProjectData);
                    indicatorController.CancleSkill();
                }
            }
        }

        // 取消技能
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            indicatorController.CancleSkill();
        }

        if (Application.isEditor && character != null)
        {
            Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            if (input == Vector3.zero) return;
            character.JoyStick(input);
        }
    }
}
