using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestAnim : MonoBehaviour
{
    Animator anim;
    NavMeshAgent agent;

    [SerializeField]
    private float dampTime = .1f;

    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        float speed = agent.velocity.magnitude / agent.speed;
        anim.SetFloat("Speed", speed, dampTime, Time.deltaTime);
    }
}
