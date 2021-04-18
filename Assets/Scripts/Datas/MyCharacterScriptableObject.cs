using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MyCharacterScriptableObject_DefaultName", menuName = "MyGame/Create MyCharacterScriptableObject", order = 22)]
public class MyCharacterScriptableObject : ScriptableObject
{
    [System.Serializable]
    public class MyCharacterScriptableObjectData
    {
        public string Name;
        public WuXing WuXing;
        public MyRangeIntScriptableObject HP;
        public MyRangeIntScriptableObject MP;
        public float MoveSpeed;
        public float RotateSmoothTime;
        public float RotateSpeedMovement;
        public float MoveSpeedDampTime;
        #region attribute
        public int Strength;             // 力量
        public int Intellect;            // 魔法
        public int Endurance;            // 耐力
        public int Agility;              // 敏捷
        public int PhysicalResistance;   // 物理抗性
        public int MagicResistance;      // 魔法抗性
        #endregion
        public int ice;
        public int fire;
        public int thunder;
        public int dark;
        public RuntimeAnimatorController animatorController;
        public GameObject prefab;
        public MySkillScriptableObject skillDatas;
    }
    
    [SerializeField] public MyCharacterScriptableObjectData[] Values;
}
