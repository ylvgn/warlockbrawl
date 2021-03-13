using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Character character;
    public static PlayerController Instance;

    void Start()
    {
        if (Instance == null) 
            Instance = this;

        character = GetComponent<Character>();
        SkillPanel.OnClickSkill += (skillId) =>
        {
            character.SelectSkill(skillId);
        };

        SkillPanel.OnCoolDownFinished += (skillId) =>
        {
            character.SelectSkill(skillId);
        };
    }

    void Update()
    {
        Input();
    }

    private void Input()
    {
        if (UnityEngine.Input.GetMouseButton(1))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition), out hit, Mathf.Infinity))
            {
                character.Move(hit.point);
            }
        }
    }
}
