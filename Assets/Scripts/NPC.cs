using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

//using UnityEditor.Rendering;

public class NPC : MonoBehaviour
{
    public string NPCName;
    private Animator anim;

    public GameObject fImage;
    public GameObject nameText;
    
    [Header("Dialogue Components")]
    public Dialogue dialogue;
    
    private RectTransform fImageRect;
    private RectTransform nameTextRect;
    private float fImageScaleX;
    private float newImageScaleX;

    private void Start()
    {
        anim = this.gameObject.GetComponent<Animator>();
        nameText.GetComponent<Text>().text = string.Format("<color=#D2D2D2>" + NPCName + "</color>");
        fImage.SetActive(false);
        nameText.gameObject.SetActive(true);
        dialogue.name = NPCName;
        
        fImageRect = fImage.GetComponent<RectTransform>();
        nameTextRect = nameText.GetComponent<RectTransform>();

        fImageScaleX = fImageRect.localScale.x;
        newImageScaleX = nameTextRect.localScale.x;
    }

    private void Update()
    {
        if (Mathf.Sign(this.gameObject.transform.lossyScale.x) == -1f)
        {
            fImageRect.localScale = new Vector3(-fImageScaleX, fImageRect.localScale.y, fImageRect.localScale.z);
            nameTextRect.localScale = new Vector3(-newImageScaleX, nameTextRect.localScale.y, nameTextRect.localScale.z);
        }
        else
        {
            fImageRect.localScale = new Vector3(fImageScaleX, fImageRect.localScale.y, fImageRect.localScale.z);
            nameTextRect.localScale = new Vector3(newImageScaleX, nameTextRect.localScale.y, nameTextRect.localScale.z);
        }
    }

    IEnumerator SelectIdleAnim()
    {
        //yield return new WaitForSeconds(5);
        int num = Random.Range(1, 3);

        while (anim.GetInteger("idle") == num)
        {
            num = Random.Range(1, 3);
        }

        anim.SetInteger("idle", num);

        yield return null;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            fImage.SetActive(true);
            nameText.gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            nameText.gameObject.SetActive(true);
            fImage.SetActive(false);
        }
    }
}
