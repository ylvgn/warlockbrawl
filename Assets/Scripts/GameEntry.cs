using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntry : MonoBehaviour
{
    public UISkillPanel UISkillPanel;

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

        var myCharacter = ResManager.Instance.CreateCharacter(characterData, "Assets/Resources/Player/Animators/Mage.controller");
        foreach(var skillData in skillList) {
            myCharacter.LearnSkill(skillData);
            UISkillPanel.AddSkill(skillData);
        }

        ResManager.Instance.BuildHUD<Character>(myCharacter, myCharacter.CharacterData.maxHP);
        ResManager.Instance.BuildSkillIndicator(myCharacter);
        myCharacter.gameObject.AddComponent<PlayerController>();

        // AI
        CharacterData AICharacterData = new CharacterData("MyAICharacter");
        var myAICharacter = ResManager.Instance.CreateCharacter(AICharacterData, "Assets/Resources/Player/Animators/AIMage.controller");
        myAICharacter.SetData(AICharacterData);
        myAICharacter.LearnSkill(new SkillData(1, "Fireball", "Effect/FireBall", 3, 10, RangeType.Point));
        ResManager.Instance.BuildHUD<Character>(myAICharacter, myAICharacter.CharacterData.maxHP);
        myAICharacter.gameObject.AddComponent<AIController>();

        // stats
        StatsManager.Instance.AddCharacter(myCharacter);
        StatsManager.Instance.AddCharacter(myAICharacter);
    }

}
