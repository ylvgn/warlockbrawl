using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Character character;
    private CharacterSkillIndicator indicatorController; // 技能引导UI
    private CharacterBlood characterBlood;

    public static PlayerController Instance;

    void Start()
    {
        if (Instance == null) 
            Instance = this;

        character = GetComponent<Character>();
        indicatorController = transform.GetComponentInChildren<CharacterSkillIndicator>();
        characterBlood = transform.GetComponentInChildren<CharacterBlood>();

        // 选择了某个技能
        SkillPanel.OnClickSkill += (skillId) =>
        {
            character.SelectSkill(skillId);
        };

        // 技能冷却完毕
        SkillPanel.OnCoolDownFinished += (skillId) =>
        {
            character.CoolDownFinish(skillId);
        };

        // 掉血 tmp
        Character.OnGetHurt += (characterData) =>
        {
            int health = characterData.roleData.health;
            characterBlood.SetData(health);
        };
    }

    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
        var playerTrans = character.transform;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            // 角色移动
            if (UnityEngine.Input.GetMouseButton(1))
            {
                character.Move(hit.point);
            }

            // 技能引导
            indicatorController.ShowIndicator(hit, playerTrans);

            // 释放技能
            if (character.canFireSkill() && Input.GetMouseButton(0))
            {
                Vector3 dir = (hit.point - playerTrans.position).normalized;
                character.TowardDir(dir);
                character.FireSkill(hit);
            }
        }

        // 取消技能
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            character.CancleSelectSkill();
        }
    }
}
