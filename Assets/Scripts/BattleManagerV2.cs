using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO;
using UnityEngine.Serialization;

public class BattleManagerV2 : MonoBehaviour
{
    [Header("Audio Manager")] private AudioManager audioManager;
    [Header("Current Battle")] 
    private Party currentParty;
    private EncounterScript currentEncounter;
    private string playerTag;

    public enum BattleState {Start, Partyturn, Encounterturn, Won, Lost };
    [Header("Current Game State")]
    public BattleState currentState;

    [Header("Cameras")]
    public GameObject mainCamera;
    public GameObject battleCamera;

    [Header("Things to Spawn")]
    public GameObject btncharacter;
    public GameObject btnEnemyCharacter;
    public GameObject btnAbilitiesItems;
    public GameObject pnlAttacksItems;

    [Header("Sprite Spawn Points")]
    public Transform playerSpawnPoint;
    public List<Transform> possibleDoggoSpawnPoints;
    public List<Transform> possibleEnemySpawnPoints;

    [Header("Things to Parent to")]
    public GameObject parentPartyButtons;
    public GameObject parentEnemyButtons;
    public GameObject parentAttacksItems;

    [Header("Battle ToolTip reference")]
    public GameObject toolTip;
    public TextMeshProUGUI txtToolTip;

    [Header("Multiplayer Canvas and Items")]
    public Canvas multiplayerCanvas;
    public GameObject multiplayerPanel;
    public Text txtMultiEnemyName;
    public Animator multiplayerAnim;
    public string multiStartKeyWord;

    [Header("All Buttons Spawned")] public List<GameObject> spawnedThings;

    [Header("MAIN Animators")] 
    public Animator battleTransitionAnimator;
    public string transitionStartKeyWord;
    public string transitionExitKeyWord;
    public Animator currentTurnTextAnimator;
    public String turnTextString;

    private Vector3 playerOgPosition;
    private GameObject currentActiveAbilitiesItemsPanel = null;

    private bool runCharacterButtons = false;
    private bool runAttackItemButtons = false;
    private bool runSpawnSprites = false;
    [HideInInspector] public bool runPlayerInitiative = false;
    private bool enemyRunThrough = false;
    private bool partyTurn = false;
    

    [HideInInspector] public AttacksScriptable attacksScriptableInUse;
    [HideInInspector] public ItemScritpable itemScritpableInUse;
    [HideInInspector] public int currentPartyInitiative;
    [HideInInspector] public bool isInBattle;

    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    void Start()
    {
        isInBattle = false;
        spawnedThings = new List<GameObject>();
        currentPartyInitiative = -1;
        multiplayerCanvas.gameObject.SetActive(false);
        enemyRunThrough = false;
        partyTurn = false;
        runPlayerInitiative = false;
        partyTurn = false;
    }

    IEnumerator BattleStateCheckStart()
    {
        switch (currentState)
        {
            case BattleState.Start:
                break;
            case BattleState.Partyturn:
                if (partyTurn == false)
                {
                    Debug.Log("THIS IS RUNNING MORE THAN I WANT IT OOO");
                    //SetAllButtons(false);  Removed because would cause Player buttosn at start to be uninteractable
                    currentTurnTextAnimator.SetBool(turnTextString, false);
                    enemyRunThrough = false;
                    runPlayerInitiative = false;
                    partyTurn = true;
                    PartyInitiativeCheck();
                }
                else
                {
                    Debug.Log("ATTEMPTED TO GO THROUGH PARTY TURN, BUT FAILED" + partyTurn);
                }
                break;
            case BattleState.Encounterturn:
                partyTurn = false;
                currentTurnTextAnimator.SetBool(turnTextString, true);
                SetAllButtons(false);
                yield return new WaitForSeconds(1f);
                //Apply Debufs
                for (int i = 0; i < currentEncounter.Enemies.Count; i++)
                {
                    if (currentEncounter.Enemies[i].debuff)
                    {
                        currentEncounter.Enemies[i].txtDebuffEnemy.gameObject.SetActive(true);
                        if (currentEncounter.Enemies[i].turnsRemaining != 0)
                        {
                            currentEncounter.Enemies[i].txtDebuffEnemy.text = currentEncounter.Enemies[i].debuff.itemName.Substring(0, 1) + " " + currentEncounter.Enemies[i].turnsRemaining + "/" + currentEncounter.Enemies[i].debuff.amntOfTurns;
                        }
                        else
                        {
                            currentEncounter.Enemies[i].txtDebuffEnemy.gameObject.SetActive(false);
                        }
                        currentEncounter.Enemies[i].debuff.ApplyDebuff(currentEncounter, i);
                    }
                }
                yield return new WaitForSeconds(1f);
                if (FindObjectOfType<DifficultyManager>().typeOfAi == DifficultyManager.TypeOfAi.NormalAi)
                {
                    switch (FindObjectOfType<DifficultyManager>().typeOfPlay)
                    {
                        case DifficultyManager.TypeOfPlay.SinglePlayer:
                            StartCoroutine(EncounterTurn());
                            break;
                        case DifficultyManager.TypeOfPlay.Multiplayer:
                            multiplayerCanvas.gameObject.SetActive(true);
                            txtMultiEnemyName.text = currentEncounter.Enemies[currentEncounter.ReturnInitiative()]
                                .enemyName;
                            multiplayerAnim.SetTrigger(multiStartKeyWord);
                            break;
                    }       
                }
                else
                {
                    StartCoroutine(EncounterTurn());
                }
                break;
            case BattleState.Won:
                StartCoroutine(WonBattle());
                break;
            case BattleState.Lost:
                StartCoroutine(LostBattle());
                break;
            default:
                break;
        }
    }
    public void BattleStateCheck()
    {
        StartCoroutine(BattleStateCheckStart());
    }

    public void StartBattler(Party party, EncounterScript encounter)
    {
        FindObjectOfType<QuestWayPoint>().SetWayPointView(false);
        party.player.SetOnScreenPlayerElements(false);
        //StartCoroutine(StartBattle(party, encounter));
        currentParty = party;
        currentEncounter = encounter;
        currentParty.player.currentMoveSpeed = 0;
        battleTransitionAnimator.SetBool(transitionStartKeyWord, true);
        isInBattle = true;
        partyTurn = false;
    }

    public void ExecuteBattler()
    {
        currentPartyInitiative = -1;
        audioManager = FindObjectOfType<AudioManager>();
        audioManager.StopAll();
        audioManager.Play("BattleMusic");
        //Spawn Party and Enemy Sprites
        runSpawnSprites = false;
        SpawnSprites();

        //set player speed to 0
        currentParty.player.currentMoveSpeed = 0;

        //Spawn Attack and Item Buttons (panels) for each player/doggo
        runAttackItemButtons = false;
        SpawnAttackAndItemButtons();

        //Spawn Character Buttons
        runCharacterButtons = false;
        SpawnCharacterButtons();
        SetAllToMaxHealth();

        #region Set max health for all characters in health bar
        currentParty.player.playerHealthBar.SetMaxHealth(currentParty.player.maxHealth);
        for (int i = 0; i < currentParty.doggos.Count; i++)
        {
            if (currentParty.doggos[i].doggoInUse)
            {
                currentParty.doggos[i].doggoHealthBar.SetMaxHealth(currentParty.doggos[i].maxHealth);
            }
        }

        for (int i = 0; i < currentEncounter.Enemies.Count; i++)
        {
            currentEncounter.Enemies[i].enemyHealthBar.SetMaxHealth(currentEncounter.Enemies[i].maxHealth);
        }
        #endregion

        //change currentstate and turn on battle camera
        currentState = BattleState.Partyturn;
        BattleStateCheck();
    }

    private void SpawnSprites()
    {
        if (!runSpawnSprites)
        {
            //Spawn Player Sprites
            playerOgPosition = currentParty.player.transform.position;
            currentParty.player.transform.position = playerSpawnPoint.position;
            playerTag = currentParty.player.tag;
            currentParty.player.tag = Convert.ToString(-1);
            //Spawn Doggo Sprites
            for (int i = 0; i < currentParty.doggos.Count; i++)
            {
                if (currentParty.doggos[i].doggoInUse)
                {
                    GameObject newGo = Instantiate(currentParty.doggos[i].thisDoggoGameObject, possibleDoggoSpawnPoints[i].position, Quaternion.identity);
                    newGo.tag = Convert.ToString(i);
                    newGo.AddComponent<BoxCollider2D>();
                    newGo.AddComponent<Rigidbody2D>();
                    newGo.GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                    newGo.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                    if (!newGo.activeSelf)
                    {
                        newGo.SetActive(true);
                    }
                    currentParty.doggos[i].currentBattlePos = newGo;
                    AddToSpawnedThings(newGo);
                }
            }
            //Spawn EncounterSprites
            for (int i = 0; i < currentEncounter.Enemies.Count; i++)
            {
                GameObject newGo = Instantiate(currentEncounter.Enemies[i].enemyGameObject, possibleEnemySpawnPoints[i].position, Quaternion.identity);
                newGo.AddComponent<BoxCollider2D>();
                newGo.AddComponent<Rigidbody2D>();
                newGo.GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                newGo.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                currentEncounter.Enemies[i].currentBattlePos = newGo;
                newGo.gameObject.tag = i.ToString();
                AddToSpawnedThings(newGo);
            }
            runSpawnSprites = true;
        }
        runSpawnSprites = true;
    }

    private void SpawnAttackAndItemButtons()
    {
        if (!runAttackItemButtons)
        {
            //Spawn Player Panels
            GameObject pnlPlayerAttackItems = Instantiate(pnlAttacksItems, parentAttacksItems.transform);
            currentParty.player.pnlPlayerAbilitiesItems = pnlPlayerAttackItems;
            GameObject pnlPlayerAbilities = pnlPlayerAttackItems.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).transform.gameObject;
            GameObject pnlPlayerItems = pnlPlayerAttackItems.transform.GetChild(1).GetChild(1).GetChild(0).GetChild(0).transform.gameObject;
            AddToSpawnedThings(pnlPlayerAttackItems);
            //Spawn Player Attack btns
            for (int j = 0; j < currentParty.player.playerAttacks.Count; j++)
            {
                GameObject newGo1 = Instantiate(btnAbilitiesItems, pnlPlayerAbilities.transform);
                newGo1.GetComponentInChildren<Text>().text = currentParty.player.playerAttacks[j].item.attackName;
                newGo1.GetComponent<ToolTipScript>().rawImage = toolTip;
                newGo1.GetComponent<ToolTipScript>().txt = txtToolTip;
                newGo1.GetComponent<ToolTipScript>().As = currentParty.player.playerAttacks[j].item;
                newGo1.GetComponent<Button>().onClick.AddListener(() => { newGo1.GetComponent<ToolTipScript>().As.AttackTarget(currentEncounter, currentParty, true); });
                AddToSpawnedThings(newGo1);
            }
            //Spawn Player Items btns
            for (int j = 0; j < currentParty.player.playerItems.Count; j++)
            {
                GameObject newGo1 = Instantiate(btnAbilitiesItems, pnlPlayerItems.transform);
                currentParty.player.playerItems[j].playerItemBtn = newGo1.GetComponent<Button>();
                newGo1.GetComponentInChildren<Text>().text = currentParty.player.playerItems[j].item.itemName + " " +  currentParty.player.playerItems[j].itemAmount;
                newGo1.GetComponent<ToolTipScript>().rawImage = toolTip;
                newGo1.GetComponent<ToolTipScript>().txt = txtToolTip;
                newGo1.GetComponent<ToolTipScript>().Is = currentParty.player.playerItems[j].item;
                AddToSpawnedThings(newGo1);
                newGo1.GetComponent<Button>().onClick.AddListener(() =>
                { newGo1.GetComponent<ToolTipScript>().Is
                        .ItemPlayerActivate(currentParty.player); });
            }
            //Spawn Doggo Attack panel and btns
            foreach (Doggo doggoItem in currentParty.doggos)
            {
                if (doggoItem.doggoInUse)
                {
                    GameObject newGo = Instantiate(pnlAttacksItems, parentAttacksItems.transform);
                    GameObject pnlAbilities = newGo.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).transform.gameObject;
                    GameObject pnlItems = newGo.transform.GetChild(1).GetChild(1).GetChild(0).GetChild(0).transform.gameObject;
                    doggoItem.pnlDoggoAbilitiesItems = newGo;
                    AddToSpawnedThings(newGo);
                    //Doggo Attack Buttons
                    for (int j = 0; j < doggoItem.doggoAttacks.Count; j++)
                    {
                        GameObject newGo1 = Instantiate(btnAbilitiesItems, pnlAbilities.transform);
                        newGo1.GetComponentInChildren<Text>().text = doggoItem.doggoAttacks[j].attackName;
                        newGo1.GetComponent<ToolTipScript>().rawImage = toolTip;
                        newGo1.GetComponent<ToolTipScript>().txt = txtToolTip;
                        newGo1.GetComponent<ToolTipScript>().As = doggoItem.doggoAttacks[j];
                        newGo1.GetComponent<Button>().onClick.AddListener(() => { newGo1.GetComponent<ToolTipScript>().As.AttackTarget(currentEncounter, 
                            currentParty, true); });
                        AddToSpawnedThings(newGo1);
                    }
                    //Doggo Item Buttons [A shared Inventory]
                    foreach (PlayerItems VARIABLE in currentParty.player.playerItems)
                    {
                        DoggoItems DGI = new DoggoItems();
                        GameObject newGo1 = Instantiate(btnAbilitiesItems, pnlItems.transform);
                        //currentParty.player.playerItems[j].playerItemBtn = newGo1.GetComponent<Button>();
                        newGo1.GetComponentInChildren<Text>().text = VARIABLE.item.itemName + " " +  
                                                                     VARIABLE.itemAmount;
                        
                        newGo1.GetComponent<ToolTipScript>().rawImage = toolTip;
                        newGo1.GetComponent<ToolTipScript>().txt = txtToolTip;
                        newGo1.GetComponent<ToolTipScript>().Is = VARIABLE.item;
                        AddToSpawnedThings(newGo1);
                        newGo1.GetComponent<Button>().onClick.AddListener(() =>
                        { newGo1.GetComponent<ToolTipScript>().Is
                            .ItemDoggoUse(doggoItem, currentParty.player); });
                        
                        DGI.doggoItemBtn = newGo1.GetComponent<Button>();
                        DGI.doggoItemRef = newGo1.GetComponent<ToolTipScript>().Is;
                        doggoItem.doggoItems.Add(DGI);
                    }
                }
            }

            //Setting Panels Active to false
            currentParty.player.pnlPlayerAbilitiesItems.SetActive(false);
            for (int i = 0; i < currentParty.doggos.Count; i++)
            {
                if (currentParty.doggos[i].doggoInUse)
                {
                    currentParty.doggos[i].pnlDoggoAbilitiesItems.SetActive(false);
                }
            }
        }
        runAttackItemButtons = true;
    }

    private void SpawnCharacterButtons()
    {
        if (!runCharacterButtons)
        {
            //Spawn Player character button
            GameObject playerBtn = Instantiate(btncharacter, parentPartyButtons.transform);
            currentParty.player.btnPlayerCharacter = playerBtn;
            playerBtn.GetComponentInChildren<Text>().text = currentParty.player.heroName;
            playerBtn.GetComponentInChildren<HealthBar>().SetHealth(currentParty.player.currentHealth);
            currentParty.player.playerHealthBar = playerBtn.GetComponentInChildren<HealthBar>();
            playerBtn.transform.GetChild(2).gameObject.SetActive(false);
            //playerBtn.gameObject.GetComponent<Button>().onClick.AddListener(() => { ShowAbilitiesItemsPanel(currentParty.player.pnlPlayerAbilitiesItems); });
            AddToSpawnedThings(playerBtn);
            //Spawn doggo character btns
            for (int i = 0; i < currentParty.doggos.Count; i++)
            {
                if (currentParty.doggos[i].doggoInUse)
                {
                    GameObject newGo = Instantiate(btncharacter, parentPartyButtons.transform);
                    currentParty.doggos[i].btnDoggoCharacterButton = newGo;
                    newGo.GetComponentInChildren<Text>().text = currentParty.doggos[i].doggoName;
                    newGo.GetComponentInChildren<HealthBar>().SetHealth(currentParty.doggos[i].currentHealth);
                    currentParty.doggos[i].doggoHealthBar = newGo.GetComponentInChildren<HealthBar>();
                    currentParty.doggos[i].btnDoggoCharacterButton.transform.GetChild(2).gameObject.SetActive(false);
                    //newGo.GetComponent<Button>().onClick.AddListener(() => { ShowAbilitiesItemsPanel(currentParty.doggos[i - 1].pnlDoggoAbilitiesItems); });
                    AddToSpawnedThings(newGo);
                }
            }
            //Spawn Encounter character btns
            for (int i = 0; i < currentEncounter.Enemies.Count; i++)
            {
                GameObject newGo = Instantiate(btnEnemyCharacter, parentEnemyButtons.transform);
                currentEncounter.Enemies[i].btnEnemyCharacterButton = newGo;
                newGo.GetComponentInChildren<Text>().text = currentEncounter.Enemies[i].enemyName;
                newGo.GetComponentInChildren<HealthBar>().SetHealth(currentEncounter.Enemies[i].currentHealth);
                currentEncounter.Enemies[i].enemyHealthBar = newGo.GetComponentInChildren<HealthBar>();
                currentEncounter.Enemies[i].btnEnemyCharacterButton.transform.GetChild(2).gameObject.SetActive(false);
                currentEncounter.Enemies[i].txtDebuffEnemy = newGo.transform.GetChild(3).GetComponent<Text>();
                currentEncounter.Enemies[i].txtDebuffEnemy.gameObject.SetActive(false);
                GameObject tempEnemyObject = currentEncounter.Enemies[i].currentBattlePos;
                newGo.GetComponent<Button>().onClick.AddListener(() => { EnemyCharacterButtonMethod(currentEncounter, tempEnemyObject, attacksScriptableInUse, 
                    itemScritpableInUse); });
                AddToSpawnedThings(newGo);
            }
        }
        runCharacterButtons = true;
    }

    // private void ShowAbilitiesItemsPanel(GameObject panel)
    // {
    //     if (currentActiveAbilitiesItemsPanel || currentActiveAbilitiesItemsPanel == null)
    //     {
    //         if (currentActiveAbilitiesItemsPanel != null)
    //         {
    //             SetCharacterAbilityButtonsInteractable(currentActiveAbilitiesItemsPanel, false);
    //             currentActiveAbilitiesItemsPanel.SetActive(false);
    //         }
    //         panel.SetActive(true);
    //         SetCharacterAbilityButtonsInteractable(panel, true);
    //         currentActiveAbilitiesItemsPanel = panel;
    //     }
    // }


    private void EnemyCharacterButtonMethod(EncounterScript encounter, GameObject enemy, AttacksScriptable attacksScriptable, ItemScritpable itemScritpable)
    {
        if (!enemyRunThrough)
        {
            int x = int.Parse(enemy.tag);
            if (attacksScriptable)
            {
                Debug.Log("currentPartyInitiative = " + currentPartyInitiative);
                //spawn attackSprtie
                if (currentPartyInitiative - 1 == -1)
                {
                    Vector2 spawnPoint = new Vector2(currentParty.player.transform.position.x + 0.4f, currentParty.player.transform.position.y);
                    if (attacksScriptable.attackType == AttacksScriptable.AttackType.FlyTo)
                    {
                        GameObject attackSprite = Instantiate(attacksScriptable.attackSprite, spawnPoint, Quaternion.identity);
                        AddToSpawnedThings(attackSprite);
                        attackSprite.GetComponent<AttackAnimationScript>().SetVariables(true, true, x, attacksScriptable.damageDealt, attacksScriptable.attackHitSprite, currentEncounter, null);
                        attackSprite.GetComponent<AttackAnimationScript>().MoveTowardsTarget(currentEncounter.Enemies[x].currentBattlePos.transform.position);
                    }
                    else if (attacksScriptable.attackType == AttacksScriptable.AttackType.DirectAttack)
                    {
                        GameObject attackSprite = Instantiate(attacksScriptable.attackSprite, currentEncounter.Enemies[x].currentBattlePos.transform.position, Quaternion.identity);
                        AddToSpawnedThings(attackSprite);
                        attackSprite.GetComponent<AttackAnimationScript>().SetVariables(true, true, x, attacksScriptable.damageDealt, attacksScriptable.attackHitSprite, currentEncounter, null);
                    }
                }
                else
                {
                    Vector2 spawnPoint = new Vector2(currentParty.doggos[currentPartyInitiative - 1].currentBattlePos.transform.position.x + 0.4f, currentParty.doggos[currentPartyInitiative - 1].currentBattlePos.transform.position.y);
                    if (attacksScriptable.attackType == AttacksScriptable.AttackType.FlyTo)
                    {
                        GameObject attackSprite = Instantiate(attacksScriptable.attackSprite, spawnPoint, Quaternion.identity);
                        AddToSpawnedThings(attackSprite);
                        attackSprite.GetComponent<AttackAnimationScript>().SetVariables(true, true, x, attacksScriptable.damageDealt, attacksScriptable.attackHitSprite, currentEncounter, null);
                        attackSprite.GetComponent<AttackAnimationScript>().MoveTowardsTarget(currentEncounter.Enemies[x].currentBattlePos.transform.position);

                    }
                    else if (attacksScriptable.attackType == AttacksScriptable.AttackType.DirectAttack)
                    {
                        GameObject attackSprite = Instantiate(attacksScriptable.attackSprite, currentEncounter.Enemies[x].currentBattlePos.transform.position, Quaternion.identity);
                        AddToSpawnedThings(attackSprite);
                        attackSprite.GetComponent<AttackAnimationScript>().SetVariables(true, true, x, attacksScriptable.damageDealt, attacksScriptable.attackHitSprite, currentEncounter, null);
                    }
                }
            }
            else if (itemScritpable)
            {
                GameObject itemSprite = Instantiate(itemScritpable.itemSpriteAnimation,
                    currentEncounter.Enemies[x].currentBattlePos.transform.position, Quaternion.identity);

                currentEncounter.Enemies[x].debuff = itemScritpableInUse;
                currentEncounter.Enemies[x].turnsRemaining = itemScritpableInUse.amntOfTurns;
                Instantiate(itemScritpableInUse.itemSpriteAnimation,
                    currentEncounter.Enemies[x].currentBattlePos.transform.position, Quaternion.identity);
                currentEncounter.Enemies[x].enemyHealthBar.isPoisioned = true;
                
                itemScritpableInUse.DecrementItemAmount(currentParty.player);
                
                SetEnemyCharacterButtonsInteractable(false);
                currentState = BattleManagerV2.BattleState.Encounterturn;
                BattleStateCheck();
            }



            //Reset Current Attack Scritable in use to empty
            attacksScriptableInUse = null;
            itemScritpableInUse = null;
            SetEnemyCharacterButtonsInteractable(false);
            
            //Checks if you've killed everyone, else the battle continues - moved to Attack Animation Script
            /*if (currentEncounter.IsEncounterDead())
            {
                currentState = BattleState.Won;
                BattleStateCheck();
            }
            else
            {
                currentState = BattleState.Encounterturn;
                BattleStateCheck(); 
            }*/
        }
        enemyRunThrough = true;
    }

    
    public void MultiplayerEncounterTurnBtn(int num)
    {
        StartCoroutine(EncounterTurn(num));
    }
    IEnumerator EncounterTurn(int num = 0)
    {
        Debug.Log("DAM ENCOUNTER TURN WAS CALLED");
        //int enemyAttacking;
        SetAllButtons(false);
        //yield return new WaitForSeconds(0.5f);
        //currentEncounter.EncounterAttack(currentParty);
        currentEncounter.AiState = EncounterScript.EnemyAIState.CheckingAI;
        currentEncounter.CheckAIState(currentParty, num);
        //yield return new WaitForSeconds(2f);
        runPlayerInitiative = false;
        //currentEncounter.Enemies[enemyAttacking].btnEnemyCharacterButton.transform.GetChild(2).gameObject.SetActive(false);
    
        //Checks if the enemies have killed all party members, else continues the battle

        //currentState = BattleState.Partyturn;
        //BattleStateCheck();
        yield return null;

    }

    IEnumerator WonBattle()
    {
        FindObjectOfType<QuestWayPoint>().SetWayPointView(true);
        isInBattle = false;
        audioManager.StopAll();
        audioManager.Play("Medieval Ambiance");
        battleTransitionAnimator.SetBool(transitionStartKeyWord, false);
        //yield return new WaitForSeconds(2f);
        //battleCamera.SetActive(false);
        DestroySpawnedThings();
        ErasePartyEnemyBtnVariables();
        SetAllToMaxHealth();

        //When you win, game destroys the encounter in the main game
        Destroy(currentEncounter.gameObject);
        currentParty.player.transform.position = playerOgPosition;
        currentParty.player.tag = playerTag;
        playerTag = null;

        currentParty.player.currentMoveSpeed = currentParty.player.startingMoveSpeed;
        
        //mainCamera.SetActive(true);

        currentParty.player.isDead = false;
        for (int i = 0; i < currentParty.doggos.Count; i++)
        {
            currentParty.doggos[i].isDead = false;
        }
        currentParty = null;
        currentEncounter = null;

        runCharacterButtons = false;
        runAttackItemButtons = false;
        runSpawnSprites = false;
        yield return new WaitForSeconds(1f);
        FindObjectOfType<Player>().SetOnScreenPlayerElements(true);
    }

    IEnumerator LostBattle()
    {
        FindObjectOfType<QuestWayPoint>().SetWayPointView(true);
        isInBattle = false;
        audioManager.StopAll();
        audioManager.Play("Medieval Ambiance");
        battleTransitionAnimator.SetBool(transitionStartKeyWord, false);
        //battleCamera.SetActive(false);
        DestroySpawnedThings();
        ErasePartyEnemyBtnVariables();
        SetAllToMaxHealth();
        currentParty.player.transform.position = playerOgPosition;
        currentParty.player.tag = playerTag;
        playerTag = null;
        yield return new WaitForSeconds(2f);
        currentParty.player.currentMoveSpeed = currentParty.player.startingMoveSpeed;
        //mainCamera.SetActive(true);

        currentParty.player.isDead = false;
        for (int i = 0; i < currentParty.doggos.Count; i++)
        {
            currentParty.doggos[i].isDead = false;
        }
        currentParty = null;
        foreach (Enemy VARIABLE in currentEncounter.Enemies)
        {
            VARIABLE.debuff = null;
        }
        currentEncounter = null;

        runCharacterButtons = false;
        runAttackItemButtons = false;
        runSpawnSprites = false;
        yield return new WaitForSeconds(1f);
        FindObjectOfType<Player>().SetOnScreenPlayerElements(true);
    }

    private void ErasePartyEnemyBtnVariables()
    {
        currentParty.player.btnPlayerCharacter = null;
        currentParty.player.playerHealthBar = null;
        currentParty.player.pnlPlayerAbilitiesItems = null;
        currentParty.player.isDead = false;

        foreach (Doggo item in currentParty.doggos)
        {
            item.btnDoggoCharacterButton = null;
            item.doggoHealthBar = null;
            item.pnlDoggoAbilitiesItems = null;
            item.isDead = false;
            foreach (DoggoItems VARIABLE in item.doggoItems)
            {
                VARIABLE.doggoItemBtn = null;
                VARIABLE.doggoItemRef = null;
            }
        }

        foreach (Enemy item in currentEncounter.Enemies)
        {
            item.btnEnemyCharacterButton = null;
            item.enemyHealthBar = null;
            item.isDead = false;
        }
    }

    private void SetAllToMaxHealth()
    {
        currentParty.player.currentHealth = currentParty.player.maxHealth;
        for (int i = 0; i < currentParty.doggos.Count; i++)
        {
            currentParty.doggos[i].currentHealth = currentParty.doggos[i].maxHealth;
        }
        for (int i = 0; i < currentEncounter.Enemies.Count; i++)
        {
            currentEncounter.Enemies[i].currentHealth = currentEncounter.Enemies[i].maxHealth;
        }
    }

    public void SetAllButtons(bool x)
    {
        for (int i = 0; i < spawnedThings.Count; i++)
        {
            if (spawnedThings[i] != null)
            {
                if (spawnedThings[i].GetComponent<Button>())
                {
                    spawnedThings[i].GetComponent<Button>().interactable = x;
                }
            }
        }
    }

    private void SetCharacterAbilityButtonsInteractable(GameObject panel, bool interactable)
    {
        panel.SetActive(interactable);
        Button[] beep = panel.GetComponentsInChildren<Button>();
        foreach (Button item in beep)
        {
            if (item.interactable != interactable)
            {
                item.interactable = interactable;
            }
        }
    }

    public void SetEnemyCharacterButtonsInteractable(bool x)
    {
        foreach (Enemy item in currentEncounter.Enemies)
        {
            if (item.btnEnemyCharacterButton != null)
            {
                item.btnEnemyCharacterButton.GetComponent<Button>().interactable = x;
            }
        }
    }

    

    private void PartyInitiativeCheck()
    {
        Debug.Log("Player Initiative Going In: " + currentPartyInitiative);
        if (runPlayerInitiative == false)
        {
            if ((currentPartyInitiative == -1 && currentParty.player.isDead == false) || currentPartyInitiative >= currentParty.doggos.Count)
            {
                foreach (Doggo item in currentParty.doggos)
                {
                    if (item.doggoInUse)
                    {
                        if (item.btnDoggoCharacterButton != null)
                        {
                            item.btnDoggoCharacterButton.transform.GetChild(2).gameObject.SetActive(false);
                            SetCharacterAbilityButtonsInteractable(item.pnlDoggoAbilitiesItems, false);
                            item.pnlDoggoAbilitiesItems.SetActive(false);
                        }
                    }
                }

                currentParty.player.btnPlayerCharacter.transform.GetChild(2).gameObject.SetActive(true);
                currentParty.player.pnlPlayerAbilitiesItems.SetActive(true);
                Debug.Log("Player PANELL: " + currentParty.player.pnlPlayerAbilitiesItems.activeSelf);
                SetCharacterAbilityButtonsInteractable(currentParty.player.pnlPlayerAbilitiesItems, true);

                currentPartyInitiative = -1;
                currentPartyInitiative++;
                Debug.Log("Players Turn");
                runPlayerInitiative = true;
            }
            else
            {
                if (currentPartyInitiative < currentParty.doggos.Count)
                {
                    Debug.Log("RUN1");
                    currentParty.player.btnPlayerCharacter.transform.GetChild(2).gameObject.SetActive(false);
                    SetCharacterAbilityButtonsInteractable(currentParty.player.pnlPlayerAbilitiesItems, false);
                    currentParty.player.pnlPlayerAbilitiesItems.SetActive(false);

                    if (currentParty.doggos[currentPartyInitiative] != null && currentParty.doggos[currentPartyInitiative].doggoInUse && currentParty.doggos[currentPartyInitiative].isDead == false)
                    {
                        Debug.Log("RUN2");

                        if (currentPartyInitiative != 0 )
                        {
                            if (currentParty.doggos[currentPartyInitiative - 1].btnDoggoCharacterButton)
                            {
                                currentParty.doggos[currentPartyInitiative - 1].btnDoggoCharacterButton.transform.GetChild(2).gameObject.SetActive(false);
                                SetCharacterAbilityButtonsInteractable(currentParty.doggos[currentPartyInitiative - 1].pnlDoggoAbilitiesItems, false);
                                currentParty.doggos[currentPartyInitiative - 1].pnlDoggoAbilitiesItems.SetActive(false);
                            }
                        }

                        currentParty.doggos[currentPartyInitiative].btnDoggoCharacterButton.transform.GetChild(2).gameObject.SetActive(true);
                        SetCharacterAbilityButtonsInteractable(currentParty.doggos[currentPartyInitiative].pnlDoggoAbilitiesItems, true);
                        currentParty.doggos[currentPartyInitiative].pnlDoggoAbilitiesItems.SetActive(true);
                        
                        currentPartyInitiative++;
                        Debug.Log("Doggo Turn");
                        runPlayerInitiative = true;
                    }
                    else
                    {
                        Debug.Log("RUN4");
                        currentPartyInitiative++;
                        partyTurn = false;
                        currentState = BattleState.Partyturn;
                        BattleStateCheck();
                    }
                }
                else
                {
                    Debug.Log("RUN5");
                    currentPartyInitiative = -1;
                    partyTurn = false;
                    currentState = BattleState.Partyturn;
                    BattleStateCheck();
                }
            }
            runPlayerInitiative = true;
        }
        
        int num = currentPartyInitiative - 1;
        if (num == -1)
        {
            Debug.Log("Party Member going: Player ");
        }
        else
        {
            
            Debug.Log("Party Member going: " + num + "name");
        }
    }



    private void AddToSpawnedThings(GameObject toAdd)
    {
        spawnedThings.Add(toAdd);
    }

    private void DestroySpawnedThings()
    {
        foreach (GameObject item in spawnedThings)
        {
            Destroy(item);
        }

        for (int i = 0; i < spawnedThings.Count; i++)
        {
            spawnedThings.RemoveAt(i);
        }
    }

    public void RUN()
    {
        currentState = BattleManagerV2.BattleState.Lost;
        BattleStateCheck();
    }
}
