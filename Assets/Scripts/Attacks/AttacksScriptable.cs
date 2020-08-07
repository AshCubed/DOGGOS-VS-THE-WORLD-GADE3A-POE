using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "new Attack", menuName = "Create Attack")]
public class AttacksScriptable : ScriptableObject
{
    public string attackName;
    public int damageDealt;
    [TextArea(3, 10)]
    public string toolTipText;
    public GameObject attackSprite;
    public GameObject attackHitSprite;
    public enum  AttackType {AOE, FlyTo, DirectAttack};
    public AttackType attackType;


    public void AttackTarget(EncounterScript currentncounter, Party currentParty, bool isToHitEnemy)
    {
        BattleManagerV2 BV2 = FindObjectOfType<BattleManagerV2>();
        switch (attackType)
        {
            case AttackType.AOE:
                if (isToHitEnemy)
                {
                    for (int i = 0; i < currentncounter.Enemies.Count; i++)
                    {
                        if (attackSprite)
                        {
                            if (currentncounter.Enemies[i].currentBattlePos)
                            {
                                GameObject newGo = Instantiate(attackSprite, currentncounter.Enemies[i].currentBattlePos.transform.position, Quaternion.identity);
                                newGo.GetComponent<AttackAnimationScript>().SetVariables(false, true, i, 
                                    damageDealt, null, currentncounter, currentParty);
                                BV2.SetAllButtons(false);
                            }
                        }
                        //currentncounter.DealDamageToEnemy(i, damageDealt);
                    }
                }
                else
                {
                    if (attackSprite)
                    {
                        GameObject newGo = Instantiate(attackSprite, currentParty.player.transform.position,
                            Quaternion.identity);
                        newGo.GetComponent<AttackAnimationScript>().SetVariables(false, false, -1, 
                            damageDealt, null, currentncounter, currentParty);
                        BV2.SetAllButtons(false);
                    }

                    for (int i = 0; i < currentParty.doggos.Count; i++)
                    {
                        if (attackSprite)
                        {
                            GameObject newGo = Instantiate(attackSprite, currentParty.doggos[i].currentBattlePos.transform.position,
                                Quaternion.identity);
                            newGo.GetComponent<AttackAnimationScript>().SetVariables(false, false, i, 
                                damageDealt, null, currentncounter, currentParty);
                            BV2.SetAllButtons(false);
                        }
                    }
                }

                break;
            case AttackType.FlyTo:
                Debug.Log(attackName + damageDealt);
                FindObjectOfType<BattleManagerV2>().attacksScriptableInUse = this;
                FindObjectOfType<BattleManagerV2>().itemScritpableInUse = null;
                FindObjectOfType<BattleManagerV2>().SetEnemyCharacterButtonsInteractable(true);
                break;
            case AttackType.DirectAttack:
                Debug.Log(attackName + damageDealt);
                FindObjectOfType<BattleManagerV2>().attacksScriptableInUse = this;
                FindObjectOfType<BattleManagerV2>().itemScritpableInUse = null;
                FindObjectOfType<BattleManagerV2>().SetEnemyCharacterButtonsInteractable(true);
                break;
        }
    }
}
