using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Nodes : MonoBehaviour
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
