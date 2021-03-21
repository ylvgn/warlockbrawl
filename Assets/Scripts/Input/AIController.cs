using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    private Character character;

    void Start()
    {
        character = MyUtility.GetComponent<Character>(transform);
    }

    public void Walk(Vector3 dir, float distance) {
        if (character.IsDead()) return;
        character.Move(dir * distance);
    }

}
