using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonOnMouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject rawImage;

    void Start()
    {
        rawImage.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        rawImage.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rawImage.SetActive(false);
    }



    


}
