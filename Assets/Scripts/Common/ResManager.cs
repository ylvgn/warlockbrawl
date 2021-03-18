using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ResManager : MonoBehaviour
{
    public GameObject UIIndicatorPrefab;
    public GameObject UIHUDPrefab;

    public static ResManager Instance => _instance;
    static ResManager _instance = null;

    void Awake()
    {
        if (_instance != null) {
            GameObject.Destroy(_instance); 
            Debug.LogError("重复实例化 ResManager.instance");
        }
        _instance = this;
    }

    public Character CreateCharacter(CharacterData characterData, string controllerPath)
    {
        var playerPrefabs = Resources.Load<GameObject>(("Player/Mage"));
        GameObject playerObj = GameObject.Instantiate<GameObject>(playerPrefabs, Vector3.zero, Quaternion.identity);
        var character = playerObj.GetComponent<Character>();
        var animator = playerObj.GetComponent<Animator>();
        var controller = AssetDatabase.LoadMainAssetAtPath(controllerPath) as UnityEditor.Animations.AnimatorController;
        animator.runtimeAnimatorController = controller;
        character.SetData(characterData);
        playerObj.name = characterData.name;
        return character;
    }

    public HUD BuildHUD<T>(T go, int maxHP)
    {
        HUD res = null;
        if (typeof(T) == typeof(Character)) {
            var character = go as Character;
            var playerHeight = character.characterController.height;
            var obj = GameObject.Instantiate<GameObject>(UIHUDPrefab, character.transform);
            res = obj.GetComponent<HUD>();
            var HUDRect = obj.GetComponent<RectTransform>();
            HUDRect.localPosition = new Vector3(0, playerHeight, 0);
        } else if (typeof(T) == typeof(GameObject)) {
            var obj = go as GameObject;
            res = GameObject.Instantiate<GameObject>(UIHUDPrefab, obj.transform).GetComponent<HUD>();
            var HUDRect = res.GetComponent<RectTransform>();
            float meshHeight = obj.GetComponent<MeshFilter>().sharedMesh.bounds.size.y;
            float scaleY =transform.lossyScale.y;
            float height = meshHeight * scaleY;
            HUDRect.localPosition = new Vector3(0, height, 0);
        }
        if (res != null) res.Init(maxHP);
        return res;
    }

    public UISkillIndicator BuildSkillIndicator(Character character)
    {
        return GameObject.Instantiate<GameObject>(UIIndicatorPrefab, character.transform).GetComponent<UISkillIndicator>();
    }

    // tmp
    public BuffData CreateBuff(int buffId, Character owner)
    {
        if (buffId == 1) {
            return new DropHPBuffData(1, owner, 100, 3, 5);
        }
        return null;
    }

}
