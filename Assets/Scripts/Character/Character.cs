using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour, IAttackable
{
    public CharacterData CharacterData => characterData;
    [Header("Datas")]
    public bool isMoving = false;
    [SerializeField] private CharacterData characterData = null;
    private Dictionary<int, float> coolingDownSkillStartTimeDict;
    private float spellingstartTime;
    private SkillProjectData currentSpellingSkill;
    private List<BuffData> waitingAddBuffList;
    
    [Header("Components")]
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public Animator anim;
    [HideInInspector] public CharacterController characterController;

    [Header("Events")]
    Action onAttackEvent;

    void Awake()
    {
        coolingDownSkillStartTimeDict = new Dictionary<int, float>();
        waitingAddBuffList = new List<BuffData>();
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
        if (agent.destination == endPos && isMoving) return;

        agent.isStopped = false;
        agent.SetDestination(endPos);
        var dir = endPos - transform.position;
        if (dir == Vector3.zero) return;
        TowardDir(dir);
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
        if (IsSpelling()) StopSpelling();
        var skillData = skillBehaviourData.skillData;
        if (skillData == null)
        {
            Debug.Log($"[{characterData.name}]传入参数 skillData == null");
            return;
        }
        if (!CanIssueSkill()) {
            Debug.Log($"[{characterData.name}]当前状态{CharacterData.CharacterState}不能发动技能");
            return;
        }
        int skillId = skillData.id;
        if (characterData.GetSkillData(skillId) != skillData)
        {
            Debug.Log($"[{characterData.name}]找不到对应的skillId={skillId}的数据");
            return;
        }
        if (IsSkillCoolDowning(skillId))
        {
            Debug.Log($"[{characterData.name}]当前skillId={skillId}未冷却完成");
            return;
        }

        HandleSkillBehaviour(skillBehaviourData);
        skillData.isCoolDowning = true;
        coolingDownSkillStartTimeDict.Add(skillId, Time.realtimeSinceStartup);
    }

    void HandleSkillBehaviour(SkillBehaviourData skillBehaviourData)
    {
        var skillData = skillBehaviourData.skillData;
        GameObject prefab = skillData.resPrefab? skillData.resPrefab : Resources.Load<GameObject>(skillData.resPath);
        if (prefab == null)
        {
            Debug.LogError("找不到Resources资源 path= " + skillData.resPath);
            return;
        }
        if (skillData.RangeType == RangeType.None)
        {
            IssueMountBuffSkill(skillBehaviourData as SkillMountBuffData, prefab);
        } else
        {
            IssueProjectileSkill(skillBehaviourData as SkillProjectData, prefab);
        }
    }

    void IssueMountBuffSkill(SkillMountBuffData skillMountBuffData, GameObject prefab)
    {
        var mountBuff = GameObject.Instantiate<GameObject>(prefab, Vector3.zero, Quaternion.identity).GetComponent<SkillMountBuff>();
        mountBuff.Init(skillMountBuffData);
    }

    void IssueProjectileSkill(SkillProjectData skillProjectData, GameObject prefab)
    {
        var skillData = skillProjectData.skillData;
        currentSpellingSkill = skillProjectData;
        onAttackEvent = null;
        onAttackEvent += () =>
        {
            var skillProject = GameObject.Instantiate<GameObject>(prefab, Vector3.zero, Quaternion.identity).GetComponent<SkillProject>();
            skillProject.Init(skillProjectData);
        };
        TowardDir(skillProjectData.GetDir());
        if (skillData.IsBallistic()) {
            anim.SetTrigger("Attack");
        }
        else if (skillData.IsSpelling()) {
            spellingstartTime = Time.realtimeSinceStartup;
            characterData.CharacterState = CharacterState.Spelling;
            anim.SetBool("Spelling", true);
        }
    }

    void Update()
    {
        if (IsDead())
            return;

        #region move
        // 角色移动
        if (!agent.isStopped && CanMove()) {
            float sqrDistance = (agent.destination - transform.position).sqrMagnitude; // sqrMagnitude has better performance than Distance because or sqrt
            if (Mathf.Approximately(sqrDistance, 0f)) {
                StopMove();
            } else {
                float speed = agent.velocity.magnitude / agent.speed;
                anim.SetFloat("Speed", speed, characterData.moveSpeedDampTime, Time.deltaTime);
            }
        }
        #endregion

        #region skill
        // skill cool downing
        if (coolingDownSkillStartTimeDict.Count > 0)
        {
            List<int> removeList = null;
            foreach (var item in coolingDownSkillStartTimeDict)
            {
                var skillData = characterData.GetSkillData(item.Key);
                float endTime = item.Value + skillData.coolDownTime;
                if (Time.realtimeSinceStartup >= endTime && skillData.isCoolDowning)
                {
                    skillData.isCoolDowning = false;
                    if (removeList == null) removeList = new List<int>(16);
                    removeList.Add(item.Key);
                }
            }
            if (removeList != null)
            {
                for (int i = 0; i < removeList.Count; i++)
                    coolingDownSkillStartTimeDict.Remove(removeList[i]);
            }
        }

        // spelling skill
        if (IsSpelling())
        {
            if (onAttackEvent != null)
            {
                onAttackEvent();
                onAttackEvent = null;
            }
            var skillData = currentSpellingSkill.skillData;
            bool isEnd = Time.realtimeSinceStartup - spellingstartTime >= skillData.durationTime;
            if (isEnd)
            {
                StopSpelling();
            }
        }
        #endregion

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

    void OnDrawGizmos()
    {
        if (currentSpellingSkill == null) return;
        var skillData = currentSpellingSkill.skillData;
        if (IsSpelling())
        {
            var skillProjectData = currentSpellingSkill;
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(skillProjectData.endPos, skillData.maxRadius);
        }
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
            Debug.Log($"[{characterData.name}]找不到skillId={skillId}的startTimeStamp");
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
            currentSpellingSkill = null;
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
        return !IsDead() && characterData.canMove;
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
        if (IsDead() || characterData.CharacterState == CharacterState.OnAttack) return false;
        return true;
    }

    public int GetHP()
    {
        return characterData.health.HP.value;
    }

    public int GetMP()
    {
        return characterData.health.MP.value;
    }

    void StopSpelling(CharacterState state = CharacterState.Idle)
    {
        if (!IsSpelling()) return;
        anim.SetBool("Spelling", false);
        characterData.CharacterState = state;
        onAttackEvent = null;
        currentSpellingSkill = null;
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
        if (!IsDead())
        {
            characterData.canMove = true;
            characterData.CharacterState = CharacterState.Idle;
            onAttackEvent = null;
        }
    }
}
