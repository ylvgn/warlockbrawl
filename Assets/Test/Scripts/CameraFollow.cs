using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    Vector3 offset;
    [SerializeField][Range(0.1f, 0.5f)]
    float smoothness = 0.5f;

    void Start()
    {
        offset = player.position - transform.position;
    }

    void Update()
    {
        var targetPos = player.position - offset;
        transform.position = Vector3.Slerp(transform.position, targetPos, smoothness);
    }
}
