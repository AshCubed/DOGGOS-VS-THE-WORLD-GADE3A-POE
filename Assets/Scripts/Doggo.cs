using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Doggo
{
    [Header("Doggo Info")]
    public string doggoName;
    public List<AttacksScriptable> doggoAttacks;
    public bool doggoInUse;
    public GameObject thisDoggoGameObject;
    public bool isDead;

    [Header("Health Stats")]
    public int maxHealth;
    public int currentHealth;

    [Header("BattleManager Spawned Stored Items")]
    public HealthBar doggoHealthBar;
    public GameObject btnDoggoCharacterButton;
    public GameObject pnlDoggoAbilitiesItems;
    
    [HideInInspector] public List<DoggoItems> doggoItems;

    //[Header("ToolTip")]
    //[TextArea(3, 10)]
    [HideInInspector]
    public string toolTipText;
    [HideInInspector]
    public GameObject currentBattlePos;

    public string CreateDoggoToolTip()
    {
        string text = doggoName + "\n";
        for (int i = 0; i < doggoAttacks.Count; i++)
        {
            text += doggoAttacks[i].attackName + "\n" + doggoAttacks[i].damageDealt;
        }
        return text;
    }
    
}

[System.Serializable]
public class DoggoItems
{
    public ItemScritpable doggoItemRef;
    public Button doggoItemBtn;
}


