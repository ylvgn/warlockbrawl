using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestMovement : MonoBehaviour
{
    NavMeshAgent agent;
    [SerializeField]
    private float rotateSmoothTime = 1;
    public float rotateSpeedMovement = 0.1f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();    
    }

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
            {
                agent.SetDestination(hit.point);

                Quaternion rotation = Quaternion.LookRotation(hit.point - transform.position);
                float rotate_y = Mathf.SmoothDampAngle(transform.eulerAngles.y, rotation.eulerAngles.y, ref rotateSmoothTime, rotateSpeedMovement * Time.deltaTime * 5);
                transform.rotation = Quaternion.Euler(0, rotate_y, 0);
            }
        }
    }
}
