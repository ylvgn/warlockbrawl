using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance => _instance;
    static StatsManager _instance = null;
    public List<Character> CharacterList { get; private set;} 

    void Awake()
    {
        if (_instance != null) {
            GameObject.Destroy(_instance); 
            Debug.LogError("重复实例化 StatsManager.instance");
        }
        _instance = this;
        CharacterList = new List<Character>();
    }

    public void AddCharacter(Character character)
    {
        CharacterList.Add(character);
    }
#if UNITY_EDITOR
    void OnGUI()
    {
        if (Selection.activeGameObject != gameObject) return;
        if (!_instance) return;
        if (CharacterList.Count == 0) return;
        float groupWidth = 200f;
        float posY = 0, height = 30, spacingY = 30;
        int n = CharacterList.Count;
        GUI.BeginGroup(new Rect(Screen.width - groupWidth, 0, groupWidth, Screen.height));
        GUI.Box(new Rect(0, 0, groupWidth, height * n + 30), $"StatsManager:{n}");
        posY += 30;
        foreach (var item in CharacterList)
        {
            if (GUI.Button(new Rect(0, posY, groupWidth, height), item.name))
            {
                Selection.activeGameObject = item.gameObject;
                EditorGUIUtility.PingObject(Selection.activeGameObject);
            }
            posY += spacingY;
        }
        GUI.EndGroup();
    }
#endif
}
