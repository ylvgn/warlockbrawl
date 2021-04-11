using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
[ExecuteInEditMode]
public class MyGizmosPoint : MonoBehaviour
{
    public bool AlwaysEnable = true;

    public bool EnableOnSelect = true;

    [Range(0, 1)]public float radius;

    public Color color = new Color(1, 1, 1, 1);

    void OnDrawGizmos()
    {
        var oldColor = Gizmos.color;
        if (!AlwaysEnable) return;
        Gizmos.color = color;
        Gizmos.DrawSphere(transform.position, radius);
        Gizmos.color = oldColor;
    }

    void OnDrawGizmosSelected()
    {
        var oldColor = Gizmos.color;
        if (!EnableOnSelect) return;
        Gizmos.color = color;
        Gizmos.DrawSphere(transform.position, radius);
        Gizmos.color = oldColor;
    }
}
