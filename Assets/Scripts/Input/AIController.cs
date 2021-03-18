using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    private Character character;
    float speedDampTime;

    void Start()
    {
        character = MyUtility.GetComponent<Character>(transform);
    }

    public void Walk(Vector3 dir, float distance)
    {
        character.Move(dir * distance);
    }

}
