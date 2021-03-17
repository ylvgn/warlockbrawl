using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance => _instance;
    static StatsManager _instance = null;
    public List<Character> characterList { get; private set;} 

    void Awake()
    {
        if (_instance != null) {
            GameObject.Destroy(_instance); 
            Debug.LogError("重复实例化 StatsManager.instance");
        }
        _instance = this;
        characterList = new List<Character>();
    }

    public void AddCharacter(Character character)
    {
        characterList.Add(character);
    }
    
}
