using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class Enemy
{
    public string enemyName;
    public int enemyLevel;
    public int maxHealth;
    public int currentHealth;
    public bool isDead;
    public GameObject enemyGameObject;
    public ItemScritpable debuff;
    public int turnsRemaining;

    [HideInInspector]
    public GameObject btnEnemyCharacterButton;
    [HideInInspector]
    public HealthBar enemyHealthBar;
    [HideInInspector]
    public GameObject currentBattlePos;
    [HideInInspector] public Text txtDebuffEnemy;

    public void OnDeathDestroy()
    {
        GameObject.Destroy(btnEnemyCharacterButton);
        currentBattlePos.SetActive(false);
        currentBattlePos = null;
    }
    
    public AttacksScriptable PickAttackByName(string attackName)
    {
        //Object[] data = AssetDatabase.LoadAllAssetsAtPath("Assets/Scripts/Attacks");
        List<AttacksScriptable> attacks = FindAssetsByType<AttacksScriptable>();
        AttacksScriptable useThisAttack = attacks.Find(x => x.attackName == attackName);
        return useThisAttack;
    }

    public ItemScritpable PickItemByName(string itemName)
    {
        //Object[] data = AssetDatabase.LoadAllAssetsAtPath("Assets/Scripts/Attacks");
        List<ItemScritpable> items = FindAssetsByType<ItemScritpable>();
        ItemScritpable useThisItem = items.Find(x => x.itemName == itemName);
        return useThisItem;
    }

    public AttacksScriptable PickAttack(AttacksScriptable.AttackType attackType)
    {
        //Object[] data = AssetDatabase.LoadAllAssetsAtPath("Assets/Scripts/Attacks");
        List<AttacksScriptable> attacks = FindAssetsByType<AttacksScriptable>();
        AttacksScriptable useThisAttack = attacks.Find(x => x.attackType == attackType);
        return useThisAttack;
    }

    public ItemScritpable PickItem(ItemScritpable.ItemType itemType)
    {
        //Object[] data = AssetDatabase.LoadAllAssetsAtPath("Assets/Scripts/Attacks");
        List<ItemScritpable> items = FindAssetsByType<ItemScritpable>();
        ItemScritpable useThisItem = items.Find(x => x.itemType == itemType);
        return useThisItem;
    }
    
    private static List<T> FindAssetsByType<T>() where T : UnityEngine.Object
    {
        List<T> assets = new List<T>();
        Object[] guids = Resources.LoadAll("");
        for( int i = 0; i < guids.Length; i++ )
        {
            for (int j = 0; j < guids.Length; j++)
            {
                if (guids[i].GetType() == typeof(T))
                {
                    assets.Add((T)guids[i]);
                }
            }
        }
        return assets;
    }
}
