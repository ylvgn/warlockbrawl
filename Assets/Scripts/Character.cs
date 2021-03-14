using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
    [Header("Datas")]
    [SerializeField] private CharacterData characterData = null;
    [SerializeField] private int curSelectedSkillId = -1;

    [Header("Components")]
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public Animator anim;
    [HideInInspector] public CharacterController characterController;

    public static event Action<CharacterData, int> OnSelectedSkill; // <CharacterData, skillId>
    public static event Action<CharacterData, int> OnFiredSkill;    // <CharacterData, skillId>
    public static event Action<CharacterData, int> OnCancledSkill;  // <CharacterData, skillId>
    public static event Action<CharacterData> OnGetHurt;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    public void SetData(CharacterData data)
    {
        this.characterData = data;
        this.curSelectedSkillId = -1;
    }

    public void Move(Vector3 destPos)
    {
        var data = this.characterData;
        this.agent.SetDestination(destPos);
        this.TowardDir(destPos - transform.position);
        StartCoroutine(MoveAnim());
    }

    IEnumerator MoveAnim()
    {
        var data = this.characterData;
        while (!agent.isStopped)
        {
            float speed = agent.velocity.magnitude / agent.speed;
            anim.SetFloat("Speed", speed, data.roleData.moveSpeedDampTime, Time.deltaTime);
            yield return new WaitForSeconds(0.02f);
        }
    }

    public void TowardDir(Vector3 dir)
    {
        var data = this.characterData;
        Quaternion rotation = Quaternion.LookRotation(dir);
        float rotation_y = Mathf.SmoothDampAngle(
            transform.eulerAngles.y,
            rotation.eulerAngles.y,
            ref data.roleData.rotateSmoothTime,
            data.roleData.rotateSpeedMovement * Time.deltaTime
        );
        transform.rotation = Quaternion.Euler(0, rotation_y, 0);
    }

    public bool canFireSkill()
    {
        return this.curSelectedSkillId != -1;
    }

    // 释放技能
    public void FireSkill(RaycastHit hit)
    {
        int skillId = this.curSelectedSkillId;
        if (skillId == -1) 
            return;
        var skillData = this.characterData.GetSkillData(skillId);
        if (skillData == null)
        {
            Debug.Log(string.Format("不存在该skillID={0}", skillId));
            return;
        }
        if (skillData.isCoolDowning)
        {
            Debug.Log(string.Format("当前skillID={0} 还在冷却中", skillId));
            return;
        }

        this.HandleSkillFire(hit);
        skillData.isCoolDowning = true;
        this.curSelectedSkillId = -1;
        if (OnFiredSkill != null)
            OnFiredSkill(this.characterData, skillId);
        Debug.Log("Fire Skill: " + skillId);
    }

    // 选择技能
    public void SelectSkill(int skillId)
    {
        if (this.curSelectedSkillId != -1)
        {
            Debug.Log(string.Format("已选择skillID{0}, 不可同时选择 skillDID{1}", curSelectedSkillId, skillId));
            return;
        }
        this.curSelectedSkillId = skillId;
        if (OnSelectedSkill != null)
            OnSelectedSkill(this.characterData, skillId);
        Debug.Log("SelectSkill: " + skillId);
    }

    // 技能冷却完毕
    public void CoolDownFinish(int skillId)
    {
        var skillData = this.characterData.GetSkillData(skillId);
        if (skillData == null)
        {
            Debug.Log(string.Format("skillId={0} 不存在", skillId));
            return;
        }
        skillData.isCoolDowning = false;
    }

    // 取消选择技能
    public void CancleSelectSkill()
    {
        int skillId = this.curSelectedSkillId;
        if (skillId == -1)
            return;

        curSelectedSkillId = -1;
        if (OnCancledSkill != null)
            OnCancledSkill(this.characterData, skillId);
        Debug.Log("Cancle Skill: " + skillId);
    }

    // tmp
    void HandleSkillFire(RaycastHit hit)
    {
        var skillData = this.characterData.GetSkillData(this.curSelectedSkillId);
        var playerPos = transform.position;
        var rangeType = skillData.rangeType;
        switch (rangeType)
        {
            case CharacterData.SkillData.RangeType.None:
                break;
            case CharacterData.SkillData.RangeType.Point:
                Vector3 pos = new Vector3(hit.point.x, 0, hit.point.z);
                Vector3 dir = (pos - playerPos).normalized;
                float offsetY = this.characterController.height;
                GameObject testPrefab = Resources.Load<GameObject>("Effect/sphere"); // temp
                var obj = GameObject.Instantiate<GameObject>(testPrefab, Vector3.zero, Quaternion.identity);
                var startPos = new Vector3(playerPos.x, playerPos.y + offsetY, playerPos.z) + dir;
                var endPos = new Vector3(pos.x, startPos.y, pos.z); // st 和 end 不用高度差
                obj.transform.position = startPos;
                obj.transform.LookAt(endPos);
                var ctrl = obj.AddComponent<AttackCollision>();

                Action update = () =>
                {
                    obj.transform.Translate(Vector3.forward * skillData.velocity * Time.deltaTime);
                    // dir 怎用只改变大小不改变方向。。下面写法的不行
                    // var newEndPos = endPos.normalized * data.maxAttackRadius;  // dir * float
                    // obj.transform.position = Vector3.Lerp(transform.position, newEndPos, skillData.velocity * Time.deltaTime);
                };

                ctrl.Init(update, (collision) =>
                {
                    Debug.Log(string.Format("test collosion --> {0} <color=#FFFF00>测试掉血</color>", collision.gameObject.name));
                    this.characterData.roleData.health -= 15;
                    if (OnGetHurt != null)
                        OnGetHurt(this.characterData);
                });
                break;
            case CharacterData.SkillData.RangeType.ArcShaped:
                break;
            case CharacterData.SkillData.RangeType.Circle:
                break;
            case CharacterData.SkillData.RangeType.DirectLine:
                break;
            default:
                break;
        }
    }

    // tmp 被击中的逻辑应该写在这里？
    private void OnCollisionEnter(Collision collision)
    {
        //// 测试掉血
        //var data = this.characterData;
        //data.roleData.health--;
        //if (OnGetHurt != null)
        //    OnGetHurt(data);
    }

    void OnDestroy()
    {
        OnSelectedSkill = null;
        OnFiredSkill = null;
        OnCancledSkill = null;
        OnGetHurt = null;
    }
}
