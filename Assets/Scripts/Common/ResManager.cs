using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResManager : MonoBehaviour
{
    public GameObject UIHUDPrefab;
    public MyBuffScriptableObject myBuffScriptableObject;
    public Dictionary<BuffType, MyBuffScriptableObject.Buff> allBuffConfigs;

    public static ResManager Instance => _instance;
    static ResManager _instance = null;

    void Awake()
    {
        if (_instance != null) {
            GameObject.Destroy(_instance);
            Debug.LogError("重复实例化 ResManager.instance");
        }
        _instance = this;

        if (myBuffScriptableObject == null) {
            Debug.LogError("ResManager的myBuffScriptableObject未绑定配置");
        } else {
            allBuffConfigs = new Dictionary<BuffType, MyBuffScriptableObject.Buff>();
            var buffConfigs = myBuffScriptableObject.Values;
            for (int i = 0; i < buffConfigs.Length; i++)
            {
                var buff = buffConfigs[i];
                allBuffConfigs.Add(buff.type, buff);
            }
        }
    }

    // ResManager.Instance.CreateCharacter(characterData, "Assets/Resources/Player/Animators/Mage.controller");
    public Character CreateCharacter(CharacterData characterData, string controllerPath)
    {
        if (controllerPath == null || controllerPath.Trim() == "") {
            Debug.LogError("[CreateCharacter] 传入controllerPath为空");
            return null;
        }
        var playerPrefabs = Resources.Load<GameObject>("Player/Mage");
        GameObject playerObj = GameObject.Instantiate<GameObject>(playerPrefabs, Vector3.zero, Quaternion.identity);
        var character = playerObj.GetComponent<Character>();
        var animator = playerObj.GetComponent<Animator>();
#if UNITY_EDITOR
        var controller = UnityEditor.AssetDatabase.LoadMainAssetAtPath(controllerPath) as UnityEditor.Animations.AnimatorController;
        animator.runtimeAnimatorController = controller;
#endif
        character.SetData(characterData);
        playerObj.name = characterData.name;
        return character;
    }

    public Character CreateCharacter(MyCharacterScriptableObject.MyCharacterScriptableObjectData scriptableObjectData, Vector3 pos)
    {
        var playerPrefabs = scriptableObjectData.prefab;
        if (playerPrefabs == null) {
            Debug.LogError("[CreateCharacter] prefab未赋值");
            return null;
        }
        CharacterData characterData = new CharacterData(scriptableObjectData);
        GameObject playerObj = GameObject.Instantiate<GameObject>(playerPrefabs, pos, Quaternion.identity);
        var character = playerObj.GetComponent<Character>();
        var animator = playerObj.GetComponent<Animator>();
        animator.runtimeAnimatorController = scriptableObjectData.animatorController;
        character.SetData(characterData);
        playerObj.name = characterData.name;
        return character;
    }

    public HUD BuildHUD(Character character, HealthData healthData)
    {
        var playerHeight = character.characterController.height;
        var obj = GameObject.Instantiate<GameObject>(UIHUDPrefab, character.transform);
        HUD res = obj.GetComponent<HUD>();
        var HUDRect = obj.GetComponent<RectTransform>();
        HUDRect.localPosition = new Vector3(0, playerHeight, 0);
        if (res != null) res.Init(healthData);
        return res;
    }

    public HUD BuildHUD(GameObject obj, HealthData healthData)
    {
        HUD res = GameObject.Instantiate<GameObject>(UIHUDPrefab, obj.transform).GetComponent<HUD>();
        var HUDRect = res.GetComponent<RectTransform>();
        var localScale = obj.transform.localScale;
        float height = obj.GetComponent<Collider>().bounds.size.y;
        HUDRect.localScale = new Vector3(1.0f / localScale.x, 1.0f / localScale.y, 1.0f / localScale.z);
        HUDRect.localPosition = new Vector3(0, (height / 2 * HUDRect.localScale.y) + HUDRect.localScale.y, 0);
        if (res != null) res.Init(healthData);
        return res;
    }

    public T BuildBuffData<T>(BuffType type, IAttackable target)
        where T : BuffData
    {
        MyBuffScriptableObject.Buff buffConfig;
        if (allBuffConfigs.TryGetValue(type, out buffConfig))
        {
            return (T)System.Activator.CreateInstance(typeof(T), buffConfig, target);
        }
        Debug.LogError($"[GetBuffConfig] 找不到{type}的配置");
        return default(T);
    }
}
