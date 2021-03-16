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

    void Start()
    {
        if (_instance != null) {
            GameObject.Destroy(_instance); 
            Debug.LogError("重复出现 PlayerController.instance");
        }
        _instance = this;

        character = MyUtility.GetComponent<Character>(transform);
        indicatorController = MyUtility.GetComponentInChildren<UISkillIndicator>(transform);
        HUD = MyUtility.GetComponentInChildren<HUD>(transform);
    }

    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            // 角色移动
            if (UnityEngine.Input.GetMouseButton(1))
            {
                character.Move(hit.point);
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
                    character.TowardDir(dir);
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
    }
}
