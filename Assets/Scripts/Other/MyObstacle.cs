using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyObstacle : MonoBehaviour, IAttackable
{
    [SerializeField] public HealthData health;

    void Start()
    {
        ResManager.Instance.BuildHUD(gameObject, health);
    }

    public void TakeDamage(int damge) {
        health.AddDeltaHP(-damge);
    }

    public int GetHP() {
        return health.HP.value;
    }
}
