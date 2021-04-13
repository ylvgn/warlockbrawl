using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ResManager : MonoBehaviour
{
    public GameObject UIIndicatorPrefab;
    public GameObject UIHUDPrefab;
    public Transform[] CharacterBuildPoints;

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
        var playerPrefabs = Resources.Load<GameObject>("Player/Mage");
        Vector3 pos = Vector3.zero;
        if (CharacterBuildPoints!= null)
        {
            int t = Random.Range(0, 1000 + (int)Time.time);
            pos = CharacterBuildPoints[t % CharacterBuildPoints.Length].position;
        }
        GameObject playerObj = GameObject.Instantiate<GameObject>(playerPrefabs, pos, Quaternion.identity);
        var character = playerObj.GetComponent<Character>();
        var animator = playerObj.GetComponent<Animator>();
        var controller = AssetDatabase.LoadMainAssetAtPath(controllerPath) as UnityEditor.Animations.AnimatorController;
        animator.runtimeAnimatorController = controller;
        character.SetData(characterData);
        playerObj.name = characterData.name;
        return character;
    }

    public HUD BuildHUD(Character character, HealthData healthData)
    {
        HUD res = null;
        var playerHeight = character.characterController.height;
        var obj = GameObject.Instantiate<GameObject>(UIHUDPrefab, character.transform);
        res = obj.GetComponent<HUD>();
        var HUDRect = obj.GetComponent<RectTransform>();
        HUDRect.localPosition = new Vector3(0, playerHeight, 0);
        if (res != null) res.Init(healthData);
        return res;
    }

    public HUD BuildHUD(GameObject obj, HealthData healthData)
    {
        HUD res = null;
        res = GameObject.Instantiate<GameObject>(UIHUDPrefab, obj.transform).GetComponent<HUD>();
        var HUDRect = res.GetComponent<RectTransform>();
        var localScale = obj.transform.localScale;
        float height = obj.GetComponent<Collider>().bounds.size.y;
        HUDRect.localScale = new Vector3(1.0f / localScale.x, 1.0f / localScale.y, 1.0f / localScale.z);
        HUDRect.localPosition = new Vector3(0, (height / 2 * HUDRect.localScale.y) + HUDRect.localScale.y, 0);
        if (res != null) res.Init(healthData);
        return res;
    }

    public UISkillIndicator BuildSkillIndicator(Character character)
    {
        return GameObject.Instantiate<GameObject>(UIIndicatorPrefab, character.transform).GetComponent<UISkillIndicator>();
    }
}
