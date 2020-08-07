using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public Animator dialogueCanavsAnimator;
    private string executeAnimation = "OpenIt";
    private Queue<string> sentences;
    public GameObject heroImageGameObject;
    public GameObject otherImageGameObject;
    
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI otherNameText;
    public TextMeshProUGUI dialogueText;
    public GameObject dialogueCanvas;
    public GameObject storePanel;
    
    public enum WhoIsTalking {NPC, ENEMY, QuestGiver, QuestGiverDone, DoggoFriend,MERCHANT, NONE};
    private WhoIsTalking currentlyTalking;

    public GameObject continueButton;
    public GameObject runButton;
    public GameObject endButton;
    public GameObject acceptButton;
    public GameObject fightButton;
    public GameObject openStoreButton;
    public GameObject questCompleteButton;

    private Player player;

    void Start()
    {
        player = GameObject.FindObjectOfType<Player>();
        currentlyTalking = WhoIsTalking.NONE;
        dialogueCanvas.SetActive(false);
        sentences = new Queue<string>();
    }
    
    GameObject returnButon;
    public GameObject StartDialogue(string heroName, string otherName, Sprite heroImage, Sprite otherImage, Dialogue dialogue, WhoIsTalking whoIsTalking )
    {
        FindObjectOfType<Player>().UIQuestText.gameObject.SetActive(false);
        FindObjectOfType<Player>().SetOnScreenPlayerElements(false);
        player.currentMoveSpeed = 0;
        nameText.text = heroName;
        otherNameText.text = otherName;
        heroImageGameObject.GetComponent<Image>().sprite = heroImage;
        otherImageGameObject.GetComponent<Image>().sprite = otherImage;
        currentlyTalking = whoIsTalking;
        DialougeInteraction(dialogue);
        SetAllButtons(false);
        continueButton.SetActive(true);
        //To decide which button to return to the player to assign accept or fight methods to
        switch (currentlyTalking)
        {
            case WhoIsTalking.QuestGiver:
                returnButon = acceptButton;
                break;
            case WhoIsTalking.DoggoFriend:
                returnButon = acceptButton;
                break;
            case WhoIsTalking.QuestGiverDone:
                returnButon = questCompleteButton;
                break;
            case WhoIsTalking.ENEMY:
                returnButon = fightButton;
                break;
        }
        
        return returnButon;
    }

    private void DialougeInteraction(Dialogue dialogue)
    {
        dialogueCanvas.SetActive(true);
        dialogueCanavsAnimator.SetBool(executeAnimation, true);
        sentences.Clear();
        
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
        }
        else
        {
            string sentence = sentences.Dequeue();
            StopAllCoroutines();
            StartCoroutine(TypeSentence(sentence));
        }
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
    }

    private void EndDialogue()
    {
        continueButton.SetActive(false);
        switch (currentlyTalking)
        {
            case WhoIsTalking.NPC:
                Debug.Log("End of Conversation NPC");
                endButton.SetActive(true);
                break;
            case WhoIsTalking.QuestGiver:
                Debug.Log("End of Conversation QuestGiver");
                acceptButton.SetActive(true);
                break;
            case WhoIsTalking.QuestGiverDone:
                Debug.Log("End of Conversation QuestGiverDonw");
                questCompleteButton.SetActive(true);
                break;
            case WhoIsTalking.ENEMY:
                Debug.Log("End of Conversation Encounter");
                fightButton.SetActive(true);
                break;
            case WhoIsTalking.DoggoFriend:
                Debug.Log("End of Conversation DoggoFriend");
                acceptButton.SetActive(true);
                break;
            case WhoIsTalking.MERCHANT:
                Debug.Log("End of Conversation Merchant");
                openStoreButton.SetActive(true);
                break;
            case WhoIsTalking.NONE:
                Debug.Log("End of Conversation NONE");
                break;
        }
    }
    
    public void ExitDialogue()
    {
        FindObjectOfType<Player>().UIQuestText.gameObject.SetActive(true);
        FindObjectOfType<Player>().SetOnScreenPlayerElements(true);
        storePanel.SetActive(false);
        dialogueCanavsAnimator.SetBool(executeAnimation, false);
    }

    public void ClearVariables()
    {
        nameText.text = "";
        otherNameText.text = "";
        heroImageGameObject.GetComponent<Image>().sprite = null;
        otherImageGameObject.GetComponent<Image>().sprite = null;
        currentlyTalking = WhoIsTalking.NONE;
        player.currentMoveSpeed = player.startingMoveSpeed;
        dialogueCanvas.SetActive(false);
    }

    public void SetAllButtons(bool setTo)
    {
        continueButton.SetActive(setTo);
        endButton.SetActive(setTo);
        acceptButton.SetActive(setTo);
        fightButton.SetActive(setTo);
        openStoreButton.SetActive(setTo);
        questCompleteButton.SetActive(setTo);
    }
}
