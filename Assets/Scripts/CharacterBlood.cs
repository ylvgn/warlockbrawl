using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterBlood : MonoBehaviour
{
    [SerializeField]
    private int healthMax;
    private int healthValue = 0;
    [SerializeField]
    private float animSpeed = 0.5f;
    private bool isAnim;
    private float maxWidth;

    private Image healthImg;
    private RectTransform healthImgRect;

    void Awake()
    {
        healthImgRect = transform.Find("bg/health").GetComponent<RectTransform>();
        healthImg = transform.Find("bg/health").GetComponent<Image>();
        maxWidth = healthImgRect.sizeDelta.x;
    }

    public void Init(int healthMax)
    {
        this.healthMax = healthMax;
        this.SetData(healthMax);
    }
    void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }

    public void SetData(int healthValue)
    {
        if (this.isAnim)
        {
            StopAllCoroutines();
            this.isAnim = false;
        }

        this.isAnim = true;
        StartCoroutine(UITween(this.healthValue, Mathf.Max(0, healthValue)));
        this.healthValue = healthValue;
    }

    IEnumerator UITween(float from, float to)
    {
        float imgWidth = maxWidth * to / this.healthMax;
        Vector2 toSizeDelta = new Vector2(imgWidth, healthImgRect.sizeDelta.y);
        while (!Mathf.Approximately(from, to))
        {
            from = Mathf.Lerp(from, to, animSpeed);
            healthImgRect.sizeDelta = Vector2.Lerp(healthImgRect.sizeDelta, toSizeDelta, animSpeed);
            yield return new WaitForSeconds(0.02f);
        }
        isAnim = false;
    }
}
