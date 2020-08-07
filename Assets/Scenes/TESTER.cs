using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESTER : MonoBehaviour
{
    public GameObject test;
    // Start is called before the first frame update
    void Start()
    {
        GameObject newGo = test.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
        Debug.Log(newGo.name, newGo);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
