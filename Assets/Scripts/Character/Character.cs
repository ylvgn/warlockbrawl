using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour, IAttackable
{
    public CharacterData CharacterData => characterData;
    [Header("Datas")]
    [SerializeField] private CharacterData characterData = null;
    private Dictionary<int, float> coolingDownSkillStartTimeDict;
    private float spellingstartTime;
    private SkillBehaviourData currentSkill; // emmmmmm

#region buff
    private List<BuffData> waitingAddBuffList;
#endregion
    
    [Header("Components")]
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public Animator anim;
    [HideInInspector] public CharacterController characterController;

    [Header("Configs")]
    public Transform weaponIssuePoint; // tmp

    [Header("Events")]
    Action onAttackEvent;

    void Awake()
    {
        coolingDownSkillStartTimeDict = new Dictionary<int, float>();
        #region buff
        waitingAddBuffList = new List<BuffData>();
        #endregion
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    public void SetData(CharacterData data)
    {
        characterData = data;
        characterData.CharacterState = CharacterState.Idle;
    }

    public void Move(Vector3 endPos)
    {
        if (!CanMove()) return;
        if (IsSpelling()) StopSpelling();
        if (characterData.CharacterState == CharacterState.OnAttack) return; // tmp
        characterData.CharacterState = CharacterState.Moving;
        agent.isStopped = false;
        agent.SetDestination(endPos);
        TowardDir(endPos - transform.position);
    }

    public void StopMove(CharacterState state = CharacterState.Idle)
    {
        agent.isStopped = true;
        anim.SetFloat("Speed", 0);
        if (!IsDead())
            characterData.CharacterState = state;
    }

    public void TowardDir(Vector3 dir)
    {
        if (IsDead()) return;
        Quaternion rotation = Quaternion.LookRotation(dir);
        float rotationY = Mathf.SmoothDampAngle(
            transform.eulerAngles.y,
            rotation.eulerAngles.y,
            ref characterData.rotateSmoothTime,
            characterData.rotateSpeedMovement * Time.deltaTime
        );
        transform.rotation = Quaternion.Euler(0, rotationY, 0);
    }

    public void IssueSkill(int skillId)
    {
        var skillBehaviourData = SkillData.MakeBehaviourData(this, characterData.GetSkillData(skillId));
        IssueSkill(skillBehaviourData);
    }

    public void IssueSkill(SkillBehaviourData skillBehaviourData)
    {
        if (IsDead()) return;
        if (IsSpelling()) return;

        var skillData = skillBehaviourData.skillData;
        if (skillData == null)
        {
            Debug.Log("传入参数 skillData == null");
            return;
        }
        if (!CanIssueSkill()) {
            Debug.Log("当前状态不能发动技能");
            return;
        }
        int skillId = skillData.id;
        if (characterData.GetSkillData(skillId) != skillData)
        {
            Debug.Log(string.Format("找不到对应的skillId={0}的数据", skillId));
            return;
        }
        if (IsSkillCoolDowning(skillId))
        {
            Debug.Log(string.Format("当前skillId={0}未冷却完成", skillId));
            return;
        }

        IssueProjectSkill(skillBehaviourData as SkillProjectData); // tmp, SkillMountBuff未写完

        skillData.isCoolDowning = true;
        coolingDownSkillStartTimeDict.Add(skillId, Time.realtimeSinceStartup);
        return;
    }

    
    void IssueProjectSkill(SkillProjectData skillProjectData)
    {
        var skillData = skillProjectData.skillData;
        currentSkill = skillProjectData;
        GameObject prefab = Resources.Load<GameObject>(skillData.resPath);
        if (prefab == null)
        {
            Debug.LogError("找不到Resources资源 path= " + skillData.resPath);
            return;
        }
        StopMove();
        if (skillData.IsBallistic()) {
            characterData.CharacterState = CharacterState.OnAttack;
        } else if (skillData.IsSpelling()) {
            characterData.CharacterState = CharacterState.Spelling;
            spellingstartTime = Time.realtimeSinceStartup;
        }

        anim.SetTrigger("Attack");
        onAttackEvent += () =>
        {
            var skillProject = GameObject.Instantiate<GameObject>(prefab, Vector3.zero, Quaternion.identity).GetComponent<SkillProject>();
            skillProject.Init(skillProjectData as SkillProjectData);
        };
        TowardDir(skillProjectData.GetDir());
    }

    void Update()
    {
        if (IsDead())
            return;
        
        // 技能冷却中
        if (coolingDownSkillStartTimeDict.Count > 0)
        {
            List<int> removeList = new List<int>();
            foreach(var item in coolingDownSkillStartTimeDict) {
                var skillData = characterData.GetSkillData(item.Key);
                float endTime = item.Value + skillData.coolDownTime;
                if (Time.realtimeSinceStartup >=  endTime && skillData.isCoolDowning) {
                    skillData.isCoolDowning = false;
                    removeList.Add(item.Key);
                }
            }

            for (int i = 0; i < removeList.Count; i ++) {
                coolingDownSkillStartTimeDict.Remove(removeList[i]);
                //Debug.Log("技能冷却完毕skillId:" + removeList[i]);
            }
        }

        // 角色移动
        if(!agent.isStopped) {
            float distance = Vector3.Distance(agent.destination, transform.position);
            if (Mathf.Approximately(distance, 0f)) {
                StopMove();
            } else {
                float speed = agent.velocity.magnitude / agent.speed;
                anim.SetFloat("Speed", speed, characterData.moveSpeedDampTime, Time.deltaTime);
            }
        }

        // Spelling
        if (IsSpelling())
        {
            var skillData = currentSkill.skillData;
            bool isEnd = Time.realtimeSinceStartup - spellingstartTime >= skillData.durationTime;
            if (isEnd)
                StopSpelling();
        }

#region buff
        // add buff
        if (waitingAddBuffList.Count > 0)
        {
            characterData.AddBuffs(waitingAddBuffList);
            waitingAddBuffList.Clear();
        }

        // handle buff
        characterData.HandleAllBuffs();
#endregion
    }

    private void OnDrawGizmos()
    {
        var oldColor = Gizmos.color;
        if (currentSkill == null) return;
        var skillData = currentSkill.skillData;
        if (IsSpelling())
        {
            var skillProjectData = currentSkill as SkillProjectData; // tmppppp
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(skillProjectData.endPos, skillData.maxRadius);
        }
        Gizmos.color = oldColor;
    }

    public void JoyStick(Vector3 dir)
    {   
        if (!CanMove()) return;
        float speed = agent.speed;
        TowardDir(dir);
        transform.position += dir * speed * Time.deltaTime * characterData.moveSpeedDampTime;
        anim.SetFloat("Speed", speed, characterData.moveSpeedDampTime, Time.deltaTime);
    }

    public float GetCoolDownStartTimeStamp(int skillId)
    {
        if (IsSkillCoolDowning(skillId)) {
            float res;
            if (coolingDownSkillStartTimeDict.TryGetValue(skillId, out res)){
                return res;
            }
            Debug.Log(string.Format("找不到skillId={0}的startTimeStamp", skillId));
        }
        return Time.realtimeSinceStartup;
    }

    public bool IsSkillCoolDowning(int skillId)
    {
        var skillData = characterData.GetSkillData(skillId);
        if (skillData.id == 0) {
            Debug.Log(string.Format("找不到skillId={0}的数据", skillId));
            return false;
        }
        return skillData.isCoolDowning;
    }

    public void TakeDamage(int damge)
    {
        if (IsDead()) return;
        characterData.health.SetHP(GetHP() - damge);
        if (GetHP() == 0)
        {
            characterData.CharacterState = CharacterState.Dead;
            StopMove();
            characterData.TakeOffAllBuffs();
            MyUtility.MyDebug("<color=#00FF00> === [{0}] is dead === </color>", characterData.name);
        }
    }

    public void PutOnBuff(BuffData buffData)
    {
        if (buffData == null || IsDead()) return;
        waitingAddBuffList.Add(buffData);
        MyUtility.MyDebug("PutOnBuff[{0}]:{1}", characterData.name, buffData.name);
    }

    public bool CanMove()
    {
        if (IsDead()) return false;
        return true;
    }

    public bool IsDead()
    {
        return characterData.CharacterState == CharacterState.Dead;
    }

    public bool IsSpelling()
    {
        return characterData.CharacterState == CharacterState.Spelling;
    }

    public bool CanIssueSkill()
    {
        if (IsDead()) return false;
        return true;
    }

    public int GetHP()
    {
        return characterData.health.HP;
    }

    public int GetMP()
    {
        return characterData.health.MP;
    }

    void StopSpelling(CharacterState state = CharacterState.Idle)
    {
        if (!IsSpelling()) return;
        characterData.CharacterState = state;
        onAttackEvent = null;
        currentSkill = null;
        MyUtility.MyDebug("({0})StopSpelling ===", characterData.name);
    }

    // 在animator内调用
    void OnAttack()
    {
        if (onAttackEvent != null)
            onAttackEvent();
    }

    // 在animator内调用
    void EndAttack()
    {
        if (IsSpelling()) return; // tmp
        if (!IsDead())
        {
            characterData.CharacterState = CharacterState.Idle;
            onAttackEvent = null;
        }
    }

    // temp
    public void LearnSkill(SkillData skillData)
    {
        characterData.LearnSkill(skillData);
    }
}
