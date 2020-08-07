using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItemScript : MonoBehaviour
{
    public ItemScritpable pickUpItem;
    
    public GameObject fImage;

    private RectTransform fImageRect;
    private float fImageScaleX;

    private void Start()
    {
        fImage.SetActive(false);
        fImageRect = fImage.GetComponent<RectTransform>();
        fImageScaleX = fImageRect.localScale.x;
    }

    private void Update()
    {
        if (Mathf.Sign(this.gameObject.transform.lossyScale.x) == -1f)
        {
            fImageRect.localScale = new Vector3(-fImageScaleX, fImageRect.localScale.y, fImageRect.localScale.z);
        }
        else
        {
            fImageRect.localScale = new Vector3(fImageScaleX, fImageRect.localScale.y, fImageRect.localScale.z);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            fImage.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            fImage.SetActive(false);
        }
    }
}
