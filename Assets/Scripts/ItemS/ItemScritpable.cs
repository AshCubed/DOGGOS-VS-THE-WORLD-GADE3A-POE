using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "new Item", menuName = "Create Item")]
public class ItemScritpable : ScriptableObject
{
    public string itemName;

    public enum ItemType { HealingItem, BuffItem, DebuffItem, PickUpItem };
    public ItemType itemType;
    public GameObject itemSpriteAnimation;
    
    [HideInInspector] public int healAmount;
    [HideInInspector] public int buffAmount;
    [HideInInspector] public int debuffAmount;
    [HideInInspector] public int amntOfTurns;
    [HideInInspector] public Sprite pickUpItemSprite;
    
    [TextArea(3, 10)]
    public string toolTipText;

    public void ItemPlayerActivate(Player character)
    {
        Debug.Log("item bboepe");
        Debug.Log(this.itemName);
        Debug.Log(this.itemType);

        switch (itemType)
        {
            case ItemType.HealingItem:
                Debug.Log(CheckCanUseItem(character));
                if (CheckCanUseItem(character))
                {
                    if (character.currentHealth < character.maxHealth)
                    {
                        Instantiate(itemSpriteAnimation, character.transform.position, Quaternion.identity);
                        character.currentHealth += healAmount;
                        character.playerHealthBar.SetHealth(character.currentHealth);
                        DecrementItemAmount(character);
                        FindObjectOfType<AudioManager>().Play("Healing");
                        EndPartyTurn();
                    }
                }
                else
                {
                    Debug.Log("0 " + this.itemName + "s");
                }
                break;
            case ItemType.BuffItem:

                break;
            case ItemType.DebuffItem:
                Debug.Log(CheckCanUseItem(character));
                if (CheckCanUseItem(character))
                {
                    FindObjectOfType<BattleManagerV2>().itemScritpableInUse = this;
                    FindObjectOfType<BattleManagerV2>().SetEnemyCharacterButtonsInteractable(true);
                }
                break;
            case ItemType.PickUpItem:
                Player player = FindObjectOfType<Player>();
                player.SpawnPickUpUiItem(this,player.playerPickUpItemses.FindIndex(x => x.possiblePickUpItem = this), 1 );
                break;
            default:
                break;
        }
    }
    
    public void ItemEncounterUse(Enemy currentEnemy)
    {
        switch (itemType)
        {
            case ItemType.HealingItem:
                if (currentEnemy.currentHealth < currentEnemy.maxHealth)
                {
                    Instantiate(itemSpriteAnimation, currentEnemy.currentBattlePos.transform.position, Quaternion.identity);
                    currentEnemy.currentHealth += healAmount;
                    currentEnemy.enemyHealthBar.SetHealth(currentEnemy.currentHealth);
                    //DecrementItemAmount(currentEnemy);
                    FindObjectOfType<AudioManager>().Play("Healing");
                }
                break;
            case ItemType.BuffItem:
                break;
            case ItemType.DebuffItem:
                break;
            
            default:
                break;
        }
    }
    
    public void ItemDoggoUse(Doggo doggo, Player character)
    {
        switch (itemType)
        {
            case ItemType.HealingItem:
                Debug.Log(CheckCanUseItem(character));
                if (CheckCanUseItem(character))
                {
                    if (doggo.currentHealth < doggo.maxHealth)
                    {
                        Instantiate(itemSpriteAnimation, doggo.currentBattlePos.transform.position, Quaternion.identity);
                        doggo.currentHealth += healAmount;
                        doggo.doggoHealthBar.SetHealth(doggo.currentHealth);
                        DecrementItemAmount(character);
                        FindObjectOfType<AudioManager>().Play("Healing");
                        EndPartyTurn();
                    }
                }
                else
                {
                    Debug.Log("0 " + this.itemName + "s");
                }
                break;
            case ItemType.BuffItem:

                break;
            case ItemType.DebuffItem:
                Debug.Log(CheckCanUseItem(character));
                FindObjectOfType<BattleManagerV2>().itemScritpableInUse = this;
                FindObjectOfType<BattleManagerV2>().attacksScriptableInUse = null;
                FindObjectOfType<BattleManagerV2>().SetEnemyCharacterButtonsInteractable(true);
                break;
            default:
                break;
        }
    }

    public void ApplyDebuff(EncounterScript encounterScript, int enemy)
    {
        if (encounterScript.Enemies[enemy].debuff && encounterScript.Enemies[enemy].isDead == false)
        {
            if (encounterScript.Enemies[enemy].turnsRemaining > 0)
            {
                Instantiate(encounterScript.Enemies[enemy].debuff.itemSpriteAnimation,
                    encounterScript.Enemies[enemy].currentBattlePos.transform.position, Quaternion.identity);
                encounterScript.DealDamageToEnemy(enemy, debuffAmount);
                encounterScript.Enemies[enemy].turnsRemaining--;
                if (encounterScript.IsEncounterDead())
                {
                    BattleManagerV2 BV2 = FindObjectOfType<BattleManagerV2>();
                    BV2.currentState = BattleManagerV2.BattleState.Won;
                    BV2.BattleStateCheck();
                }
            }
            else
            {
                encounterScript.Enemies[enemy].enemyHealthBar.isPoisioned = false;
                encounterScript.Enemies[enemy].debuff = null;
                encounterScript.Enemies[enemy].turnsRemaining = 0;
                encounterScript.Enemies[enemy].enemyHealthBar.SetHealth(encounterScript.Enemies[enemy].currentHealth);
            }
        }
    }

    private void EndPartyTurn()
    {
        BattleManagerV2 bm = FindObjectOfType<BattleManagerV2>();
        bm.SetEnemyCharacterButtonsInteractable(false);
        bm.currentState = BattleManagerV2.BattleState.Encounterturn;
        bm.BattleStateCheck();
    }

    private void EndEnemyTurn()
    {
        BattleManagerV2 bm = FindObjectOfType<BattleManagerV2>();
        //bm.SetEnemyCharacterButtonsInteractable(false);
        bm.currentState = BattleManagerV2.BattleState.Partyturn;
        bm.BattleStateCheck();
    }

    

    public void DecrementItemAmount(Player character)
    {
        for (int i = 0; i < character.playerItems.Count; i++)
        {
            if (character.playerItems[i].item.itemName == this.itemName)
            {
                character.playerItems[i].itemAmount--;
                //Editing Player item btn to macth player amounts
                character.playerItems[i].playerItemBtn.GetComponentInChildren<Text>().text = character.playerItems[i].item.itemName + " " + 
                                                                                             character.playerItems[i].itemAmount;
            }
        }
        //Editing DoggoItem Buttons to mactch player ammounts
        //Editing DoggoItem Buttons to mactch player ammounts
        for (int i = 0; i < character.playerItems.Count; i++)
        {
            foreach (Doggo VARIABLE in character.party.doggos)
            {
                if (VARIABLE.doggoInUse)
                {
                    foreach (DoggoItems ITEM in VARIABLE.doggoItems)
                    {
                        if (character.playerItems[i].item.itemName == ITEM.doggoItemRef.itemName)
                        {
                            ITEM.doggoItemBtn.GetComponentInChildren<Text>().text = character.playerItems[i].item.itemName + " " + 
                                                                                    character.playerItems[i].itemAmount;
                        }
                    }
                }
                
            }
        }
    }

    private bool CheckCanUseItem(Player character)
    {
        for (int i = 0; i < character.playerItems.Count; i++)
        {
            if (character.playerItems[i].item.itemName == this.itemName)
            {
                if (character.playerItems[i].itemAmount <= 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        return false;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ItemScritpable))]
public class ItemScriptable_Editor : Editor
{
    public UnityEngine.Sprite _PickUpItemSprite;
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        DrawDefaultInspector();

        ItemScritpable script = (ItemScritpable)target;
        
        switch (script.itemType)
        {
            case ItemScritpable.ItemType.HealingItem:
                script.healAmount = EditorGUILayout.IntField("Heal Amount", script.healAmount);
                break;
            case ItemScritpable.ItemType.BuffItem:
                script.buffAmount = EditorGUILayout.IntField("Buff Amount", script.buffAmount);
                script.amntOfTurns = EditorGUILayout.IntField("Turns it lasts", script.amntOfTurns);
                break;
            case ItemScritpable.ItemType.DebuffItem:
                script.debuffAmount = EditorGUILayout.IntField("Debuff Amount", script.debuffAmount);
                script.amntOfTurns = EditorGUILayout.IntField("Turns it lasts", script.amntOfTurns);
                break;
            case ItemScritpable.ItemType.PickUpItem:
                GUILayout.BeginHorizontal();
                script.pickUpItemSprite = (Sprite) EditorGUILayout.ObjectField("Item Pick Up Sprite:", script.pickUpItemSprite,
                    typeof(Sprite), false);
                GUILayout.EndHorizontal();
                break;
            default:
                break;
        }
    }
}
#endif
