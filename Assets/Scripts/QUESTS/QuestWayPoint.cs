using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestWayPoint : MonoBehaviour
{
    public GameObject player;
    public RawImage img;
    public Text meter;
    public Vector3 offset;
    private Transform target;

    private void Update()
    {
        if (target)
        {
            float minX = img.GetPixelAdjustedRect().width / 2;
            float maxX = Screen.width - minX;

            float minY = img.GetPixelAdjustedRect().height / 2;
            float maxY = Screen.height - minX;

            Vector2 pos = UnityEngine.Camera.main.WorldToScreenPoint(target.position + offset);

            if (Vector2.Dot((target.position - transform.position), transform.forward) < 0)
            {
                if (pos.x < Screen.width / 2)
                {
                    pos.x = maxX;
                }
                else
                {
                    pos.x = minX;
                }
            }

            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);

            img.transform.position = pos;
            meter.text = (((int)Vector2.Distance(target.position, player.transform.position)).ToString() + "m");
        }
        else
        {
            img.gameObject.SetActive(false);
        }
    }

    public void SetTargetPos(Transform pos)
    {
        img.gameObject.SetActive(true);
        target = pos;
    }

    public void DeleteTargetPos()
    {
        target = null;
    }

    public void SetWayPointView(bool x)
    {
        img.gameObject.SetActive(x);
    }
}
