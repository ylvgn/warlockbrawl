using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
    [SerializeField]
    private SkillData[] skillList;
    private CharacterData characterData;

    private float rotateSmoothTime = 1f;
    private float rotateSpeedMovement = 0.1f;
    private float moveSpeedDampTime = 0.1f;

    private NavMeshAgent agent;
    private Animator anim;
    private int curSkillIndex = -1;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    public void SetData(CharacterData data)
    {
        characterData = data;
        agent.speed = data.movespeed;
    }

    public void Move(Vector3 dest)
    {
        agent.SetDestination(dest);
        Quaternion rotation = Quaternion.LookRotation(dest - transform.position);
        float rotate_y = Mathf.SmoothDampAngle(transform.eulerAngles.y, rotation.eulerAngles.y, ref rotateSmoothTime, rotateSpeedMovement * Time.deltaTime * 5);
        transform.rotation = Quaternion.Euler(0, rotate_y, 0);
        StartCoroutine(MoveAnim());
    }

    IEnumerator MoveAnim()
    {
        while (!agent.isStopped)
        {
            float speed = agent.velocity.magnitude / agent.speed;
            anim.SetFloat("Speed", speed, moveSpeedDampTime, Time.deltaTime);
            yield return new WaitForSeconds(0.02f);
        }
    }

    public void SelectSkill(int index)
    {
        if (curSkillIndex != -1) return;
        curSkillIndex = index;
    }

    public void CancleSelectSkill()
    {
        curSkillIndex = -1;
    }
}
