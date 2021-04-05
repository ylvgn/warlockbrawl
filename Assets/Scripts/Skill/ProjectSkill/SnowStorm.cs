using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowStorm : SkillProject
{
    private bool isEnable;
    private float startTime;
    private Vector3 startPos;
    private bool isRoot;

    public override void Init(SkillProjectData data)
    {
        base.Init(data);
        var skillData = getSkillData();
        float offsetY = Owner.characterController.height;
        transform.position = new Vector3(9999, 9999, 9999);
        startPos = Owner.transform.position + new Vector3(0, offsetY + 10, 0);
        isRoot = true;
        isEnable = true;
        GameObject.Destroy(gameObject, skillData.durationTime);
    }

    private SnowStorm CloneSelf()
    {
        var skillData = getSkillData();
        float skillRadius = skillData.maxRadius;
        float randomX = Random.Range(-skillRadius, skillRadius);
        float rangeZ = Mathf.Sqrt(skillRadius * skillRadius - randomX * randomX); // x² + y² = r²
        float randomZ = Random.Range(-rangeZ, rangeZ); ;
        Vector3 randomPosOffset = new Vector3(randomX, 0, randomZ);
        Vector3 endPos = getEndPos() + randomPosOffset;
        SnowStorm res = GameObject.Instantiate<GameObject>(gameObject, startPos + randomPosOffset, Quaternion.identity).GetComponent<SnowStorm>();
        Vector3 lookDir = endPos - res.transform.position;
        res.transform.rotation = Quaternion.LookRotation(lookDir);
        res.SkillProjectData = SkillProjectData;
        res.Owner = Owner;
        res.isEnable = true;
        GameObject.Destroy(res.gameObject, startPos.y / skillData.flySpeed + 1.0f); // 1s延迟
        //MyUtility.MyDebug("src:{0} dest:{1} PosOffset:{2} Radius:{3}", getEndPos(), endPos, randomPosOffset, skillRadius);
        return res;
    }

    void Update()
    {
        if (!isEnable) return;
        var skillData = getSkillData();
        if (isRoot) {
            if (!Owner.IsSpelling()) {
                GameObject.Destroy(gameObject);
                return;
            }

            // 每隔 spellingIntervalTime 秒执行1次
            if (Time.realtimeSinceStartup - startTime >= skillData.spellingIntervalTime) {
                startTime = Time.realtimeSinceStartup;
                for (int i = 0; i < 5; i ++) // tmp
                    CloneSelf();
            }
        } else {
            transform.Translate(Vector3.forward * skillData.flySpeed * Time.deltaTime);
        }
    }

    public override void OnCollisionEnter(Collision collision)
    {
        if (!isEnable) return;
        var same = collision.gameObject.GetComponent<SnowStorm>();
        if (same) return;

        Character enemy = collision.gameObject.GetComponent<Character>();
        if (enemy == Owner) return;
        if (enemy && !enemy.IsDead())
        {
            DamgeData damge = GetDamage(enemy);
            enemy.TakeDamage(damge.CalcDamage());
        }
        GameObject.Destroy(gameObject);
    }

    public override DamgeData GetDamage(IAttackable other)
    {
        var skillData = getSkillData();
        var enemy = other as Character;
        return new DamgeData(Owner.CharacterData, skillData, enemy.CharacterData);
    }
}
