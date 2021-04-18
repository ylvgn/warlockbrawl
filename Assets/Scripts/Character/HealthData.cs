using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HealthData
{
    [SerializeField] public MyRangeInt HP;
    [SerializeField] public MyRangeInt MP;

    public HealthData(MyRangeInt HP_, MyRangeInt MP_ = default(MyRangeInt))
    {
        HP = HP_;
        MP = MP_;
    }

    public void AddDeltaHP(int delta)
    {
        HP.AddValue(delta);
    }

    public void AddDeltaMP(int delta)
    {
        MP.AddValue(delta);
    }

    public void SetHP(int HP_) {
        HP.SetValue(HP_);
    }

    public void SetMP(int MP_) {
        MP.SetValue(MP_);
    }

    public int GetMaxHP()
    {
        return HP.max;
    }

    public int GetMaxMP()
    {
        return MP.max;
    }

    public override string ToString()
    {
        return $"HP[{HP.min}, {HP.max}]:{HP.value}\nMP[{MP.min}, {MP.max}]:{MP.value}";
    }
}
