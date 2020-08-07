using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [Header("General Player Info")]
    public BattleManagerV2 battleManager;
    public float startingMoveSpeed = 0.009f;
    public float currentMoveSpeed;
    public string heroName;
    public Party party;
    public List<PlayerAttacks> playerAttacks;
    public List<PlayerItems> playerItems;
    public List<PlayerPickUpItems> playerPickUpItemses;
    public Light2D playerLight;
    public float lightFadeTime;
    private bool isLightActive;
    public bool isDead;
    public Animator playerAnim;
    public bool isInventoryOpen;
    public int currentMoneyAmnt;
    private Sprite playerSprite;
    
    [Header("Player Level System")]
    public int playerCurrentLevel;
    public int playerExperienceAmount;
    public int playerCurrentUpgradePoints;
    public int xpToNextLevel;
    public int differenceBetweenLevels;
    public GameObject playerXPCanvas;
    public GameObject playerXPBar;
    public Text playerCurrrentLeveltxt;
    public Text playerCurrentMoneytxt;
    public Text playerUpgradePointstxt;


    [Header("PlayerInventoryRef")]
    public GameObject playerInventoryCanvas;
    public GameObject playerInventoryAbilities;
    public GameObject playerInventoryItems;
    public GameObject playerInventoryDoggoSelect;
    public GameObject btnToSpawn;
    public GameObject textToSpawn;
    public TextMeshProUGUI tmproCurrentMoney;
    public TextMeshProUGUI totalActiveDoggos;

    [Header("Player Doggos Buttons")]
    public GameObject doggoBtnToSpawn;
    public GameObject toParentButtons;

    [Header("Player Quests Buttons and Such")]
    public GameObject questBtnToSpawn;
    public GameObject toParentQuestsbtn;

    [Header("Player Picked Up Items UI")] 
    public GameObject gbToSpawn;
    public Transform gbToParent;

    [Header("Health Stats")]
    public int maxHealth;
    public int currentHealth;

    [Header("Quest Stuff")] 
    public Text UIQuestText;
    public List<Quest> quests;

    [Header("BattleManager Spawned Stored Items")]
    public GameObject btnPlayerCharacter;
    public HealthBar playerHealthBar;
    public GameObject pnlPlayerAbilitiesItems;

    [Header("PasueMenu")] public GameObject pauseMenuCanvas;

    void Start()
    {
        playerCurrrentLeveltxt.text = "Lvl " + playerCurrentLevel;
        playerCurrentLevel = 0;
        playerExperienceAmount = 0;
        playerCurrentUpgradePoints = 0;
        playerUpgradePointstxt.gameObject.SetActive(false);
        pauseMenuCanvas.SetActive(true);
        playerSprite = GetComponent<SpriteRenderer>().sprite;
        playerLight.intensity = 0f;
        isLightActive = false;
        isDead = false;
        SetXPBar(playerExperienceAmount);
        MoneyTextUpdate(currentMoneyAmnt);
        currentMoveSpeed = startingMoveSpeed;
        currentHealth = maxHealth;
        playerInventoryCanvas.SetActive(false);
        isInventoryOpen = false;
        PlayerInventoryAbilitiesItemsBtnCreate();
        totalActiveDoggos.text = "0/" + party.partyMax;
        UIQuestText.text = "";
        
        //Object[] data = AssetDatabase.LoadAllAssetsAtPath("Assets/Scripts/ItemS/PickUpItems");
        /*List<ItemScritpable> pickUpItems = FindAssetsByType<ItemScritpable>();
        Debug.Log(pickUpItems.Count);
        foreach (ItemScritpable VARIABLE in pickUpItems)
        {
            if (VARIABLE.itemType == ItemScritpable.ItemType.PickUpItem)
            {
                PlayerPickUpItems test = new PlayerPickUpItems();
                test.possiblePickUpItem = VARIABLE;
                test.amountOfItem = 0;
                playerPickUpItemses.Add(test);
            }
        }*/
    }

    void Update()
    {
        #region Movement
        if (currentMoveSpeed != 0)
        {
            if (Input.GetKey((KeyCode.W)))
            {
                playerAnim.SetInteger("movementInt", 3);
                transform.Translate(Vector2.up * currentMoveSpeed);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                playerAnim.SetInteger("movementInt", 1);
                transform.Translate(Vector2.right * currentMoveSpeed);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                playerAnim.SetInteger("movementInt", 2);
                transform.Translate(Vector2.left * currentMoveSpeed);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                playerAnim.SetInteger("movementInt", 4);
                transform.Translate(Vector2.down * currentMoveSpeed);
            }
            else
            {
                if (playerAnim.GetInteger("movementInt") != 0)
                {
                    playerAnim.SetInteger("movementInt", 0);
                }
            }
        }
        #endregion

        //Opening Inventory
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (FindObjectOfType<BattleManagerV2>().isInBattle == false)
            {
                if (isInventoryOpen == false)
                {
                    tmproCurrentMoney.text = "Money: " + currentMoneyAmnt.ToString();
                    isInventoryOpen = true;
                    currentMoveSpeed = 0;
                    foreach (PlayerItems VARIABLE in playerItems)
                    {
                        VARIABLE.updatebtnAmount();
                    }
                    playerInventoryCanvas.SetActive(true);
                }
            }
        }
        
        //Opeining PauseMenu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //if (FindObjectOfType<BattleManagerV2>().isInBattle == false)
            {
                //pauseMenuCanvas.SetActive(true);
                FindObjectOfType<SettingsManager>().OpenSettings();
            }
        }

        //Fade Input for players light
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (FindObjectOfType<BattleManagerV2>().isInBattle == false)
            {
                if (isLightActive)
                {
                    StartCoroutine(PlayerLightFade(playerLight, false, lightFadeTime));
                    isLightActive = false;
                }
                else
                {
                    StartCoroutine(PlayerLightFade(playerLight, true, lightFadeTime));
                    isLightActive = true;
                }
            }
        }

        //Continously Checking if the Quest is Complete
        for (int i = 0; i < quests.Count; i++)
        {
            if (quests[i] != null && quests[i].isActive == true)
            {
                quests[i].Complete(quests, UIQuestText, this);
            }
        }
    }

    public void IncreaseXP(int xp = 0, int gold = 0)
    {
        MoneyTextUpdate(gold);
        int tempNum = playerExperienceAmount + xp;
        
        if (tempNum >= xpToNextLevel)
        {
            int temp2 = tempNum - xpToNextLevel;
            xpToNextLevel += differenceBetweenLevels;
            playerCurrentLevel++;
            playerCurrrentLeveltxt.text = "Lvl " + playerCurrentLevel;
            
            playerCurrentUpgradePoints++;
            playerUpgradePointstxt.text = "Upgrade Points: " + playerCurrentUpgradePoints;
            playerUpgradePointstxt.gameObject.SetActive(true);

            if (temp2 >= xpToNextLevel)
            {
                IncreaseXP(temp2);
            }
            else
            {
                playerExperienceAmount = temp2;
                SetXPBar(playerExperienceAmount);
                FindObjectOfType<NotificationManager>().StartNotification("Level Up: Lvl " + playerCurrentLevel + "Gold: " + currentMoneyAmnt);
            }
        }
        else
        {
            playerExperienceAmount += xp;
            SetXPBar(playerExperienceAmount);
        }
    }

    private void SetXPBar(int XP)
    {
        Slider xpSlider = playerXPBar.gameObject.GetComponent<Slider>();
        Text xpText = playerXPBar.transform.GetChild(2).gameObject.GetComponent<Text>();

        xpSlider.maxValue = xpToNextLevel;
        xpSlider.value = XP;
        xpText.text = XP + "/" + xpToNextLevel + " XP";
    }

    public void MoneyTextUpdate(int num = 0, int addSub = 0)
    {
        if (addSub == 0)
        {
            currentMoneyAmnt += num;
        }
        
        playerCurrentMoneytxt.text = "Money: " + currentMoneyAmnt.ToString();
        tmproCurrentMoney.text = "Money: " + currentMoneyAmnt.ToString();
    }

    public void UseUpgradePoint(int requiredAmnt)
    {
        if (playerCurrentUpgradePoints >= requiredAmnt)
        {
            playerCurrentUpgradePoints--;
            playerUpgradePointstxt.text = "Upgrade Points: " + playerCurrentUpgradePoints;
        }

        if (playerCurrentUpgradePoints <= 0)
        {
            playerUpgradePointstxt.gameObject.SetActive(false);
        }
    }

    public void SetOnScreenPlayerElements(bool x)
    {
        UIQuestText.gameObject.SetActive(x);
        playerCurrrentLeveltxt.gameObject.SetActive(x);
        playerXPCanvas.SetActive(x);
        playerXPBar.SetActive(x);
        playerCurrentMoneytxt.gameObject.SetActive(x);
        if (playerCurrentUpgradePoints > 0)
        {
            playerUpgradePointstxt.gameObject.SetActive(x);
        }
    }

    public void SpawnPickUpUiItem(ItemScritpable pickUpItem, int listLocation, int nums)
    {
        if (playerPickUpItemses[listLocation].amountOfItem > 0)
        {
            playerPickUpItemses[listLocation].amountOfItem += nums;
            playerPickUpItemses[listLocation].UpdateGameObject();
        }
        else
        {
            playerPickUpItemses[listLocation].amountOfItem += nums;
            GameObject beep = Instantiate(gbToSpawn, gbToParent.transform);
            beep.GetComponentInChildren<Image>().sprite =
                playerPickUpItemses[listLocation].possiblePickUpItem.pickUpItemSprite;
            beep.GetComponentInChildren<Text>().text = playerPickUpItemses[listLocation].amountOfItem.ToString();
            playerPickUpItemses[listLocation].uiGameObject = beep;
        }
    }

    IEnumerator PlayerLightFade(Light2D lightToFade, bool fadeIn, float duration)
    {
        float minLuminosity = 0; // min intensity
        float maxLuminosity = 1; // max intensity

        float counter = 0f;

        //set values dependign on if fadeIn or fadeOut
        float a, b;

        if (fadeIn)
        {
            a = minLuminosity;
            b = maxLuminosity;
        }
        else
        {
            a = maxLuminosity;
            b = minLuminosity;
        }

        float currentIntensity = lightToFade.intensity;

        while (counter < duration)
        {
            counter += Time.deltaTime;

            lightToFade.intensity = Mathf.Lerp(a, b, counter / duration);

            yield return null;
        }
    }

    public void CloseInventory()
    {
        if (isInventoryOpen)
        {
            currentMoveSpeed = startingMoveSpeed;
            playerInventoryCanvas.SetActive(false);
            isInventoryOpen = false;
        }
    }

    private bool runThroughBtnCreate = false;
    private void PlayerInventoryAbilitiesItemsBtnCreate()
    {
        Button btn = playerInventoryItems.GetComponentInChildren<Button>();
        if (runThroughBtnCreate == false)
        {
            foreach (PlayerAttacks item in playerAttacks)
            {
                GameObject newGo = Instantiate(btnToSpawn, playerInventoryAbilities.transform);
                newGo.GetComponentInChildren<Text>().text = item.item.attackName;
                newGo.GetComponent<ToolTipScript>().As = item.item;
                Toggle newGoTGL = newGo.GetComponentInChildren<Toggle>();
                newGoTGL.isOn = item.learned;
                newGo.GetComponentInChildren<Toggle>().onValueChanged.AddListener((value) => { PlayerAbilitiesLearnedUpdate(item, newGoTGL); });
            }
            btnToSpawn.SetActive(false);

            foreach (PlayerItems item in playerItems)
            {
                GameObject newGo = Instantiate(btn.gameObject, playerInventoryItems.transform);
                newGo.GetComponent<ToolTipScript>().Is = item.item;
                item.playerInventoryButton = newGo.GetComponent<Button>();
                item.updatebtnAmount();
                newGo.GetComponent<Button>().onClick.AddListener(() => {SelectItem(item.item, newGo.GetComponent<Button>()); });
            }
            btn.gameObject.SetActive(false);
            
            runThroughBtnCreate = true;
        }
    }

    //INDIVIDUAL INVENTORY METHOD CREATORS
    #region Inventory Creation
    public void PlayerInventoryAbilitiesCreate(AttacksScriptable newItem)
    {
        int p = 0;
        for (int i = 0; i < playerAttacks.Count; i++)
        {
            if (playerAttacks[i].item.attackName != newItem.attackName)
            {
                p++;
            }
        }

        if (p != playerAttacks.Count)
        {
            Debug.Log("Attack Already Known");
        }
        else
        {
            PlayerAttacks beep = new PlayerAttacks();
            beep.item = newItem;
            beep.learned = true;
            GameObject newGo = Instantiate(btnToSpawn, playerInventoryAbilities.transform);
            newGo.SetActive(true);
            newGo.GetComponentInChildren<Text>().text = beep.item.attackName;
            Toggle newGoTGL = newGo.GetComponentInChildren<Toggle>();
            newGoTGL.isOn = beep.learned;
            newGo.GetComponentInChildren<Toggle>().onValueChanged.AddListener((value) => { PlayerAbilitiesLearnedUpdate(beep, newGoTGL); });
            playerAttacks.Add(beep);
        }
    }

    #region Inventory Items
    private ItemScritpable selectedItem;
    private Button selectedItemButton;
    public void PlayerInventoryItemsCreate(ItemScritpable newItem)
    {
        int p = 0;
        for (int i = 0; i < playerItems.Count; i++)
        {
            if (playerItems[i].item.itemName != newItem.itemName)
            {
                p++;
            }
        }

        if (p != playerItems.Count)
        {
            Debug.Log("Already Have Item");
        }
        else
        {
            PlayerItems beep = new PlayerItems();
            beep.item = newItem;
            beep.itemAmount++;
            Button btn = playerInventoryItems.GetComponentInChildren<Button>();
            btn.GetComponent<ToolTipScript>().Is = newItem;
            GameObject newGo = Instantiate(btn.gameObject, playerInventoryItems.transform);
            newGo.SetActive(true);
            beep.playerInventoryButton = newGo.GetComponent<Button>();
            beep.updatebtnAmount();
            beep.playerInventoryButton.onClick.AddListener(() => { SelectItem(newItem, newGo.GetComponent<Button>()); });
            playerItems.Add(beep);
        }
    }

    private void SelectItem(ItemScritpable item, Button itemButton)
    {
        selectedItem = item;
        selectedItemButton = itemButton;
    }

    private void DoggoSelectCreate(Doggo doggo)
    {
        selectedItem.ItemDoggoUse(doggo, this);
    }
    #endregion
    
    public void PlayerInventoryDoggosCreate(Doggo doggo)
    {
        GameObject newGo = Instantiate(doggoBtnToSpawn, toParentButtons.transform);
        newGo.SetActive(true);
        newGo.GetComponentInChildren<Text>().text = doggo.doggoName;

        ToolTipScript toolTipScript = newGo.GetComponent<ToolTipScript>();
        toolTipScript.doggo = doggo;

        Toggle newGoTGL = newGo.GetComponentInChildren<Toggle>();
        newGoTGL.isOn = doggo.doggoInUse;
        newGo.GetComponentInChildren<Toggle>().onValueChanged.AddListener((value) => { PlayerDoggoInUse(doggo, newGoTGL); });
        
        //Creating inventory item doggo selection
        GameObject button = playerInventoryDoggoSelect.GetComponentInChildren<Button>().gameObject;
        GameObject newBtn = Instantiate(button, playerInventoryDoggoSelect.transform);
        newBtn.GetComponentInChildren<Text>().text = doggo.doggoName;
        newBtn.GetComponent<Button>().onClick.AddListener(() =>
        {
            DoggoSelectCreate(doggo);
        });
    }

    public void PlayerInventoryQuestsCreate(Quest quest)
    {
        Quest newQuest = quest;
        GameObject newGo = Instantiate(questBtnToSpawn, toParentQuestsbtn.transform);
        newGo.SetActive(true);
        newGo.GetComponentInChildren<Text>().text = newQuest.title;

        ToolTipScript toolTipScript = newGo.GetComponent<ToolTipScript>();
        toolTipScript.quest = newQuest;
        newQuest.questButton = newGo;
        
        

        //Code to make toggle active quest work
        newGo.gameObject.transform.GetChild(1).GetComponent<Toggle>().onValueChanged.AddListener((value) => {newQuest.onCHange(newQuest, 
            newGo.gameObject.transform.GetChild(1).GetComponent<Toggle>(), UIQuestText); });

        //Code to make abandon quest buttonWork
        newGo.gameObject.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => newQuest.AbandonQuest(quests, UIQuestText));
        
        //Code to finishQuestBtn
        newGo.GetComponent<Button>().onClick.AddListener(() => {newQuest.CompleteQuest(newQuest, quests, UIQuestText); });
        quests.Add(newQuest);
        quest.UpdateUIText(quests, UIQuestText);
    }
    #endregion
    
    
    private void PlayerAbilitiesLearnedUpdate(PlayerAttacks attack, Toggle tgl)
    {
        attack.learned = tgl.isOn;
    }

    private void PlayerDoggoInUse(Doggo doggo, Toggle tgl)
    {
        if (party.DoggosInUse() < party.partyMax)
        {
            doggo.doggoInUse = tgl.isOn;
            totalActiveDoggos.color = Color.black;
            int total = party.DoggosInUse();
            totalActiveDoggos.text = total + "/" + party.partyMax;

        }
        else
        {
            tgl.isOn = false;
            doggo.doggoInUse = false;
            if (party.DoggosInUse() < party.partyMax)
            {
                totalActiveDoggos.color = Color.black;
            }
            else
            {
                totalActiveDoggos.color = Color.red;
            }
            int total = party.DoggosInUse();
            totalActiveDoggos.text = total + "/" + party.partyMax;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("LightChanger"))
        {
            Debug.Log("LightChange");
            GameObject.FindObjectOfType<LightManager>().WorldLightFade();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("NPC") && Input.GetKey(KeyCode.F))
        {
            NPC NPC = collision.GetComponent<NPC>();
            Sprite NPCSprite = collision.GetComponent<SpriteRenderer>().sprite;
            FindObjectOfType<DialogueManager>().StartDialogue(heroName, NPC.NPCName, playerSprite, NPCSprite,
                NPC.dialogue, DialogueManager.WhoIsTalking.NPC);
        }

        if (collision.CompareTag("Encounter") && Input.GetKey(KeyCode.F))
        {
            EncounterScript encounter = collision.GetComponent<EncounterScript>();
            Sprite enconterSprite = collision.GetComponent<SpriteRenderer>().sprite;
            GameObject enemyAttackButton = FindObjectOfType<DialogueManager>().StartDialogue(heroName, encounter.encounterName, playerSprite,
                enconterSprite, encounter.dialogue, DialogueManager.WhoIsTalking.ENEMY);
            enemyAttackButton.GetComponent<Button>().onClick.AddListener(() => { battleManager.StartBattler(party, encounter); });
        }

        if (collision.CompareTag("QuestGiver") && Input.GetKey(KeyCode.F))
        {
            QuestGiver questGiver = collision.GetComponent<QuestGiver>();
            Sprite questGiverSprite = collision.GetComponent<SpriteRenderer>().sprite;
            Quest quest = quests.Find(x => x == questGiver.quest);
            Dialogue questCurrentDialogue = questGiver.DialogueSelection();
            if (questCurrentDialogue.whoIsTalking == DialogueManager.WhoIsTalking.QuestGiver)
            {
                GameObject acceptButton = FindObjectOfType<DialogueManager>().StartDialogue(heroName, questGiver.questGiverName, playerSprite,
                    questGiverSprite, questGiver.DialogueSelection(), DialogueManager.WhoIsTalking.QuestGiver);
                acceptButton.GetComponent<Button>().onClick.AddListener(() => { questGiver.AcceptQuest(); });
            }
            else if (questCurrentDialogue.whoIsTalking == DialogueManager.WhoIsTalking.QuestGiverDone)
            {
                GameObject acceptButton = FindObjectOfType<DialogueManager>().StartDialogue(heroName, questGiver.questGiverName, playerSprite,
                    questGiverSprite, questGiver.DialogueSelection(), DialogueManager.WhoIsTalking.QuestGiverDone);
                Quest newQuest = quests.Find(x => x == questGiver.quest);
                acceptButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    newQuest.CompleteQuest(newQuest, quests, UIQuestText);});
            }
            else
            {
                FindObjectOfType<DialogueManager>().StartDialogue(heroName, questGiver.questGiverName, playerSprite,
                    questGiverSprite, questGiver.DialogueSelection(), DialogueManager.WhoIsTalking.NPC);
            }
        }

        if (collision.CompareTag("DoggoFriend") && Input.GetKey(KeyCode.F))
        {
            DoggoScript currentDoggo = collision.GetComponent<DoggoScript>();
            Sprite doggoSprite = collision.GetComponent<SpriteRenderer>().sprite;
            GameObject acceptButton = FindObjectOfType<DialogueManager>().StartDialogue(heroName,
                currentDoggo.doggo.doggoName, playerSprite,
                doggoSprite, currentDoggo.dialogue, DialogueManager.WhoIsTalking.QuestGiver);
            acceptButton.GetComponent<Button>().onClick.AddListener(() => { currentDoggo.AddDoggo(); });
        }

        if (collision.CompareTag("Merchant" )&& Input.GetKey(KeyCode.F))
        {
            Merchant currentMerchant = collision.GetComponent<Merchant>();
            Sprite merchantSprite = collision.GetComponent<SpriteRenderer>().sprite;
            FindObjectOfType<DialogueManager>().StartDialogue(heroName, currentMerchant.merchantName, playerSprite,
                collision.GetComponent<SpriteRenderer>().sprite, currentMerchant.checkDialogue(),
                DialogueManager.WhoIsTalking.MERCHANT);
        }

        if (collision.CompareTag("PickUpItem") && Input.GetKey(KeyCode.F))
        {
            collision.GetComponent<PickUpItemScript>().pickUpItem.ItemPlayerActivate(FindObjectOfType<Player>());
            Destroy(collision.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.CompareTag("GenCol"))
        {
            FindObjectOfType<MapGENERATOR>().SpawnMap(other.gameObject.GetComponent<Collider2D>(), this.gameObject);
        }
    }

    /*private static List<T> FindAssetsByType<T>() where T : UnityEngine.Object
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
    }*/
}



[System.Serializable]
public class PlayerItems
{
    public int itemAmount;
    public ItemScritpable item;
    public Button playerItemBtn;
    public Button playerInventoryButton;

    public void updatebtnAmount()
    {
        playerInventoryButton.GetComponentInChildren<Text>().text = item.itemName + " " + itemAmount;
    }
}

[System.Serializable]
public class PlayerAttacks
{
    public bool learned;
    public AttacksScriptable item;
}
[System.Serializable]
public class PlayerPickUpItems
{
    public ItemScritpable possiblePickUpItem;
    public GameObject uiGameObject;
    public int amountOfItem;

    public void UpdateGameObject()
    {
        if (this.amountOfItem <= 0)
        {
            GameObject.Destroy(uiGameObject);
        }
        else
        {
            uiGameObject.GetComponentInChildren<Text>().text = amountOfItem.ToString();
        }
    }
}

[System.Serializable]
public class Party
{
    public Player player;
    public List<Doggo> doggos;
    public int partyMax = 2;

    public bool IsPartyDead()
    {
        int dead = 0;
        int doggosInUse = 0;
        if (player.isDead)
        {
            dead++;
        }
        foreach (Doggo item in doggos)
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
        Debug.Log("Party Members Dead:" + dead);
        if (dead >= doggosInUse + 1)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public void TakeDamage(int num, int damageToTake)
    {
        //Check if i is player
        if (num == -1)
        {
            player.currentHealth -= damageToTake;
            player.playerHealthBar.SetHealth(player.currentHealth);
            
            Debug.Log("Player Health: " + player.currentHealth);
            if (player.currentHealth <= 0)
            {
                player.isDead = true;
                OnDeathDestory(true, this, 1);
            }
        }
        else
        {
            //TakeDamage
            doggos[num].currentHealth -= damageToTake;
            if (doggos[num].currentHealth <= 0)
            {
                doggos[num].isDead = true;
                OnDeathDestory(false, this, num);
            }

            //Update health bar
            doggos[num].doggoHealthBar.SetHealth(doggos[num].currentHealth);
            Debug.Log("Doggo " + num + " Health: " + doggos[num].currentHealth);
        }
    }

    private void OnDeathDestory(bool isPlayer, Party currentParty, int i)
    {
        if (isPlayer)
        {
            GameObject.Destroy(currentParty.player.btnPlayerCharacter);
        }
        else
        {
            GameObject.Destroy(currentParty.doggos[i].btnDoggoCharacterButton);
        }
    }

    public int DoggosInUse()
    {
        int num = 0;
        for (int i = 0; i < doggos.Count; i++)
        {
            if (doggos[i].doggoInUse)
            {
                num++;
            }
        }

        return num;
    }
}
