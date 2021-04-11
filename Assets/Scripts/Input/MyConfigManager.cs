using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MyConfigManager : MonoBehaviour
{
    private bool isEnable = false;
    public bool isShowTips = true;
    Dictionary<string, GameObject> allConfig;
    [Range(100, 200)]  public float groupWidth = 100;
    [Range(20, 100)] public float buttonHeight = 20;
    [Range(0, 10)] public float spacingY = 0;
    [HideInInspector] public GUIStyle tipsStyle;

    [Header("ALL CONFIG")]
    public FloorController floorController;
    public StatsManager StatsManager;
    public ResManager ResManager;
    public MyGameManager MyGameManager;

    void Start()
    {
        allConfig = new Dictionary<string, GameObject>()
        {
            { "FloorController", floorController.gameObject },
            { "StateManager", StatsManager.gameObject },
            { "ResManager", ResManager.gameObject }, // tmp
        };

        Selection.activeGameObject = gameObject;
        isEnable = true;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F1))
        {
            isEnable = !isEnable;
            if (isEnable) Selection.activeGameObject = gameObject;
        }
    }

    void OnGUI()
    {
        if (!isEnable) return;
        if (Selection.activeGameObject == gameObject)
        {
            if (isShowTips)
            {
                GUI.Box(new Rect(100, 0, Screen.width - groupWidth, 30), "Short Cut Key: 'Left Ctrl + F1' ", tipsStyle);
            }
            GUI.BeginGroup (new Rect (Screen.width - groupWidth, 0, groupWidth, Screen.height));
            float height = Mathf.Min(Screen.height / allConfig.Count, buttonHeight);
            float posY = 0;
            float spacing = spacingY + height;
            foreach (var item in allConfig)
            {
                if (GUI.Button(new Rect(0, posY, groupWidth, height), item.Key))
                {
                    Selection.activeGameObject = item.Value;
                    EditorGUIUtility.PingObject(Selection.activeGameObject);
                }
                posY += spacing;
            }
            GUI.EndGroup();
            if (GUI.Button(new Rect(0, 0, 100, 100), "ReLoad"))
            {
                if (!MyGameManager) return;
                MyGameManager.ResetStart();
            }
        }
        else // 在其他config界面
        {
            if (GUI.Button(new Rect(0, 0, 100, 100), "Back"))
            {
                Selection.activeGameObject = gameObject;
                EditorGUIUtility.PingObject(Selection.activeGameObject);
            }
            if (GUI.Button(new Rect(0, 100, 100, 100), "ReLoad"))
            {
                if (!MyGameManager) return;
                MyGameManager.ResetStart(); // tmp
            }
        }
    }
}
