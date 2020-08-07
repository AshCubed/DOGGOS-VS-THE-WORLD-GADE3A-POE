using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EncounterScript : MonoBehaviour
{
    public string encounterName;
    public List<Enemy> Enemies;
    public Animator anim;
    public GameObject fImage;
    public GameObject nameText;
    private int currentEncounterInitiative;
    private Party currentParty;
    private BattleManagerV2 BV2;
    private DifficultyManager DM;
    private int multiplayerNumInfluence;
    private List<Nodes> savedNodes;
    
    public enum EnemyAIState{CheckingAI, Attack, Heal, AdvancedAiMove };
    [Header("AI")]
    public EnemyAIState AiState;


    [Header("Dialogue Components")]
    public Dialogue dialogue;
    
    private RectTransform fImageRect;
    private RectTransform nameTextRect;
    private float fImageScaleX;
    private float newImageScaleX;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < Enemies.Count; i++)
        {
            Enemies[i].currentHealth = Enemies[i].maxHealth;
        }
        
        for (int i = 0; i < Enemies.Count; i++)
        {
            Enemies[i].currentHealth = Enemies[i].maxHealth;
            Enemies[i].isDead = false;
        }

        BV2 = FindObjectOfType<BattleManagerV2>();
        DM = FindObjectOfType<DifficultyManager>();
        currentParty = null;
        dialogue.name = encounterName;
        nameText.GetComponent<Text>().text = string.Format("<color=#FF2D19>" + encounterName + "</color>");
        currentEncounterInitiative = -1;
        
        fImage.SetActive(false);
        nameText.gameObject.SetActive(true);
        
        fImageRect = fImage.GetComponent<RectTransform>();
        nameTextRect = nameText.GetComponent<RectTransform>();

        fImageScaleX = fImageRect.localScale.x;
        newImageScaleX = nameTextRect.localScale.x;
        
        savedNodes = new List<Nodes>();
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

    //Called by player and party members when they attack
    
    public void DealDamageToEnemy(int num, int damage)
    {
        Enemies[num].currentHealth -= damage;

        //Updates Health Bar
        Enemies[num].enemyHealthBar.SetHealth(Enemies[num].currentHealth);
        Debug.Log("Enemy " + num + " Health: " + Enemies[num].currentHealth);

        if (Enemies[num].currentHealth <= 0)
        {
            Enemies[num].isDead = true;
            Enemies[num].debuff = null;
            Enemies[num].OnDeathDestroy();
        }
    }

    //Enemy Turn AI Begins here
    public void CheckAIState(Party passThroughParty, int multiplayerNum = 0)
    {
        currentParty = passThroughParty;
        multiplayerNumInfluence = multiplayerNum;
        switch (AiState)
        {
            case EnemyAIState.CheckingAI:
                EncounterInitiativeCheck();
                break;
            case EnemyAIState.Attack:
                EncounterAttack();
                break;
            case EnemyAIState.Heal:
                EncounterHeal();
                break;
        }
    }
    
    private void EncounterInitiativeCheck()
    {
        currentEncounterInitiative++;

        if (currentEncounterInitiative > Enemies.Count - 1)
        {
            currentEncounterInitiative = 0;
        }
        Debug.Log("Current Enemy Initiative: " + currentEncounterInitiative + " " + Enemies[currentEncounterInitiative].enemyName);
        Debug.Log("Current Enemy Initiative: " + currentEncounterInitiative + " " + Enemies[currentEncounterInitiative].isDead);
        Debug.Log("Current Enemy Initiative: " + currentEncounterInitiative + " " + Enemies[currentEncounterInitiative].currentHealth);
        if (Enemies[currentEncounterInitiative].isDead == true ||
            Enemies[currentEncounterInitiative].currentHealth <= 0)
        {
            Enemy enemy = Enemies.Find(x => (x.isDead == false));
            Debug.Log("FOUND ENEMY: " + enemy.enemyName);
            for (int i = 0; i < Enemies.Count; i++)
            {
                if (Enemies[i] == enemy)
                {
                    currentEncounterInitiative = i;
                    break;
                }
            }
        }
        Debug.Log("Enemy Going: " + currentEncounterInitiative + " " + Enemies[currentEncounterInitiative].enemyName);
        Debug.Log("Enemy Going: " + currentEncounterInitiative + " " + Enemies[currentEncounterInitiative].isDead);
        Debug.Log("Enemy Going: " + currentEncounterInitiative + " " + Enemies[currentEncounterInitiative].currentHealth);
        AiThreatCheck(currentParty);
    }

    public int ReturnInitiative() //Used to find the name of the next enemy for battle Multiplayer ONLY
    {
        int tempNum = currentEncounterInitiative;
        tempNum++;

        if (tempNum > Enemies.Count - 1)
        {
            tempNum = 0;
        }
        if (Enemies[tempNum].isDead == true ||
            Enemies[tempNum].currentHealth <= 0)
        {
            Enemy enemy = Enemies.Find(x => (x.isDead == false));
            Debug.Log("FOUND ENEMY: " + enemy.enemyName);
            for (int i = 0; i < Enemies.Count; i++)
            {
                if (Enemies[i] == enemy)
                {
                    tempNum = i;
                    break;
                }
            }
        }

        return tempNum;
    }

    private void AiThreatCheck(Party passedParty)
    {
        currentParty = passedParty;

        if (DM.typeOfAi == DifficultyManager.TypeOfAi.NormalAi)
        {
            List<int> threatLevel = new List<int>();

            #region ThreatAddition
            for (int i = 0; i < Enemies.Count; i++)  //Thinking behind is that if their health is above half, they're not at threat of the party
            {
                if (Enemies[i].currentHealth >= Enemies[i].maxHealth/2)
                {
                    threatLevel.Add(0);
                }
                else
                {
                    threatLevel.Add(1);
                }
            }

            if (currentParty.player.currentHealth >= currentParty.player.maxHealth/2)
            {
                threatLevel.Add(1);
            }
            else
            {
                threatLevel.Add(0);
            }
        
            for (int i = 0; i < currentParty.doggos.Count; i++)
            {
                if (currentParty.doggos[i].currentHealth >= currentParty.doggos[i].maxHealth/2)
                {
                    threatLevel.Add(1);
                }
                else
                {
                    threatLevel.Add(0);
                }
            }
            #endregion
        
            threatLevel.Add(DM.DifficultyThreatCheck()); //Difficulty Check
            threatLevel.Add(multiplayerNumInfluence); //if multiplayer adding influence

            #region AiStateChange
            int threatLevelCount = 0;
            for (int i = 0; i < threatLevel.Count; i++)
            {
                threatLevelCount += threatLevel[i];
            }
            Debug.Log("Threat Level Count: " + threatLevelCount);

            if (threatLevelCount > 3)
            {
                AiState = EnemyAIState.Attack;
                CheckAIState(currentParty);
            }
            else
            {
                AiState = EnemyAIState.Heal;
                CheckAIState(currentParty);
            }
            #endregion
        }
        else
        {
            FileCreation fileCreation = new FileCreation();
            savedNodes = fileCreation.ReadFromSavedFile();
            Nodes useThisNodeForAction = new Nodes();
            if (savedNodes.Count != 0)
            {
                List<int> encounterSavedHealths = new List<int>();
                foreach (var variable in savedNodes)
                {
                    foreach (var variable2 in variable.currentEncounterHealth)
                    {
                        encounterSavedHealths.Add(variable2);
                    }
                }
                var closests = FindClosest(encounterSavedHealths, this.Enemies[currentEncounterInitiative].currentHealth);
                useThisNodeForAction = savedNodes.Find(x => x.currentEncounterHealth[currentEncounterInitiative] 
                                                            == (int) closests);
                Debug.Log("CURRENT NODE FOR ACTION: " + useThisNodeForAction);
                
                /*if (BV2.currentPartyInitiative == -1)
                {
                    List<int> playerSavedHealth = new List<int>();
                    foreach (var variable in savedNodes)
                    {
                        playerSavedHealth.Add(variable.currentPlayerHealth);  
                    }
                    var closest = FindClosest(playerSavedHealth, currentParty.player.currentHealth - 10);
                    useThisNodeForAction = savedNodes.Find(x => x.currentPlayerHealth == (int) closest);
                }
                else
                {
                    List<int> doggoSavedHealths = new List<int>();
                    foreach (var variable in savedNodes)
                    {
                        foreach (var doggosHealth  in variable.currentPartyHealth)
                        {
                            doggoSavedHealths.Add(doggosHealth);
                        }
                    }
                    var closest = FindClosest(doggoSavedHealths, currentParty.player.currentHealth);
                    foreach (var variable in savedNodes)
                    {
                        foreach (var doggosHealth  in variable.currentPartyHealth)
                        {
                            if (doggosHealth == closest)
                            {
                                useThisNodeForAction = variable;
                            }
                        }
                    }
                }*/
            }
            if (useThisNodeForAction != null)
            {
                AdvancedAiCommands(useThisNodeForAction);
            }
        }
    }

    private void AdvancedAiCommands(Nodes nodeToUse)
    {
        Debug.Log("AdvanceAI TERMS");
        Debug.Log(nodeToUse.counterAttack);
        Debug.Log(nodeToUse.counterItem);
        if (nodeToUse.counterAttack)
        {
            switch (nodeToUse.counterAttack.attackType)
            {
                /*case AttacksScriptable.AttackType.DirectAttack:
                    break;*/
                case AttacksScriptable.AttackType.FlyTo:
                    if (BV2.currentPartyInitiative == -1)
                    {
                        Debug.Log(string.Format("ENEMY ATTACK: " + "<color=green>{0}</color>", "Player"));

                        //direact attack player
                        Vector2 spawnPoint = new Vector2(Enemies[currentEncounterInitiative].currentBattlePos.transform.position.x - 0.3f, 
                            Enemies[currentEncounterInitiative].currentBattlePos.transform.position.y);
                        GameObject newGo = Instantiate(nodeToUse.counterAttack.attackSprite, spawnPoint, Quaternion.identity);
                        newGo.transform.localRotation = Quaternion.Euler(0, 0, 180);
                        newGo.GetComponent<AttackAnimationScript>().SetVariables(true,false, -1, 
                            nodeToUse.counterAttack.damageDealt, nodeToUse.counterAttack.attackHitSprite, null, currentParty);
                        newGo.GetComponent<AttackAnimationScript>().MoveTowardsTarget(currentParty.player.transform.position);
                    }
                    else
                    {
                        List<Doggo> customDoggoList = new List<Doggo>();
                        Doggo doggoToAttack = customDoggoList.Find(z => z.currentHealth == 
                                                                        (customDoggoList.Max(x => x.currentHealth)));
                        int actualListNum = currentParty.doggos.FindIndex(x => x == doggoToAttack);
                        if (currentParty.doggos.Count != 0 && customDoggoList != null && customDoggoList.Count != 0)
                        {
                            //direact attack doggo
                            Debug.Log(string.Format("ENEMY ATTACK: " + "<color=green>{0}</color>", currentParty.doggos[actualListNum].doggoName));
                
                            Vector2 spawnPoint = new Vector2(Enemies[currentEncounterInitiative].currentBattlePos.transform.position.x - 0.3f, 
                                Enemies[currentEncounterInitiative].currentBattlePos.transform.position.y);
                            GameObject newGo = Instantiate(nodeToUse.counterAttack.attackSprite, spawnPoint, Quaternion.identity);
                            newGo.transform.localRotation = Quaternion.Euler(0, 0, 180);
                            newGo.GetComponent<AttackAnimationScript>().SetVariables(true,false, actualListNum, 
                                nodeToUse.counterAttack.damageDealt, nodeToUse.counterAttack.attackHitSprite, null, currentParty);
                            newGo.GetComponent<AttackAnimationScript>()
                                .MoveTowardsTarget(currentParty.doggos[actualListNum].currentBattlePos.transform.position);
                        }
                    }
                    break;
                case AttacksScriptable.AttackType.AOE:
                    if (nodeToUse.counterAttack)
                    {
                        nodeToUse.counterAttack.AttackTarget(this, currentParty, false);
                    }
                    break;
                default:
                    break;
            }
        }
        else //if (nodeToUse.counterItem)
        {
            AiState = EnemyAIState.Heal;
            CheckAIState(currentParty);
        }
    }
    
    private int? FindClosest(IEnumerable<int> numbers, int x)
    {
        return
            (from number in numbers
                let difference = Math.Abs(number - x)
                orderby difference, Math.Abs(number), number descending
                select (int?) number)
            .FirstOrDefault();
    }

    
    private void EncounterAttack()
    {
        List<int> threatLevel = new List<int>();

        #region PartyMemberToAttackPicker
        if ((currentParty.player.currentHealth >= currentParty.player.maxHealth/2) && currentParty.player.isDead == false)
        {
            threatLevel.Add(1);
        }
        else
        {
            threatLevel.Add(0);
        }
        
        for (int i = 0; i < currentParty.doggos.Count; i++)
        {
            if ((currentParty.doggos[i].currentHealth >= currentParty.doggos[i].maxHealth/2) && currentParty.doggos[i].isDead == false)
            {
                threatLevel.Add(1);
            }
            else
            {
                threatLevel.Add(0);
            }
        }

        int high = 0;
        int low = 0;
        for (int i = 0; i < threatLevel.Count; i++)
        {
            if (threatLevel[i] == 0)
            {
                low++;
            }
            else
            {
                high++;
            }
        }
        #endregion

        #region AttackPartyTarget
        if (low > high && currentParty.doggos.Count != 0) //choose AOE attack
        {
            AttacksScriptable useAttack = null;

            useAttack = Enemies[currentEncounterInitiative].PickAttack(AttacksScriptable.AttackType.AOE);

            if (useAttack)
            {
                useAttack.AttackTarget(this, currentParty, false);
            }
        }
        else //Direct Attacks
        {
            AttacksScriptable useAttack = null;
            useAttack = Enemies[currentEncounterInitiative].PickAttack(AttacksScriptable.AttackType.FlyTo);
            List<Doggo> customDoggoList = new List<Doggo>();

            foreach (Doggo VARIABLE in currentParty.doggos)
            {
                if (VARIABLE.doggoInUse && VARIABLE.isDead == false)
                {
                    customDoggoList.Add(VARIABLE);
                }
            }

            Doggo doggoToAttack = customDoggoList.Find(z => z.currentHealth == 
                                                            (customDoggoList.Max(x => x.currentHealth)));
            int actualListNum = currentParty.doggos.FindIndex(x => x == doggoToAttack);
            
            if (currentParty.doggos.Count != 0 && customDoggoList != null && customDoggoList.Count != 0)
            {
                //direact attack doggo
                

                Debug.Log(string.Format("ENEMY ATTACK: " + "<color=green>{0}</color>", currentParty.doggos[actualListNum].doggoName));
                
                Vector2 spawnPoint = new Vector2(Enemies[currentEncounterInitiative].currentBattlePos.transform.position.x - 0.3f, 
                    Enemies[currentEncounterInitiative].currentBattlePos.transform.position.y);
                GameObject newGo = Instantiate(useAttack.attackSprite, spawnPoint, Quaternion.identity);
                newGo.transform.localRotation = Quaternion.Euler(0, 0, 180);
                newGo.GetComponent<AttackAnimationScript>().SetVariables(true,false, actualListNum, 
                    useAttack.damageDealt, useAttack.attackHitSprite, null, currentParty);
                newGo.GetComponent<AttackAnimationScript>()
                    .MoveTowardsTarget(currentParty.doggos[actualListNum].currentBattlePos.transform.position);
            }
            else
            {
                Debug.Log("ENEMY ATTACK: PLAYER");
                //direact attack player
                Vector2 spawnPoint = new Vector2(Enemies[currentEncounterInitiative].currentBattlePos.transform.position.x - 0.3f, 
                    Enemies[currentEncounterInitiative].currentBattlePos.transform.position.y);
                GameObject newGo = Instantiate(useAttack.attackSprite, spawnPoint, Quaternion.identity);
                newGo.transform.localRotation = Quaternion.Euler(0, 0, 180);
                newGo.GetComponent<AttackAnimationScript>().SetVariables(true,false, -1, 
                    useAttack.damageDealt, useAttack.attackHitSprite, null, currentParty);
                newGo.GetComponent<AttackAnimationScript>().MoveTowardsTarget(currentParty.player.transform.position);
            }
        }
        #endregion
    }

    private void EncounterHeal()
    {
        if (DM.typeOfPlay == DifficultyManager.TypeOfPlay.SinglePlayer && DM.typeOfAi == DifficultyManager.TypeOfAi.NormalAi)
        {
            if (Enemies[currentEncounterInitiative].currentHealth < (DM.DifficultyHealthNumCheck(Enemies[currentEncounterInitiative].maxHealth)))
            {
                //Heal
                Enemies[currentEncounterInitiative].PickItem(ItemScritpable.ItemType.HealingItem).
                    ItemEncounterUse(Enemies[currentEncounterInitiative]);
            }
            else
            {
                AiState = EnemyAIState.Attack;
                CheckAIState(currentParty);
            }
        }
        else
        {
                        
            Enemies[currentEncounterInitiative].PickItem(ItemScritpable.ItemType.HealingItem).
                ItemEncounterUse(Enemies[currentEncounterInitiative]);
        }
        EndTurn();
    }

    private void EndTurn()
    {
        BV2.currentState = BattleManagerV2.BattleState.Partyturn;
        BV2.BattleStateCheck();
    }

    public bool IsEncounterDead()
    {
        int dead = 0;

        foreach (Enemy item in Enemies)
        {
            if (item.isDead)
            {
                dead++;
            }
        }

        if (dead == Enemies.Count)
        {
            return true;
        }

        return false;
    }

    private static readonly int Idle = Animator.StringToHash("idle");
    IEnumerator SelectIdleAnim()
    {
        if (anim)
        {
            //yield return new WaitForSeconds(5);
            int num = Random.Range(1, 3);

            while (anim.GetInteger(Idle) == num)
            {
                num = Random.Range(1, 3);
            }

            anim.SetInteger(Idle, num);

            yield return null;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            fImage.SetActive(true);
            nameText.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            fImage.SetActive(false);
            nameText.SetActive(true);
        }
    }
}
