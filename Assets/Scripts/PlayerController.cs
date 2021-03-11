using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Character character;

    void Start()
    {
        character = GetComponent<Character>();

        CharacterData a = new CharacterData();
        a.movespeed = 5;
        character.SetData(a);
    }

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
            {
                character.Move(hit.point);
            }
        }
            
        // test skill
        if (Input.GetKeyDown(KeyCode.Space))
        {
            character.SelectSkill(0);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            character.CancleSelectSkill();
        }
    }
}
