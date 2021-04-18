using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MySpawnPointsScriptableObject_DefaultName", menuName = "MyGame/Create MySpawnPointsScriptableObject", order = 24)]
public class MySpawnPointsScriptableObject : ScriptableObject
{
    [System.Serializable]
    public class Point
    {
        public float x, y, z;
    }

    [SerializeField]
    public Point[] Values;
}
