using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Quest
{
    public bool isActive;
    public string title;
    public string description;
    public int rewardGoldAmount;
    public int rewardExperience;
    public QuestGoal questGoal;
    public Transform questTargetPos;
    public GameObject questButton;

    [HideInInspector] public bool isComplete;
    [HideInInspector] public bool hasAcceptedRewards;
    private bool hasGoneThrogh = false;
    public void Complete(List<Quest> quests, Text uiText, Player player)
    {
        if (questGoal.QuestGoalCheck(player))
        {
            if (hasGoneThrogh == false)
            {
                NotificationManager notificationManager = GameObject.FindObjectOfType<NotificationManager>();
                GameObject.FindObjectOfType<QuestWayPoint>().DeleteTargetPos();
                questButton.GetComponent<Button>().interactable = true;
                GameObject.FindObjectOfType<AudioManager>().Play("QuestComplete");
                title = string.Format("<color=green>{0}</color>", title);
                GameObject.FindObjectOfType<Player>().UIQuestText.gameObject.SetActive(true);
                UpdateUIText(quests, uiText);
                notificationManager.StartNotification("Quest Complete: " + title);
                hasGoneThrogh = true;
                isComplete = true;
            }
        }
    }

    public void UpdateUIText(List<Quest> quests, Text uiText)
    {
        uiText.text = "";
        foreach (Quest VARIABLE in quests)
        {
            if (VARIABLE.isActive)
            {
                Debug.Log(VARIABLE.title);
                uiText.text += VARIABLE.title + "\n";
            }
        }
    }

    public void onCHange(Quest quest, Toggle tgl, Text UIText)
    {
        quest.isActive = tgl.isOn;
        if (quest.isActive == true)
        {
            GameObject.FindObjectOfType<QuestWayPoint>().SetTargetPos(questTargetPos);
        }
        else
        {
            GameObject.FindObjectOfType<QuestWayPoint>().DeleteTargetPos();
        }
        
        UpdateUIText(GameObject.FindObjectOfType<Player>().quests, UIText);
    }
    
    public void AbandonQuest(List<Quest> quests, Text UIText)
    {
        GameObject.FindObjectOfType<QuestWayPoint>().DeleteTargetPos();
        quests.Remove(this);
        GameObject.Destroy(questButton);
        UpdateUIText(quests, UIText);
    }

    private bool hasRunthroughCompleteQuest = false;
    public void CompleteQuest(Quest newQuest, List<Quest> quests, Text UIText)
    {
        if (hasRunthroughCompleteQuest == false)
        {
            GameObject.FindObjectOfType<QuestWayPoint>().DeleteTargetPos();
            GameObject.FindObjectOfType<Player>().IncreaseXP(rewardExperience, rewardGoldAmount);
            //GameObject.FindObjectOfType<Player>().MoneyTextUpdate(rewardGoldAmount);
            UpdateUIText(GameObject.FindObjectOfType<Player>().quests, UIText);
            quests.Remove(newQuest);
            GameObject.Destroy(this.questButton);
            UpdateUIText(GameObject.FindObjectOfType<Player>().quests, UIText);
            if (this.questGoal.goalType == QuestGoal.GoalType.Gathering)
            {
                Player player = GameObject.FindObjectOfType<Player>();
                player.SpawnPickUpUiItem(player.playerPickUpItemses[player.playerPickUpItemses.FindIndex
                        (x => x.possiblePickUpItem == this.questGoal.itemToPickUp)].possiblePickUpItem,
                    player.playerPickUpItemses.FindIndex(x => x.possiblePickUpItem == this.questGoal.itemToPickUp),
                    -this.questGoal.requiredAmount );
            }
            GameObject.FindObjectOfType<NotificationManager>().StartNotification("Rewards Earned: " + rewardExperience + "XP " + rewardGoldAmount + "Gold");
            hasAcceptedRewards = true;
            hasRunthroughCompleteQuest = true;
        }
    }
}
