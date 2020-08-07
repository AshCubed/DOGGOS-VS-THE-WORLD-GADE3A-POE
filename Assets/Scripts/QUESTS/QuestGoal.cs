using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class QuestGoal
{
    public enum GoalType {None, Kill, Gathering };
    //[HideInInspector]
    public GoalType goalType;
    //[HideInInspector]
    public EncounterScript currentEncounterTarget;
    //[HideInInspector]
    public ItemScritpable itemToPickUp;
    //[HideInInspector]
    public int requiredAmount;

    public bool QuestGoalCheck(Player player)
    {
        switch (goalType)
        {
            case GoalType.None:
                break;
            case GoalType.Kill:
                if (currentEncounterTarget)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            case GoalType.Gathering:
                return (player.playerPickUpItemses[player.playerPickUpItemses.FindIndex
                    (x => x.possiblePickUpItem == itemToPickUp)].amountOfItem >= requiredAmount);
            default:
                break;
        }

        return false;
    }

    public static explicit operator QuestGoal(UnityEngine.Object v)
    {
        throw new NotImplementedException();
    }


}

#if UNITY_EDITOR
[CustomEditor(typeof(QuestGoal))]
public class QuestGoal_Editor : Editor
{
    public UnityEngine.Object _CurrentEnemy;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        QuestGoal script = (QuestGoal)target;

        EditorGUILayout.EnumFlagsField(script.goalType);

        switch (script.goalType)
        {
            case QuestGoal.GoalType.None:
                break;
            case QuestGoal.GoalType.Kill:
                GUILayout.BeginHorizontal();
                _CurrentEnemy = EditorGUILayout.ObjectField("Current Enemy", _CurrentEnemy, typeof(Enemy), true);
                GUILayout.EndHorizontal();
                break;
            case QuestGoal.GoalType.Gathering:
                //script.currentAmount = EditorGUILayout.IntField("Current Amount", script.currentAmount);
                script.requiredAmount = EditorGUILayout.IntField("Required Amount", script.requiredAmount);
                break;
            default:
                break;
        }
    }
}
#endif


