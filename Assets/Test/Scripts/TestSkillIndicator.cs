using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestSkillIndicator : MonoBehaviour
{
    public float maxRange;
    public Transform player;
    public int selected = -1;

    Image skillImg;
    GameObject effectPrefab;

    float rotateSmoothTime = 0.1f;
    float rotateSpeedMovement = 0.1f;

    void Start()
    {
        maxRange = 100f;
        selected = -1;

        effectPrefab = Resources.Load<GameObject>("Effect/sphere");
        skillImg = transform.Find("indicator").GetComponent<Image>();
        skillImg.gameObject.SetActive(false);
    }

    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // 选择技能
        if (Input.GetKeyDown(KeyCode.Q))
        {            
            selected = 1;
            Debug.Log("选择技能");
        }

        // 取消选择
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (selected != -1)
            {
                selected = -1;
                skillImg.gameObject.SetActive(false);
                Debug.Log("Cancle!!");
            }
        }

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Vector3 pos = new Vector3(hit.point.x, 0, hit.point.z);
            Vector3 dir = (pos - player.transform.position).normalized;

            // 释放技能
            if (Input.GetMouseButton(0) && selected != -1)
            {
                selected = -1;
                skillImg.gameObject.SetActive(false);

                Quaternion rotation = Quaternion.LookRotation(hit.point - player.transform.position);
                float rotate_y = Mathf.SmoothDampAngle(player.transform.eulerAngles.y, rotation.eulerAngles.y, ref rotateSmoothTime, rotateSpeedMovement * Time.deltaTime * 5);
                player.transform.rotation = Quaternion.Euler(0, rotate_y, 0);
                Vector3 player_pos = player.transform.position;

                var obj = GameObject.Instantiate<GameObject>(effectPrefab, Vector3.zero, Quaternion.identity);
                var effectCtrl = obj.AddComponent<TestCollision>();
                TestCollision.EffectInfo info = new TestCollision.EffectInfo();
                info.skillData = new CharacterData.SkillData();
                info.skillData.maxAttackRadius = 300f;
                float height = player.GetComponent<CapsuleCollider>().height;
                info.start = new Vector3(player_pos.x, player_pos.y + height, player_pos.z) + dir;
                info.dir = new Vector3(pos.x, info.start.y, pos.z); // 无高度差
                info.speed = 1f;
                effectCtrl.Init(info, obj, () =>
                {
                    Debug.Log("call back!!");
                });
                Debug.Log("Fire !");
            }

            // 技能引导
            if (selected != -1)
            {
                if (!skillImg.IsActive())
                {
                    skillImg.gameObject.SetActive(true);
                    skillImg.transform.localScale = new Vector3(5, 5, 1);
                }
                skillImg.transform.position = hit.point;
                Debug.DrawLine(player.transform.position, hit.point, Color.red);
            }
        }
    }
}
