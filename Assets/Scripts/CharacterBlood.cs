using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterBlood : MonoBehaviour
{
    [SerializeField]
    private int healthMax;
    private int healthValue;

    private Image healthImg;

    void Start()
    {
        healthImg = transform.Find("bg/health").GetComponent<Image>();
    }
    public void Init(int healthMax)
    {
        this.healthMax = healthMax;
        this.healthValue = healthMax;
        this.SetData(healthMax);
    }

    void LateUpdate()
    {
        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180, 0);
    }

    public void SetData(int healthValue)
    {
        
    }
}
