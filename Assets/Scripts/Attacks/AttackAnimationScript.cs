using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AttackAnimationScript : MonoBehaviour
{
    private int i;
    private int damage;
    private GameObject hitSprite;
    private EncounterScript currentEncounter;
    private Party currentParty;
    private bool isEnemyToHit;
    private bool isAttackAnimation;
    private BattleManagerV2 BV2;

    private Vector2 targetPos;
    private float speed = 6.0f;

    private void Start()
    {
        BV2 = FindObjectOfType<BattleManagerV2>();
    }

    public void Update()
    {
        if (isSet)
        {
            if (isAttackAnimation)
            {
                float step = speed * Time.deltaTime;
                transform.position = Vector2.MoveTowards(gameObject.transform.position, targetPos, step);
            } 
        }
    }

    private bool isSet = false;
    public void MoveTowardsTarget(Vector2 target)
    {
        if (!isSet)
        {
            targetPos = target;
        }
        isSet = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isEnemyToHit)
        {
            if (collision.gameObject.CompareTag(i.ToString()))
            {
                currentEncounter.DealDamageToEnemy(i, damage);
                Instantiate(hitSprite, collision.transform.position, Quaternion.identity);
                
                CheckWinLose();
                
                DestroyObject();
            }
        }
        else
        {
            if (collision.gameObject.CompareTag(i.ToString()))
            {
                currentParty.TakeDamage(i, damage);
                Instantiate(hitSprite, collision.transform.position, Quaternion.identity);
                
                CheckWinLose();
 
                DestroyObject();
            }
        }
    }

    public void SetVariables(bool attackAnimation,bool hitEnemy, int enemyNum, int damageToDeal, GameObject hitSpriteSpawn, EncounterScript currentEncounterHere, Party currentPartyHere)
    {
        i = enemyNum;
        damage = damageToDeal;
        hitSprite = hitSpriteSpawn;
        currentEncounter = currentEncounterHere;
        currentParty = currentPartyHere;
        isEnemyToHit = hitEnemy;
        isAttackAnimation = attackAnimation;
    }

    public void AOE()
    {
        if (isEnemyToHit)
        {
            currentEncounter.DealDamageToEnemy(i, damage);

            if (i == 0 && currentEncounter.Enemies[i].isDead == false)
            {
                CheckWinLose();
                Debug.Log("CheckWinLose Attack slap" + i);
            }
            else if (i == 1 && currentEncounter.Enemies[i].isDead == false && currentEncounter.Enemies[0].isDead == true)
            {
                CheckWinLose();
                Debug.Log("CheckWinLose Attack slap" + i);
            }
            else if (i == 2 && currentEncounter.Enemies[i].isDead == false && currentEncounter.Enemies[0].isDead == true && currentEncounter.Enemies[1].isDead == true)
            {
                CheckWinLose();
                Debug.Log("CheckWinLose Attack slap" + i);
            }
            
        }
        else
        {
            currentParty.TakeDamage(i, damage);
            if (i == -1 && currentParty.player.isDead == false)
            {
                CheckWinLose(); //put in AOE animations timeline
            }
            else if (i == 0 && currentParty.doggos[i].isDead == false  && currentParty.player.isDead == true)
            {
                CheckWinLose();
            }
            else if (i == 1 && currentParty.doggos[i].isDead == false && currentParty.player.isDead == true && currentParty.doggos[0].isDead == true)
            {
                CheckWinLose();
            }
        }
    }

    public void DealTheDamage()
    {
        if (isEnemyToHit)
        {
            currentEncounter.DealDamageToEnemy(i, damage);
        }
    }

    public void CheckWinLose()
    {
        if (isEnemyToHit)
        {
            if (currentEncounter.IsEncounterDead())
            {
                BV2.currentState = BattleManagerV2.BattleState.Won;
                BV2.BattleStateCheck();
            }
            else
            {
                BV2.currentState = BattleManagerV2.BattleState.Encounterturn;
                BV2.BattleStateCheck();
            }
        }
        else
        {
            if (currentParty.IsPartyDead())
            {
                BV2.currentState = BattleManagerV2.BattleState.Lost;
                BV2.BattleStateCheck();
            }
            else
            {
                BV2.runPlayerInitiative = false;
                BV2.currentState = BattleManagerV2.BattleState.Partyturn;
                BV2.BattleStateCheck();
            }
        }
    }

    public void PlayAudio(string test)
    {
        FindObjectOfType<AudioManager>().Play(test);
    }

    public void StopAudio(string test)
    {
        FindObjectOfType<AudioManager>().Stop(test);
    }

    public void DestroyObject()
    {
        Destroy(this.gameObject);
    }
}
