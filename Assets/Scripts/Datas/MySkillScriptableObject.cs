using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MySkillScriptableObject_DefaultName", menuName = "MyGame/Create MySkillScriptableObject", order = 21)]
public class MySkillScriptableObject : ScriptableObject
{
    [System.Serializable]
    public struct MySkillScriptableObjectData
    {
        public int skillId;                 // id
        public string skillName;            // 名字
        public float flySpeed;              // 飞行速度
        public float maxRadius;             // 伤害范围
        public float maxRange;              // 最大射程
        public float spellingIntervalTime;  // 施放间隔时间
        public float durationTime;          // 施放持续时间
        public float coolDownTime;          // cd时间
        public RangeType RangeType;         // indicator形式
        public int basicDamage;             // 基础伤害
        public GameObject resPrefab;        // prefab资源
        #region Attribute
        public int fire;
        public int ice;
        public int thunder;
        #endregion
    }

    [SerializeField] public MySkillScriptableObjectData[] Values;
}
