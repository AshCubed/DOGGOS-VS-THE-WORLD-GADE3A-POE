using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattleManagerTraining : MonoBehaviour
{
    [Header("Party Section")] public TrainingParty trainingParty;
    [Header("Encounter Section")] public TrainingEncounter trainingEncounter;
    
    public enum TrainingBattleState {Start, Partyturn, Encounterturn, Won, Lost };
    [Header("Current Game State")]
    public TrainingBattleState currentState;

    [Header("Text Assest")] private static TextAsset _textAsset;

    [Header("AI Training")] 
    public List<Nodes> trainingNodes;
    public List<Nodes> savedNodes;
    public int wins;
    public int loses;

    [HideInInspector] public Nodes currentTurnNode;
    //public List<Nodes> testReadFromSave;

    
    /*public enum EnemyAiState{CheckingAi, Attack, Heal };
    [Header("AI State")]
    public EnemyAiState aiState;*/
    
    // Start is called before the first frame update
    void Start()
    {
        wins = 0;
        loses = 0;
        /*currentState = TrainingBattleState.Start;
        StartCoroutine(BattleStateCheckStart());*/
        StartAgain();
        /*FileCreation fileCreation = new FileCreation();
        testReadFromSave = fileCreation.ReadFromSavedFile();*/
    }

    private void StartAgain()
    {
        trainingParty.hasAttacked = false;
        trainingEncounter.hasAttacked = true;
        
        trainingParty.currentTrainingPartyInitiative = -1;
        trainingEncounter.currentEncounterInitiative = 0;
        trainingParty.currentTrainingPlayerHealth = trainingParty.maxTrainingPlayerHealth;
        for (int i = 0; i < trainingParty.trainingDoggos.Count; i++)
        {
            trainingParty.trainingDoggos[i].currentHealth = trainingParty.trainingDoggos[i].maxHealth;
            trainingParty.trainingDoggos[i].isDead = false;
        }

        trainingParty.posionAmnt = 3;
        trainingParty.healAmnt = 3;
        
        for (int i = 0; i < trainingEncounter.enemies.Count; i++)
        {
            trainingEncounter.enemies[i].currentHealth = trainingEncounter.enemies[i].maxHealth;
            trainingEncounter.enemies[i].isDead = false;
        }
        
        currentState = TrainingBattleState.Partyturn;
        StartCoroutine(BattleStateCheckStart());
    }

    /*private void RunTheGame()
    {
        switch (currentState)
        {
            case TrainingBattleState.Start:
                Debug.Log("START");
                StartAgain();
                break;
            case TrainingBattleState.Partyturn:
                Debug.Log(trainingParty.currentTrainingPartyInitiative);
                trainingParty.TrainingPartyInitiative();
                trainingParty.DealDamageToOponent(trainingParty.currentTrainingPartyInitiative, trainingEncounter);
                if (trainingEncounter.IsTrainingEncounterDead())
                {
                    currentState = TrainingBattleState.Lost;
                    RunTheGame();
                }
                else
                {
                    currentState = TrainingBattleState.Encounterturn;
                    RunTheGame();
                }
                break;
            case TrainingBattleState.Encounterturn:
                //Apply Debuff Poision
                for (int i = 0; i < trainingEncounter.enemies.Count; i++)
                {
                    if (trainingEncounter.enemies[i].debuff)
                    {
                        if (trainingEncounter.enemies[i].turnsRemaining <= 0)
                        {
                            trainingEncounter.enemies[i].debuff = null;
                            //Debug.Log($"Poison finished for {trainingEncounter.enemies[i]}");
                        }
                        else
                        {
                            trainingEncounter.enemies[i].currentHealth -=
                                trainingEncounter.enemies[i].debuff.debuffAmount;
                            trainingEncounter.enemies[i].turnsRemaining--;
                            //Debug.Log($"Poison applied to {trainingEncounter.enemies[i]} & " +
                            //          $"Turns Remaining {trainingEncounter.enemies[i].turnsRemaining}");
                        }
                    }
                }
                //Encounter Turn
                trainingEncounter.TrainingEncounterInitiativeCheck();
                Debug.Log(trainingEncounter.currentEncounterInitiative);
                //trainingEncounter.DealDamageToOponent(trainingEncounter.currentEncounterInitiative, trainingParty);
                if (trainingEncounter.IsTrainingEncounterDead())
                {
                    currentState = TrainingBattleState.Won;
                    RunTheGame();
                }
                else
                {
                    CheckNodeAtEndOfTurn(trainingNodes[trainingNodes.Count - 1]);
                    currentState = TrainingBattleState.Partyturn;
                    RunTheGame();
                }
                break;
            case TrainingBattleState.Won:
                EvaluateTrainingNode();
                
                wins++;
                //Debug.Log($"Wins: {wins}");
                //Debug.Log($"Loses: {loses}");
                if (wins != 10)
                {
                    currentState = TrainingBattleState.Start;
                    RunTheGame();
                }
                
                
                break;
            case TrainingBattleState.Lost:
                EvaluateTrainingNode();
                loses++;
                //Debug.Log($"Wins: {wins}");
                //Debug.Log($"Loses: {loses}");
                
                if (wins != 10)
                {
                    currentState = TrainingBattleState.Start;
                    RunTheGame();
                }
                break;
            default:
                break;
        }
    }*/

    public void StartBattler()
    {
        StartCoroutine(BattleStateCheckStart());
    }

    IEnumerator BattleStateCheckStart()
    {
        switch (currentState)
        {
            case TrainingBattleState.Start:
                //Debug.Log("START");
                StartAgain();
                yield return new WaitForSeconds(1f);
                currentState = TrainingBattleState.Partyturn;
                StartCoroutine(BattleStateCheckStart());
                break;
            case TrainingBattleState.Partyturn:
                //Debug.Log("Party Turn");
                trainingParty.TrainingPartyInitiative();
                yield return new WaitForSeconds(1f);
                if (trainingParty.hasAttacked == false)
                {
                    trainingParty.DealDamageToOponent(trainingParty.currentTrainingPartyInitiative, trainingEncounter);
                    trainingParty.hasAttacked = true;
                    trainingEncounter.hasAttacked = false;
                }
                yield return new WaitForSeconds(1f);
                break;
            case TrainingBattleState.Encounterturn:
                //Debug.Log("Encounter Turn");
                //Apply Debuff Poision
                for (int i = 0; i < trainingEncounter.enemies.Count; i++)
                {
                    if (trainingEncounter.enemies[i].debuff)
                    {
                        if (trainingEncounter.enemies[i].turnsRemaining <= 0)
                        {
                            trainingEncounter.enemies[i].debuff = null;
                            //Debug.Log($"Poison finished for {trainingEncounter.enemies[i]}");
                        }
                        else
                        {
                            trainingEncounter.enemies[i].currentHealth -=
                                trainingEncounter.enemies[i].debuff.debuffAmount;
                            trainingEncounter.enemies[i].turnsRemaining--;
                            //Debug.Log($"Poison applied to {trainingEncounter.enemies[i]} & " +
                            //          $"Turns Remaining {trainingEncounter.enemies[i].turnsRemaining}");
                        }
                    }
                }

                //Encounter Turn
                trainingEncounter.TrainingEncounterInitiativeCheck();
                yield return new WaitForSeconds(1f);
                //Debug.Log(trainingEncounter.currentEncounterInitiative);
                if (trainingEncounter.hasAttacked == false)
                {
                    trainingEncounter.DealDamageToOponent(trainingEncounter.currentEncounterInitiative, trainingParty);
                    trainingEncounter.hasAttacked = true;
                    trainingParty.hasAttacked = false;
                }
                yield return new WaitForSeconds(0.5f);
                break;
            case TrainingBattleState.Won:
                EvaluateTrainingNode();

                wins++;
                //Debug.Log($"Wins: {wins}");
                //Debug.Log($"Loses: {loses}");
                if (wins <= 100)
                {
                    yield return new WaitForSeconds(0.5f);
                    currentState = TrainingBattleState.Start;
                    StartCoroutine(BattleStateCheckStart());
                }
                break;
            case TrainingBattleState.Lost:
                EvaluateTrainingNode();

                loses++;
                //Debug.Log($"Wins: {wins}");
                //Debug.Log($"Loses: {loses}");

                if (loses <= 100)
                {
                    yield return new WaitForSeconds(0.5f);
                    currentState = TrainingBattleState.Start;
                    StartCoroutine(BattleStateCheckStart());
                }

                break;
            default:
                break;
        }
        yield break;
    }


    public void WeightNodeAtEndOfTurn(Nodes node)
    {
        trainingNodes.Add(node);
        currentTurnNode = null;
        List<int> threatLevel = new List<int>();

        #region ThreatAddition
        for (int i = 0; i < node.currentEncounterHealth.Count; i++) 
        {
            if (node.currentEncounterHealth[i] >= trainingEncounter.enemies[i].maxHealth/2)
            {
                threatLevel.Add(0);
            }
            else
            {
                threatLevel.Add(1);
            }
        }
        
        //Check if they healed when attacked and health is below a certain amount
        if (node.playerUsedAttack)
        {
            int check = 0;
            for (int i = 0; i < node.currentEncounterHealth.Count; i++)
            {
                if (node.currentEncounterHealth[i] <= trainingEncounter.enemies[i].maxHealth / 2)
                {
                    check++;
                }
            }
            if (node.counterItem && node.counterItem.itemType == ItemScritpable.ItemType.HealingItem && 
                check == node.currentEncounterHealth.Count)
            {
                threatLevel.Add(1);
            }
        }

        //Check if they healed when posined
        if (node.playerUsedItem && node.playerUsedItem.itemType == ItemScritpable.ItemType.DebuffItem)
        {
            if (node.counterItem && node.counterItem.itemType == ItemScritpable.ItemType.HealingItem)
            {
                threatLevel.Add(1);
            }
        }

        if (node.currentPlayerHealth <= trainingParty.maxTrainingPlayerHealth/2)
        {
            threatLevel.Add(0);
        }
        else
        {
            threatLevel.Add(1);
        }
        
        for (int i = 0; i < node.currentPartyHealth.Count; i++)
        {
            if (node.currentPartyHealth[i] <= trainingParty.trainingDoggos[i].maxHealth/2)
            {
                threatLevel.Add(0);
            }
            else
            {
                threatLevel.Add(1);
            }
        }
        #endregion
        
        /*threatLevel.Add(DM.DifficultyThreatCheck()); //Difficulty Check
        threatLevel.Add(multiplayerNumInfluence); //if multiplayer adding influence*/

        #region WeightInput
        int threatLevelCount = 0;
        for (int i = 0; i < threatLevel.Count; i++)
        {
            threatLevelCount += threatLevel[i];
        }
        //Debug.Log("Threat Level Count: " + threatLevelCount);
        

        trainingNodes[trainingNodes.Count - 1].weight = threatLevelCount;
        #endregion

        //save good training nodes to saved nodes
        //check if trianing node that wants to be saved already exists in saved nodes
    }

    public void EvaluateTrainingNode()
    {
        foreach (var node in trainingNodes)
        {
            if (node.weight >= 3)
            {
                List<Nodes> tempSavedNodes = savedNodes;
                List<Nodes> toCheck = new List<Nodes>();

                /*Nodes tempWeightCheck = new Nodes();
                tempWeightCheck = node;*/

                int tempWeightCheck = node.weight;
                
                if (node.counterAttack != null)
                {
                    toCheck = tempSavedNodes.FindAll
                        (x => (x.counterAttack = node.counterAttack) && (x.playerUsedAttack == node.playerUsedAttack));
                }
                else if (node.counterItem != null)
                {
                    toCheck = tempSavedNodes.FindAll
                        (x => (x.counterItem = node.counterItem) && (x.playerUsedItem == node.playerUsedItem));
                }
                
                if (toCheck.Count != 0)
                {
                    foreach (var variable in toCheck)
                    {
                        if (variable.weight > tempWeightCheck)
                        {
                            tempWeightCheck = variable.weight;
                        }
                    }

                    node.weight = tempWeightCheck;

                    foreach (var var in toCheck)
                    {
                        tempSavedNodes.Remove(var);
                    }
                    
                    tempSavedNodes.Add(node);
                    savedNodes = tempSavedNodes;
                }
                else
                {
                    tempSavedNodes.Add(node);
                    savedNodes = tempSavedNodes;
                }
            }
        }
        FileCreation fileCreation = new FileCreation(trainingNodes, savedNodes);
        fileCreation.WriteToTrainingFile();
        fileCreation.WriteToSavedFile();
        trainingNodes = new List<Nodes>();
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

}

public class FileCreation
{
    public FileCreation()
    {
        
    }
    
    public FileCreation(List<Nodes> trainingNodes, List<Nodes> savedNodes)
    {
        this.trainingNodes = trainingNodes;
        this.savedNodes = savedNodes;
    }

    private List<Nodes> trainingNodes;
    private List<Nodes> savedNodes;

    private string trainingNodesPath = Application.dataPath + "/Resources/trainingNodes.txt";
    private string savedNodesPath = Application.dataPath + "/Resources/savedNodes.txt";

    public void WriteToTrainingFile()
    {
        //trainingNodesWrite
        if (!File.Exists(trainingNodesPath))
        {
            File.WriteAllText(trainingNodesPath, ListOfNodesToString(trainingNodes));
        }
        else
        {
            File.AppendAllText(trainingNodesPath, ListOfNodesToString(trainingNodes));
        }
    }
    
    public void WriteToSavedFile()
    {
        //trainingNodesWrite
        if (!File.Exists(savedNodesPath))
        {
            File.WriteAllText(savedNodesPath, ListOfNodesToString(savedNodes));
        }
        else
        {
            File.WriteAllText(savedNodesPath, ListOfNodesToString(savedNodes));
        }
    }

    public List<Nodes> ReadFromSavedFile()
    {
        if (File.Exists(savedNodesPath))
        {
             string[] stringToConvert = File.ReadAllLines(savedNodesPath);
             List<Nodes> savedTrainingNodes = new List<Nodes>();
             for (int i = 0; i < stringToConvert.Length - 1; i++)
             {
                 if (stringToConvert[i].Substring(0,1) == "-")
                 {
                     if (stringToConvert[i + 1] != "")
                     {
                         Nodes node = new Nodes();
                         Enemy enemy = new Enemy();
                         int playerAction = i + 1;
                         int whoAttacked = i + 2;
                         int counterAction = i + 3;
                         int player1 = i + 4;
                         int player2 = i + 5;
                         int enemy1 = i + 6;
                         int enemy2 = i + 7;
                         int weight = i + 8;

                         if (stringToConvert[playerAction].Substring(0, 2) == "PA")
                         {
                             string playerAttackString = stringToConvert[playerAction].Substring(3);
                             AttacksScriptable pA = enemy.PickAttackByName(playerAttackString);
                             node.playerUsedAttack = pA;
                         }
                         else
                         {
                             string playerItemString = stringToConvert[playerAction].Substring(3);
                             ItemScritpable pI = enemy.PickItemByName(playerItemString);
                             node.playerUsedItem = pI;
                         }

                         if (stringToConvert[counterAction].Substring(0, 2) == "CA")
                         {
                             string counterAttackString = stringToConvert[counterAction].Substring(3);
                             AttacksScriptable cA = enemy.PickAttackByName(counterAttackString);
                             node.counterAttack = cA;
                         }
                         else
                         {
                             string counterItemStirng = stringToConvert[counterAction].Substring(3);
                             ItemScritpable cI = enemy.PickItemByName(counterItemStirng);
                             node.counterItem = cI;
                         }
                         

                         node.whoAttacked = int.Parse(stringToConvert[whoAttacked]);
                     
                         
                         //Find attack based on name and add to node

                         node.currentPlayerHealth = int.Parse(stringToConvert[player1]);
                         node.currentPartyHealth.Add(int.Parse(stringToConvert[player2]));
                         node.currentEncounterHealth.Add(int.Parse(stringToConvert[enemy1]));
                         node.currentEncounterHealth.Add(int.Parse(stringToConvert[enemy2]));
                         node.weight = int.Parse(stringToConvert[weight]);
                     
                         savedTrainingNodes.Add(node);
                     }
                 }
             }
             return savedTrainingNodes;
        }
        else
        {
            //Debug.Log("Saved Training TXT file not found");
            return null;
        }
    }
    
    private string ListOfNodesToString(List<Nodes> nodeses)
    {
        string beeep = "------" + "\n";
        if (nodeses.Count > 0)
        {
            for (int i = 0; i < nodeses.Count; i++)
            {
                if (nodeses[i].playerUsedAttack)
                {
                    beeep += "PA " + nodeses[i].playerUsedAttack.attackName + "\n";
                }
                else//(nodeses[i].playerUsedItem)
                {
                    beeep += "PI " + nodeses[i].playerUsedItem.itemName + "\n";
                }
                beeep += nodeses[i].whoAttacked.ToString() + "\n";
                if (nodeses[i].counterAttack)
                {
                    beeep += "CA " + nodeses[i].counterAttack.attackName + "\n";
                }
                else //(nodeses[i].counterItem)
                {
                    beeep += "CI " +nodeses[i].counterItem.itemName + "\n";
                }
                beeep += nodeses[i].currentPlayerHealth.ToString() + "\n";
                foreach (var variable in nodeses[i].currentPartyHealth)
                {
                    beeep += variable.ToString() + "\n";
                }
                foreach (var variable in nodeses[i].currentEncounterHealth)
                {
                    beeep += variable.ToString() + "\n";
                }

                beeep += nodeses[i].weight + "\n";
                beeep += "------" + "\n";
            }
        }
        beeep += "\n";
        return beeep;
    }
    
}

[System.Serializable]
public class Nodes
{
    public Nodes()
    {
    }

    public AttacksScriptable playerUsedAttack;
    public ItemScritpable playerUsedItem;
    public int whoAttacked; //0 is player, 1 is a doggo
    public AttacksScriptable counterAttack;
    public ItemScritpable counterItem;
    [HideInInspector] public int currentPlayerHealth;
    [HideInInspector] public List<int> currentPartyHealth = new List<int>();
    [HideInInspector] public List<int> currentEncounterHealth = new List<int>();
    public int weight;
}


[System.Serializable]
public class TrainingEncounter
{
    public string encounterName;
    public List<Enemy> enemies;
    public int currentEncounterInitiative;
    public BattleManagerTraining battleManagerTraining;
    public bool hasAttacked;

    [Header("Attacks")] 
    public AttacksScriptable flyTo;
    public AttacksScriptable aoe;
    public ItemScritpable heal;

    private int attackNum;

    public void DealDamageToTrainingEnemy(int num, int damage)
    {
        //Debug.Log("Deal damage to enemy " +num);
        
        enemies[num].currentHealth -= damage;
        
        //Debug.Log($"Ennemy {enemies[num].enemyName} current health {enemies[num].currentHealth}");
        
        /*if (enemies[num].currentHealth <= 0)
        {
            enemies[num].isDead = true;
            enemies[num].debuff = null;
        }*/
        foreach (var variable in enemies)
        {
            if (variable.currentHealth <= 0)
            {
                variable.isDead = true;
                variable.debuff = null;
            }
        }
    }

    public void DealDamageToOponent(int initiative, TrainingParty trainingParty)
    {
        if (!hasAttacked)
        {
            Nodes node = battleManagerTraining.currentTurnNode;
            /*Nodes useThisNodeForAction = new Nodes();
            if (battleManagerTraining.savedNodes.Count != 0)
            {
                if (node.whoAttacked == 0)
                {
                    List<int> playerSavedHealth = new List<int>();
                    foreach (var variable in battleManagerTraining.savedNodes)
                    {
                        playerSavedHealth.Add(variable.currentPlayerHealth);  
                    }
                    var closest = FindClosest(playerSavedHealth, trainingParty.currentTrainingPlayerHealth);
                    useThisNodeForAction = battleManagerTraining.savedNodes.Find(x => x.currentPlayerHealth == (int) closest);
                }
                else
                {
                    List<int> doggoSavedHealths = new List<int>();
                    foreach (var variable in battleManagerTraining.savedNodes)
                    {
                        foreach (var doggosHealth  in variable.currentPartyHealth)
                        {
                            doggoSavedHealths.Add(doggosHealth);
                        }
                    }
                    var closest = FindClosest(doggoSavedHealths, trainingParty.currentTrainingPlayerHealth);
                    foreach (var variable in battleManagerTraining.savedNodes)
                    {
                        foreach (var doggosHealth  in variable.currentPartyHealth)
                        {
                            if (doggosHealth == closest)
                            {
                                useThisNodeForAction = variable;
                            }
                        }
                    }
                }
            }*/
            
            int numofEnemie = Random.Range(-1, trainingParty.trainingDoggos.Count - 1);
            if (numofEnemie != -1 && trainingParty.isTrainingPlayerDead == true)
            {
                for (int i = 0; i < trainingParty.trainingDoggos.Count; i++)
                {
                    if (trainingParty.trainingDoggos[i].isDead == false)
                    {
                        numofEnemie = i;
                    }
                }
            }



            /*AttacksScriptable useThisAttack = null;
            ItemScritpable useThisItem = null;
            if (useThisNodeForAction != null)
            {
                if (trainingParty.attackUsedThisTurn != null)
                {
                    useThisAttack = useThisNodeForAction.counterAttack;
                    Debug.Log(useThisAttack);
                }
                else
                {
                    useThisItem = useThisNodeForAction.counterItem;
                    Debug.Log(useThisItem);
                }
            }

            if (useThisAttack != null)
            {
                battleManagerTraining.currentTurnNode.counterAttack = useThisAttack;
                if (useThisAttack.attackType == AttacksScriptable.AttackType.AOE)
                {
                    trainingParty.DealDamageToParty(-1, useThisAttack.damageDealt);
                    foreach (var variable in trainingParty.trainingDoggos)
                    {
                        variable.currentHealth -= aoe.damageDealt;
                    }
                }
                else
                {
                    trainingParty.DealDamageToParty(numofEnemie, useThisAttack.damageDealt);
                }

                node.counterAttack = useThisAttack;
            }
            else if (useThisItem != null)
            {
                switch (useThisItem.itemType)
                {
                    case ItemScritpable.ItemType.HealingItem:
                        battleManagerTraining.currentTurnNode.counterItem = useThisItem;
                        HealEncounterMember(initiative, useThisItem.healAmount);
                        node.counterItem = useThisItem;
                        break;
                }
            }
            else*/

            int rnd = Random.Range(0, 3);
            while (attackNum == rnd)
            {
                rnd = Random.Range(0, 3);
            }
            attackNum = rnd;

            //Debug.Log("Enemy wants to do " + rnd + " initiative " + initiative);
            switch (rnd)
            {
                case 0:
                    node.counterAttack = flyTo;
                    node.counterItem = null;
                    trainingParty.DealDamageToParty(numofEnemie, flyTo.damageDealt);
                    break;
                case 1:
                    node.counterAttack = aoe;
                    node.counterItem = null;
                    trainingParty.DealDamageToParty(-1, aoe.damageDealt);
                    foreach (var variable in trainingParty.trainingDoggos)
                    {
                        variable.currentHealth -= aoe.damageDealt;
                    }
                    break;
                case 2:
                    node.counterItem = heal;
                    node.counterAttack = null;
                    HealEncounterMember(initiative, heal.healAmount);
                    break;
            }
            
            node.currentPlayerHealth = trainingParty.currentTrainingPlayerHealth;
            foreach (var variable in trainingParty.trainingDoggos)
            {
                node.currentPartyHealth.Add(variable.currentHealth);
            }

            battleManagerTraining.currentTurnNode = node;
            
            if (trainingParty.IsPartyDead())
            {
                //yield return new WaitForSeconds(0.5f);
                battleManagerTraining.currentState = BattleManagerTraining.TrainingBattleState.Won;
                battleManagerTraining.StartBattler();
            }
            else
            {
                //yield return new WaitForSeconds(0.5f);
                battleManagerTraining.WeightNodeAtEndOfTurn(battleManagerTraining.currentTurnNode);
                battleManagerTraining.currentState = BattleManagerTraining.TrainingBattleState.Partyturn;
                battleManagerTraining.StartBattler();
            }
        }
    }
    
    public void HealEncounterMember(int i, int healtAmnt)
    {
        if (enemies[i].currentHealth < 100)
        {
            enemies[i].currentHealth += healtAmnt;
            //Debug.Log($"Enemy {enemies[i].enemyName} Healed for {healAmnt} ");
        }
    }
    
    public bool IsTrainingEncounterDead()
    {
        int dead = 0;

        foreach (Enemy item in enemies)
        {
            if (item.isDead)
            {
                dead++;
            }
        }

        if (dead == enemies.Count)
        {
            return true;
        }

        return false;
    }

    public void TrainingEncounterInitiativeCheck()
    {
        currentEncounterInitiative++;

        if (currentEncounterInitiative > enemies.Count - 1)
        {
            currentEncounterInitiative = 0;
        }
        if (enemies[currentEncounterInitiative].isDead == true ||
            enemies[currentEncounterInitiative].currentHealth <= 0)
        {
            Enemy enemy = enemies.Find(x => (x.isDead == false));
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i] == enemy)
                {
                    currentEncounterInitiative = i;
                    break;
                }
            }
        }
        //Debug.Log(currentEncounterInitiative);
        //return currentEncounterInitiative;
    }
}


[System.Serializable]
public class TrainingParty
{
    public string heroName;
    public bool isTrainingPlayerDead;
    public int maxTrainingPlayerHealth;
    public int currentTrainingPlayerHealth;
    public BattleManagerTraining battleManagerTraining;
    
    public List<Doggo> trainingDoggos;
    public int trainPartyMax = 2;
    public int currentTrainingPartyInitiative;

    [Header("Attacks")] 
    public AttacksScriptable direct;
    public AttacksScriptable flyTo;
    public AttacksScriptable aoe;
    public ItemScritpable poision;
    public int posionAmnt;
    public ItemScritpable heal;
    public int healAmnt;
    
    [HideInInspector] public AttacksScriptable attackUsedThisTurn;
    [HideInInspector] public ItemScritpable itemUsedThisTurn;
    public bool hasAttacked;

    private int attackNum;
    
    public bool IsPartyDead()
    {
        int dead = 0;
        int doggosInUse = 0;
        if (isTrainingPlayerDead)
        {
            dead++;
        }
        foreach (Doggo item in trainingDoggos)
        {
            if (item.doggoInUse)
            {
                doggosInUse++;
                if (item.isDead)
                {
                    dead++;
                }
            }
        }
        //Debug.Log("Party Members Dead:" + dead);
        if (dead >= doggosInUse + 1)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public void DealDamageToParty(int num, int damageToTake)
    {
       // Debug.Log("Deal damage to party " + num);
        //Check if i is player
        if (num == -1)
        {
            currentTrainingPlayerHealth -= damageToTake;
            
            //Debug.Log("Training Player current Health: " + currentTrainingPlayerHealth);
            if (currentTrainingPlayerHealth <= 0)
            {
                isTrainingPlayerDead = true;
            }
        }
        else
        {
            //TakeDamage
            trainingDoggos[num].currentHealth -= damageToTake;
            if (trainingDoggos[num].currentHealth <= 0)
            {
                trainingDoggos[num].isDead = true;
            }
            //Debug.Log("Training Doggo " + num + " Health: " + trainingDoggos[num].currentHealth);
        }
    }
    
    public void DealDamageToOponent(int intiative, TrainingEncounter trainingEncounter)
    {
        if (!hasAttacked)
        {
            attackUsedThisTurn = null;
            itemUsedThisTurn = null;
            int numofEnemie = Random.Range(0, trainingEncounter.enemies.Count - 1);

            if (trainingEncounter.enemies[numofEnemie].isDead == true)
            {
                for (int i = 0; i < trainingEncounter.enemies.Count; i++)
                {
                    if (trainingEncounter.enemies[i].isDead == false)
                    {
                        numofEnemie = i;
                    }
                }
            }
            
            /*if (trainingEncounter.enemies[numofEnemie].isDead == true)
            {
                Enemy temp = trainingEncounter.enemies.Find(x => x.currentHealth >= x.maxHealth);
                for (int i = 0; i < trainingEncounter.enemies.Count; i++)
                {
                    if (trainingEncounter.enemies[i] == temp)
                    {
                        numofEnemie = i;
                    }
                }
            }*/

            int rndAttack = Random.Range(0, 5);
            while (attackNum == rndAttack)
            {
                rndAttack = Random.Range(0, 5);
            }
            attackNum = rndAttack;

            if (rndAttack == 3)
            {
                if (posionAmnt <= 0)
                {
                    rndAttack = Random.Range(0, 3);
                }
            }
            else if (rndAttack == 4)
            {
                if (healAmnt <= 0)
                {
                    if (posionAmnt <= 0)
                    {
                        rndAttack = Random.Range(0, 3);
                    }
                    else
                    {
                        rndAttack = Random.Range(0, 4);
                    }
                }
            }
            
            Nodes node = new Nodes();
            //if (intiative == -1)
            {
                switch (rndAttack)
                {
                    case 0:
                        trainingEncounter.DealDamageToTrainingEnemy(numofEnemie,direct.damageDealt);
                        node.playerUsedAttack = direct;
                        node.playerUsedItem = null;
                        attackUsedThisTurn = direct;
                        break;
                    case 1:
                        trainingEncounter.DealDamageToTrainingEnemy(numofEnemie,flyTo.damageDealt);
                        node.playerUsedAttack = flyTo;
                        node.playerUsedItem = null;
                        attackUsedThisTurn = flyTo;
                        break;
                    case 2:
                        node.playerUsedAttack = aoe;
                        node.playerUsedItem = null;
                        attackUsedThisTurn = aoe;
                        for (int i = 0; i < trainingEncounter.enemies.Count; i++)
                        {
                            if (trainingEncounter.enemies[i].isDead == false)
                            {
                                trainingEncounter.DealDamageToTrainingEnemy(i,aoe.damageDealt);
                            }
                        }
                        break;
                    case 3:
                        posionAmnt--;
                        node.playerUsedItem = poision;
                        node.playerUsedAttack = null;
                        itemUsedThisTurn = poision;
                        trainingEncounter.enemies[numofEnemie].debuff = poision;
                        trainingEncounter.enemies[numofEnemie].turnsRemaining = poision.amntOfTurns;
                        break;
                    case 4:
                        healAmnt--;
                        node.playerUsedItem = heal;
                        node.playerUsedAttack = null;
                        itemUsedThisTurn = heal;
                        HealPartyMember(currentTrainingPartyInitiative, heal.healAmount);
                        break;
                }
            }/*
            else
            {
                int num = Random.Range(0, trainingDoggos[intiative].doggoAttacks.Count);
                trainingEncounter.DealDamageToTrainingEnemy(numofEnemie,trainingDoggos[intiative].doggoAttacks[num].damageDealt);
            }*/

            foreach (var variable in trainingEncounter.enemies)
            {
                node.currentEncounterHealth.Add(variable.currentHealth);
            }

            if (intiative == -1)
            {
                node.whoAttacked = 0;
            }
            else
            {
                node.whoAttacked = 1;
            }
            battleManagerTraining.currentTurnNode = node;
            
            if (trainingEncounter.IsTrainingEncounterDead())
            {
                //yield return new WaitForSeconds(0.5f);
                battleManagerTraining.currentState = BattleManagerTraining.TrainingBattleState.Lost;
                battleManagerTraining.StartBattler();
            }
            else
            {
                //yield return new WaitForSeconds(0.5f);
                battleManagerTraining.currentState = BattleManagerTraining.TrainingBattleState.Encounterturn;
                battleManagerTraining.StartBattler();
            }
        }
    }

    public void HealPartyMember(int i, int healAmount)
    {
        if (i == -1)
        {
            if (currentTrainingPlayerHealth < 100)
            {
                currentTrainingPlayerHealth += healAmount;
                //Debug.Log($"Player Healed for {healAmount} ");
            }
        }
        else
        {
            if (trainingDoggos[i].currentHealth < 100)
            {
                trainingDoggos[i].currentHealth += healAmount;
                //Debug.Log($"Doggo Healed for {healAmount} ");
            }
        }
    }


    public void TrainingPartyInitiative()
    {
        currentTrainingPartyInitiative++;

        if (currentTrainingPartyInitiative == -1 && isTrainingPlayerDead == false)
        {
            //Debug.Log("current party int" + currentTrainingPartyInitiative);
            //return currentTrainingPartyInitiative;
        }
        else
        {
            if (currentTrainingPartyInitiative > trainingDoggos.Count - 1)
            {
                currentTrainingPartyInitiative = -2;
                currentTrainingPartyInitiative++;
                //Debug.Log("current party int" + currentTrainingPartyInitiative);
                //return currentTrainingPartyInitiative;
            }
            else
            {
                if (trainingDoggos[currentTrainingPartyInitiative].isDead == true ||
                    trainingDoggos[currentTrainingPartyInitiative].currentHealth <= 0)
                {
                    Doggo doggo = trainingDoggos.Find(x => (x.isDead == false));
                    //Debug.Log("FOUND ENEMY: " + doggo.doggoName);
                    for (int i = 0; i < trainingDoggos.Count; i++)
                    {
                        if (trainingDoggos[i] == doggo)
                        {
                            currentTrainingPartyInitiative = i;
                            break;
                        }
                    }
                }
                //Debug.Log("current party int" + currentTrainingPartyInitiative);
                //return currentTrainingPartyInitiative;
            }
        }
    }
}
