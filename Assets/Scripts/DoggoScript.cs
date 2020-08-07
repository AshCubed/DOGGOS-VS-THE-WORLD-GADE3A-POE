using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DoggoScript : MonoBehaviour
{
    public string dogName;
    public GameObject currentDog;
    public Doggo doggo;
    private Player player;
    public Animator anim;
    public GameObject fImage;
    public GameObject nameText;

    [Header("Dialogue Components")]
    public Dialogue dialogue;

    private bool hasDogBeenAdded;
    
    private RectTransform fImageRect;
    private RectTransform nameTextRect;
    private float fImageScaleX;
    private float newImageScaleX;
    
    public void Start()
    {
        dialogue.name = dogName;
        hasDogBeenAdded = false;
        doggo.currentHealth = doggo.maxHealth;
        doggo.toolTipText = doggo.doggoName + "/n";
        for (int i = 0; i < doggo.doggoAttacks.Count; i++)
        {
            doggo.toolTipText += doggo.doggoAttacks[i].toolTipText + "/n";
        }
        
        
        dialogue.name = doggo.doggoName;

        player = FindObjectOfType<Player>();
        nameText.GetComponent<Text>().text = string.Format("<color=#558AC6>" + dogName + "</color>");
        
        fImage.SetActive(false);
        nameText.gameObject.SetActive(true);
        
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

    public void AddDoggo()
    {
        if (hasDogBeenAdded == false)
        {
            player.party.doggos.Add(doggo);
            player.PlayerInventoryDoggosCreate(doggo);
            currentDog.SetActive(false);
            hasDogBeenAdded = true;
        }

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
