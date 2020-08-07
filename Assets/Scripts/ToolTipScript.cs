using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ToolTipScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject rawImage;
    //[HideInInspector]
    //public string tooltipText;
    public TextMeshProUGUI txt;
    public AttacksScriptable As;
    public ItemScritpable Is;
    public Quest quest;
    public Doggo doggo;

    [HideInInspector] public bool useExtraDscrp;
    [HideInInspector] public string extraDescription;

    void Start()
    {
        //rawImage.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (As)
        {
            txt.text = As.toolTipText;
        }
        else if (Is)
        {
            txt.text = Is.toolTipText;
        }
        else if (quest.description != "" && quest.description != null)
        {
            txt.text = quest.description;
        }
        else if (doggo.toolTipText != "" || doggo.toolTipText != null)
        {
            txt.text = doggo.CreateDoggoToolTip();
        }

        if (useExtraDscrp)
        {
            txt.text += extraDescription;
        }
        
        rawImage.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        txt.text = "";
        rawImage.SetActive(false);
    }
}
