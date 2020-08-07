using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuestGiver : MonoBehaviour
{
    public Quest quest;
    public string questGiverName;
    private Player player;
    public GameObject fImage;
    public GameObject nameText;

    public Animator anim;

    [Header("Dialogue Components")]  
    public Dialogue dialogue;
    public Dialogue acceptedDialogue;
    public Dialogue questCompleteDialogue;
    public Dialogue questAfterCompleteDialogue;


    public bool hasAccepted;
    private QuestWayPoint questWayPoint;
    
    private RectTransform fImageRect;
    private RectTransform nameTextRect;
    private float fImageScaleX;
    private float newImageScaleX;

    public void Start()
    {
        dialogue.whoIsTalking = DialogueManager.WhoIsTalking.QuestGiver;
        acceptedDialogue.whoIsTalking = DialogueManager.WhoIsTalking.NPC;
        questCompleteDialogue.whoIsTalking = DialogueManager.WhoIsTalking.QuestGiverDone;
        questAfterCompleteDialogue.whoIsTalking = DialogueManager.WhoIsTalking.NPC;
        
        questWayPoint = FindObjectOfType<QuestWayPoint>();
        dialogue.name = questGiverName;
        acceptedDialogue.name = questGiverName;
        questCompleteDialogue.name = questGiverName;
        hasAccepted = false;
        nameText.GetComponent<Text>().text = string.Format("<color=#FFD400>" + questGiverName + "</color>");
        player = FindObjectOfType<Player>();
        
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

    public Dialogue DialogueSelection()
    {
        if (hasAccepted)
        {
            if (quest.hasAcceptedRewards)
            {
                return questAfterCompleteDialogue;
            }
            else if (quest.isComplete)
            {
                return questCompleteDialogue;
            }
            return acceptedDialogue;
        }
        else
        {
            return dialogue;
        }
    }

    public void AcceptQuest()
    {
        if (hasAccepted == false)
        {
            quest.isActive = true;
            questWayPoint.SetTargetPos(quest.questTargetPos);
            FindObjectOfType<AudioManager>().Play("QuestAccept");
            FindObjectOfType<Player>().UIQuestText.gameObject.SetActive(true);
            player.PlayerInventoryQuestsCreate(quest);
            hasAccepted = true;
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

// #if UNITY_EDITOR
// [CustomEditor(typeof(QuestGiver))]
// public class QuestGiver_Editor : Editor
// {
//     public UnityEngine.Object _CurrentEnemy;
//
//     public override void OnInspectorGUI()
//     {
//         DrawDefaultInspector();
//
//         QuestGiver script = (QuestGiver)target;
//
//         EditorGUILayout.EnumFlagsField(script.quest.questGoal.goalType);
//
//         switch (script.quest.questGoal.goalType)
//         {
//             case QuestGoal.GoalType.None:
//                 break;
//             case QuestGoal.GoalType.Kill:
//                 GUILayout.BeginHorizontal();
//                 _CurrentEnemy = EditorGUILayout.ObjectField("Current Enemy", _CurrentEnemy, typeof(Enemy), true);
//                 GUILayout.EndHorizontal();
//                 break;
//             case QuestGoal.GoalType.Gathering:
//                 script.quest.questGoal.currentAmount = EditorGUILayout.IntField("Current Amount",  script.quest.questGoal.currentAmount);
//                 script.quest.questGoal.requiredAmount = EditorGUILayout.IntField("Required Amount", script.quest.questGoal.requiredAmount);
//                 break;
//             default:
//                 break;
//         }
//     }
// }
// #endif
