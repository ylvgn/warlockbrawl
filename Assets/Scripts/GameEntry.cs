using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntry : MonoBehaviour
{
    public UISkillPanel UISkillPanel;

    // Config
    public static Dictionary<string, System.Func<SkillData>> MySkillConfig = new Dictionary<string, System.Func<SkillData>>()
    {
        { "FireBall", () => { return new SkillData(1, "Fireball", "Effect/FireBall", 3, 5, 1, 10, RangeType.DirectLine); } },
        { "SnowStorm", () => { return new SkillData(2, "SnowStorm", "Effect/SnowStorm", 5, 15, 3, 10, RangeType.Circle, 10, 2); } },
        { "ArmorBarrier", () => { return new SkillData(3, "ArmorHalo", "Effect/ArmorBarrier", 5, 0, 0, 0, RangeType.None, 10); } },
    };

    void Start()
    {

        // Test Data
        List<SkillData> skillList = new List<SkillData>()
        {
            MySkillConfig["FireBall"](),
            MySkillConfig["SnowStorm"](),
            MySkillConfig["ArmorBarrier"](),
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
        myAICharacter.LearnSkill(MySkillConfig["FireBall"]());
        ResManager.Instance.BuildHUD(myAICharacter, myAICharacter.CharacterData.health);
        myAICharacter.gameObject.AddComponent<AIController>();

        // stats
        StatsManager.Instance.AddCharacter(myCharacter);
        StatsManager.Instance.AddCharacter(myAICharacter);
    }
}
