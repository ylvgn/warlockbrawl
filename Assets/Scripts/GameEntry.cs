using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntry : MonoBehaviour
{
    public UISkillPanel UISkillPanel;
    public GameObject UIIndicatorPrefab;
    public MyCharacterScriptableObject CharacterScriptableObjectData;
    public MySpawnPointsScriptableObject CharacterSpawnPointsScriptableObject;

    Vector3 GetRandomSpawnPoint()
    {
        if (CharacterSpawnPointsScriptableObject == null) return Vector3.zero;
        var SpawnList = CharacterSpawnPointsScriptableObject.Values;
        if (SpawnList.Length == 0) return Vector3.zero;
        int t = Random.Range(0, 1000 + (int)Time.time);
        var pos = SpawnList[t % SpawnList.Length];
        return new Vector3(pos.x, pos.y, pos.z);
    }

    void Start()
    {
        if (CharacterScriptableObjectData == null) {
            Debug.LogError("CharacterScriptableObjectData == null");
            return;
        }
        if (UIIndicatorPrefab == null) {
            Debug.LogError("UIIndicatorPrefab == null");
            return;
        }
        if (UISkillPanel == null) {
            Debug.LogError("UISkillPanel == null");
            return;
        }

        // MyCharacter
        var myCharacter = ResManager.Instance.CreateCharacter(CharacterScriptableObjectData.Values[0], GetRandomSpawnPoint());
        GameObject.Instantiate<GameObject>(UIIndicatorPrefab, myCharacter.transform);
        myCharacter.gameObject.AddComponent<PlayerController>();
        UISkillPanel.Init(myCharacter.CharacterData);

        // AI
        var myAICharacter = ResManager.Instance.CreateCharacter(CharacterScriptableObjectData.Values[1], GetRandomSpawnPoint());
        myAICharacter.SetData(myAICharacter.CharacterData);
        myAICharacter.gameObject.AddComponent<AIController>();

        // stats
        StatsManager.Instance.AddCharacter(myCharacter);
        StatsManager.Instance.AddCharacter(myAICharacter);
    }
}
