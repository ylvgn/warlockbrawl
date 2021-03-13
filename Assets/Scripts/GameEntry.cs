using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntry : MonoBehaviour
{
    public SkillPanel skillPanel;

    void Start()
    {
        // Test Data
        List<CharacterData.SkillData> skillList = new List<CharacterData.SkillData>()
        {
            new CharacterData.SkillData(1, 5, 10),
            new CharacterData.SkillData(2, 6),
            new CharacterData.SkillData(3, 7, 5),
        };
        CharacterData data = new CharacterData(new CharacterData.RoleData("PlayerA", 100), skillList);

        // build Model
        var Player = CreateCharacter(data);
        Player.AddComponent<PlayerController>();

        // build UI
        for (int i = 0; i < skillList.Count; i++)
            skillPanel.InitData(skillList[i].id);

    }

    GameObject CreateCharacter(CharacterData characterData)
    {
        // player obj
        var playerPrefabs = Resources.Load<GameObject>(("Player/Mage"));
        GameObject playerObj = GameObject.Instantiate<GameObject>(playerPrefabs, Vector3.zero, Quaternion.identity);
        var character = playerObj.GetComponent<Character>();
        character.SetData(characterData);

        // blood UI
        var bloodPrefabs = Resources.Load<GameObject>(("UI/CharacterBlood"));
        var playerHeight = character.characterController.height;
        var playerBlood = GameObject.Instantiate<GameObject>(bloodPrefabs, playerObj.transform);
        var bloodController = playerBlood.GetComponent<CharacterBlood>();
        var playerBloodRect = playerBlood.GetComponent<RectTransform>();
        playerBloodRect.localPosition = new Vector3(0, playerHeight, 0);
        bloodController.Init(characterData.roleData.health);
        return playerObj;
    }
}
