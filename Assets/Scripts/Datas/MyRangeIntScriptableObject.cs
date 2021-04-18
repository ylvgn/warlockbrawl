using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MyRangeIntScriptableObject_DefaultName", menuName = "MyGame/Create MyRangeIntScriptableObject", order = 20)]
public class MyRangeIntScriptableObject : ScriptableObject
{
    public int maxValue;
    public int minValue;
}
