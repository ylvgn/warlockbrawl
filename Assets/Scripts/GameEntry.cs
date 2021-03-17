using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntry : MonoBehaviour
{
    public UISkillPanel UISkillPanel;
    public FloorController FloorController;
    public GameObject UIIndicatorPrefab;
    public GameObject UIHUDPrefab;

    void Start()
    {
        // Test Data
        List<SkillData> skillList = new List<SkillData>()
        {
            new SkillData(1, "Fireball", "Effect/FireBall", 3, 10, RangeType.Point),
            new SkillData(2, "RecoverHP", "Effect/FireBall", 4),
            new SkillData(3, "SnowStorm", "Effect/FireBall", 5, 10, RangeType.Circle),
        };
        CharacterData characterData = new CharacterData("MyCharacter");

        var myCharacter = CreateCharacter(characterData);
        foreach(var skillData in skillList) {
            myCharacter.LearnSkill(skillData);
            UISkillPanel.AddSkill(skillData);
        }

        BuildHUD(myCharacter);
        BuildSkillIndicator(myCharacter);
        myCharacter.gameObject.AddComponent<PlayerController>();
        StatsManager.Instance.AddCharacter(myCharacter);
    }

    Character CreateCharacter(CharacterData characterData)
    {
        var playerPrefabs = Resources.Load<GameObject>(("Player/Mage"));
        GameObject playerObj = GameObject.Instantiate<GameObject>(playerPrefabs, Vector3.zero, Quaternion.identity);
        var character = playerObj.GetComponent<Character>();
        character.SetData(characterData);
        return character;
    }

    HUD BuildHUD(Character character)
    {
        var playerHeight = character.characterController.height;
        var obj = GameObject.Instantiate<GameObject>(UIHUDPrefab, character.transform);
        var res = obj.GetComponent<HUD>();
        var HUDRect = obj.GetComponent<RectTransform>();
        HUDRect.localPosition = new Vector3(0, playerHeight, 0);
        res.Init(character.CharacterData.maxHP);
        return res;
    }

    UISkillIndicator BuildSkillIndicator(Character character)
    {
        var res = GameObject.Instantiate<GameObject>(UIIndicatorPrefab, character.transform).GetComponent<UISkillIndicator>();
        return res;
    }
}
