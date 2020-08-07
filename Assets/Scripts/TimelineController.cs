using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineController : MonoBehaviour
{
    [Header("King Man Intro")]
    private string heroName;
    public string otherName;
    private Sprite heroImage;
    public Sprite otherImage;
    public Dialogue dialogue;
    
    private DialogueManager DM;

    
    #region IntroSequenceCode
    public void StartKingDialogue()
    {
        Player player = FindObjectOfType<Player>();
        heroName = player.heroName;
        heroImage = player.GetComponent<SpriteRenderer>().sprite;
        DM = FindObjectOfType<DialogueManager>();
        
        DM.StartDialogue(heroName, otherName, heroImage, otherImage, dialogue, DialogueManager.WhoIsTalking.NPC);
    }

    public void StartAmbientMusic()
    {
        FindObjectOfType<AudioManager>().Play("Medieval Ambiance");
    }
    #endregion

}
