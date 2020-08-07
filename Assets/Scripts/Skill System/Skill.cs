using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

//MARKER Skill Detail Information
[CreateAssetMenu(fileName = "new Skill", menuName = "Create Skill")]
public class Skill : ScriptableObject
{
    public enum SkillType{Health, Attack, Items};
    public SkillType skillType;
    [HideInInspector] public int healthUpgradeAmnt;
    [HideInInspector] public AttacksScriptable attack;
    [HideInInspector] public ItemScritpable item;
    
    public string skillName;
    public Sprite skillSprite;
    public int requiredLevel;
    public int requiredUpgradeAmnt;
    
    [TextArea(1, 3)] public string skillDes;
    public bool isUpgrade;

    public void ActivateSkill(RawImage upgradeImage)
    {
        Player player = FindObjectOfType<Player>();
        NotificationManager notificationManager = FindObjectOfType<NotificationManager>();
        if (CheckIfCanActivate(player))
        {
            switch (skillType)
            {
                case SkillType.Health:
                    player.maxHealth = healthUpgradeAmnt;
                    notificationManager.StartNotification("Health Now: " + healthUpgradeAmnt);
                    break;
                case SkillType.Attack:
                    player.PlayerInventoryAbilitiesCreate(attack);
                    notificationManager.StartNotification("New Attack Learned:" + attack.attackName);
                    break;
                case SkillType.Items:
                    player.PlayerInventoryItemsCreate(item);
                    notificationManager.StartNotification("New Item Obtained:" + item.itemName);
                    break;
            }
            Color color;
            color = Color.green;
            color.a = 1;
            upgradeImage.color = color;
            player.UseUpgradePoint(requiredUpgradeAmnt);
            this.isUpgrade = true;
        }
    }

    private bool CheckIfCanActivate(Player player)
    {
        NotificationManager notificationManager = FindObjectOfType<NotificationManager>();
        if (player.playerCurrentLevel >= this.requiredLevel)
        {
            if (isUpgrade == false)
            {
                if (player.playerCurrentUpgradePoints >= this.requiredUpgradeAmnt)
                {
                    return true;
                }
                else
                {
                    notificationManager.StartNotification("Not Enough Upgrade Points");
                    return false;
                }
            }
            else
            {
                notificationManager.StartNotification("Already been choosen");
                return false;
            }
        }
        else
        {
            notificationManager.StartNotification("Player Level too Low");
            return false;
        }
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(Skill))]
public class Skill_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        DrawDefaultInspector();

        Skill script = (Skill)target;

        switch (script.skillType)
        {
            case Skill.SkillType.Health:
                script.healthUpgradeAmnt = EditorGUILayout.IntField("Health Upgrade Amount", script.healthUpgradeAmnt);
                break;
            case Skill.SkillType.Attack:
                script.attack =
                    EditorGUILayout.ObjectField("Attack", script.attack, typeof(AttacksScriptable),
                        true) as AttacksScriptable;
                break;
            case Skill.SkillType.Items:
                script.item = 
                    EditorGUILayout.ObjectField("Item", script.item, typeof(ItemScritpable), 
                        true) as ItemScritpable;
                break;
            default:
                break;
        }
    }
}
#endif
