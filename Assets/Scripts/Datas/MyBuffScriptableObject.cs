using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MyBuffScriptableObject_DefaultName", menuName = "MyGame/Create MyBuffScriptableObject", order = 25)]
public class MyBuffScriptableObject : ScriptableObject
{
    [System.Serializable]
    public struct Buff
    {
        public BuffType type;
        public string name;
        public float duringTime;
        public float intervalTime;
        public float delayTime;
        public BuffMode BuffMode;
        public bool canStackUp;
        public int damage;
    }

    public Buff[] Values;
}
