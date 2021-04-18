using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyObstacle : MonoBehaviour, IAttackable
{
    [SerializeField] private HealthData health;
    public MyRangeIntScriptableObject HP_scriptableObject;

    void Start()
    {
        if (HP_scriptableObject == null)
        {
            Debug.LogError("HP_scriptableObject == null");
            return;
        }

        var HP = new MyRangeInt(HP_scriptableObject.maxValue, HP_scriptableObject.minValue);
        health = new HealthData(HP);
        ResManager.Instance.BuildHUD(gameObject, new HealthData(HP));
    }

    public void TakeDamage(int damge) {
        health.AddDeltaHP(-damge);
    }

    public int GetHP() {
        return health.HP.value;
    }
}
