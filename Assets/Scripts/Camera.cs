using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public bool isThisProceduralMap;
    public Transform player;

    // Update is called once per frame
    void Update()
    {
        if (isThisProceduralMap)
        {
            transform.position = new Vector3(
                player.position.x,
                Mathf.Clamp(player.position.y, -3.025999f, -0.6240011f),
                transform.position.z);
        }
        else
        {
            transform.position = new Vector3(
                Mathf.Clamp(player.position.x, 0.951f, 14.37f),
                Mathf.Clamp(player.position.y, -3.025999f, -0.6240011f),
                transform.position.z);
        }
    }
}
