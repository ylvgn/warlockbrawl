using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillProject : MonoBehaviour, IDamage
{
    public SkillProjectData SkillProjectData { get; protected set; }
    protected Vector3 StartPos { get; set; }
    protected Character Owner { get; set; }

    public virtual void Init(SkillProjectData skillProjectData_)
    {
        SkillProjectData = skillProjectData_;
        Owner = GetOwner() as Character; // tmp
        StartPos = Owner.transform.position;
    }

    public SkillData getSkillData() {
        return SkillProjectData.skillData;
    }

    public Vector3 getDir() {
        return SkillProjectData.GetDir();
    }

    public Vector3 getEndPos()
    {
        return SkillProjectData.endPos;
    }

    public virtual void OnCollisionEnter(Collision collision) {
        IAttackable something = collision.gameObject.GetComponent<IAttackable>();
        if (something == GetOwner()) return;
        if (something != null && something.GetHP() > 0) {
            var damge = GetDamage(something);
            something.TakeDamage(damge.CalcDamage());
        }
        GameObject.Destroy(gameObject);
    }

    public IAttackable GetOwner()
    {
        return SkillProjectData.owner;
    }

    public abstract DamgeData GetDamage(IAttackable other);
}
