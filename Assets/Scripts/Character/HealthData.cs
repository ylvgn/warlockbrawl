[System.Serializable]
public class HealthData
{
    public int HP;
    public int MP;
    public int maxHP;
    public int maxMP;

    public HealthData(int maxHP_, int maxMP_ = 0) {
        maxHP = maxHP_;
        maxMP = maxMP_;
        SetHP(maxHP);
        SetHP(maxMP);
    }

    public void SetHP(int HP_) {
        HP = System.Math.Max(HP_, 0);
    }

    public void SetMP(int MP_) {
        MP = System.Math.Max(MP_, 0);
    }
}
