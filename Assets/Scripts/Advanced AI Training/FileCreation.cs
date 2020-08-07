using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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

    public void ClearFiles()
    {
        if (File.Exists(trainingNodesPath))
        {
            File.WriteAllText(trainingNodesPath, "");
        }
        /*if (File.Exists(savedNodesPath))
        {
            File.WriteAllText(savedNodesPath, "");
        }*/
    }

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
                     if (stringToConvert[i + 1] != "" || stringToConvert[i + 1] != " " || stringToConvert[i + 1] != null)
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
                         i = i + 8;
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
        string beeep = "------";
        if (nodeses.Count > 0)
        {
            for (int i = 0; i < nodeses.Count; i++)
            {
                beeep += "\n";
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
                beeep += "------";
            }
        }
        //beeep += "\n";
        return beeep;
    }
}
