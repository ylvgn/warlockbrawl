using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntry : MonoBehaviour
{
    public UISkillPanel UISkillPanel;

    // Config
    public static Dictionary<string, SkillData> MySkillConfig = new Dictionary<string, SkillData>()
    {
        { "FireBall", new SkillData(1, "Fireball", "Effect/FireBall", 3, 15, 1, RangeType.DirectLine) },
        { "SnowStorm", new SkillData(2, "SnowStorm", "Effect/SnowStorm", 5, 15, 3, RangeType.Circle, 10, 2) },
        { "ArmorBarrier", new SkillData(3, "ArmorHalo", "Effect/ArmorBarrier", 5) },
    };

    void Start()
    {
        // Test Data
        List<SkillData> skillList = new List<SkillData>()
        {
            MySkillConfig["FireBall"],
            MySkillConfig["SnowStorm"],
            MySkillConfig["ArmorBarrier"],
        };
        CharacterData characterData = new CharacterData("MyCharacter");

        var myCharacter = ResManager.Instance.CreateCharacter(characterData, "Assets/Resources/Player/Animators/Mage.controller");
        foreach(var skillData in skillList) {
            myCharacter.LearnSkill(skillData);
            UISkillPanel.AddSkill(skillData);
        }

        ResManager.Instance.BuildHUD(myCharacter, myCharacter.CharacterData.health);
        ResManager.Instance.BuildSkillIndicator(myCharacter);
        myCharacter.gameObject.AddComponent<PlayerController>();

        // AI
        CharacterData AICharacterData = new CharacterData("MyAICharacter");
        var myAICharacter = ResManager.Instance.CreateCharacter(AICharacterData, "Assets/Resources/Player/Animators/AIMage.controller");
        myAICharacter.SetData(AICharacterData);
        myAICharacter.LearnSkill(new SkillData(1, "Fireball", "Effect/FireBall", 3, 100, 5, RangeType.Point));
        ResManager.Instance.BuildHUD(myAICharacter, myAICharacter.CharacterData.health);
        myAICharacter.gameObject.AddComponent<AIController>();

        // stats
        StatsManager.Instance.AddCharacter(myCharacter);
        StatsManager.Instance.AddCharacter(myAICharacter);
    }
}
