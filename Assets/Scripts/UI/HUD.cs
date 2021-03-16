﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private int healthMax;
    [SerializeField] private int healthValue;
    private bool isAnim;
    private float maxWidth;

    private Image healthImg;
    private RectTransform healthImgRect;
    public AnimationCurve animationCurve;
    private Character character;
    void Awake()
    {
        character = MyUtility.GetComponentInParent<Character>(transform);
        healthImgRect = MyUtility.GetComponent<RectTransform>(transform, "bg/health");
        healthImg = MyUtility.GetComponent<Image>(transform, "bg/health");
        maxWidth = healthImgRect.sizeDelta.x;
    }

    public void Init(int healthMax)
    {
        this.healthMax = healthMax;
        this.SetData(healthMax);
    }

    void Update()
    {
        if (character != null)
        {
            if (character.GetHP() != healthValue)
            {
                SetData(character.GetHP());
            }
        }
    }

    void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(Camera.main.transform.forward);
    }

    public void SetData(int healthValue_)
    {
        if (healthValue_ <= 0) {
            healthValue_ = 0;
        }

        if (isAnim)
        {
            StopAllCoroutines();
            isAnim = false;
        }

        isAnim = true;
        StartCoroutine(UITween(healthValue_));
        healthValue = healthValue_;
    }

    IEnumerator UITween(float endValue)
    {
        float imgWidth = maxWidth * endValue / this.healthMax;
        Vector2 toSizeDelta = new Vector2(imgWidth, healthImgRect.sizeDelta.y);
        float startTime = Time.realtimeSinceStartup;
        float endTime = Time.realtimeSinceStartup + 1;
        while (Time.realtimeSinceStartup <= endTime)
        {
            float passTime = Time.realtimeSinceStartup - startTime;
            healthImgRect.sizeDelta = Vector2.Lerp(healthImgRect.sizeDelta, toSizeDelta, animationCurve.Evaluate(passTime));
            yield return new WaitForSeconds(0.02f);
        }
        isAnim = false;
    }
}