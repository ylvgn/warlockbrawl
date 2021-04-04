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
        var owner = getOwner();
        var skillData = getSkillData();
        Vector3 dir = getDir();
        float offsetY = owner.characterController.height;
        startPos = owner.transform.position + new Vector3(0, offsetY + 10, 0);
        transform.position = new Vector3(9999, 9999, 9999);
        GameObject.Destroy(gameObject, skillData.durationTime);
        isRoot = true;
        isEnable = true;
    }

    private SnowStorm CloneSelf(SkillProjectData data)
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
        res.isEnable = true;
        GameObject.Destroy(res.gameObject, startPos.y / skillData.flySpeed + 1.0f); // 1s延迟
        //MyUtility.MyDebug("src:{0} dest:{1} PosOffset:{2} Radius:{3}", getEndPos(), endPos, randomPosOffset, skillRadius);
        return res;
    }

    void Update()
    {
        if (!isEnable) return;
        var skillData = getSkillData();
        if (isRoot)
        {
            var owner = getOwner();
            if (!owner.IsSpelling())
            {
                GameObject.Destroy(gameObject);
                return;
            }

            // 每隔 spellingIntervalTime 秒后执行1次
            if (Time.realtimeSinceStartup - startTime >= skillData.spellingIntervalTime)
            {
                startTime = Time.realtimeSinceStartup;
                for (int i = 0; i < 5; i ++) // tmp
                    CloneSelf(SkillProjectData);
            }
        } else
        {
            transform.Translate(Vector3.forward * skillData.flySpeed * Time.deltaTime);
        }
    }

    public override void OnCollisionEnter(Collision collision)
    {
        if (!isEnable) return;
        var same = collision.gameObject.GetComponent<SnowStorm>();
        if (same) return;

        Character enemy = collision.gameObject.GetComponent<Character>();
        var owner = getOwner();
        if (enemy == owner) return;
        if (enemy && !enemy.IsDead())
        {
            var skillData = getSkillData();
            DamgeData damge = new DamgeData(owner.CharacterData, skillData, enemy.CharacterData);
            enemy.TakeDamege(damge.CalcDamage());
        }
        GameObject.Destroy(gameObject);
    }
}
