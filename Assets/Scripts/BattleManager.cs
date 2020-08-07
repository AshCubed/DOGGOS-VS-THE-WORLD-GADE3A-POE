using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
    public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }
    [Header("Current Game State")]
    public BattleState state;

    [Header("Cameras")]
    public GameObject mainCamera;
    public GameObject battleCamera;

    [Header("Hero Information")]
    public Button btnHero;
    public HealthBar sldHero;
    public GameObject btnToCreate;
    public GameObject toParentAttackButtonsToo;
    public GameObject toParentItemButtonsToo;
    public Text txtHeroName;
    public Transform heroSpawnPoint;
    public List<Button> btnBackButtons;
    private Vector3 heroOGMapPoint;
    private GameObject currentHero;
    private List<GameObject> btnHeroButtons;

    [Header("Party Information")]
    public HealthBar sldParty;
    public GameObject btnPartyToCreate;
    public GameObject parentPartyAttackButtonsToo;
    public Transform partySpawnPoint;
    public List<Button> btnPartyBackButtons;
    private List<GameObject> btnDoggoPartyButtons;
    private GameObject currentPartyMember;

    [Header("Enemy Information")]
    public TextMeshProUGUI txtEnemyName;
    public HealthBar sldEnemy;
    public Transform enemySpawnPoint;
    private Vector3 enemyOGMapPoint;
    private GameObject currrenEnemy;
   

    private void Start()
    {
        btnHeroButtons = new List<GameObject>();
    }

    public void BattleStateCheck()
    {
        switch (state)
        {
            case BattleState.START:
                break;

            case BattleState.PLAYERTURN:
                if (btnHero.interactable != true)
                {
                    btnHero.interactable = true;
                }
                break;

            case BattleState.ENEMYTURN:
                PressMenuBackButtons();
                if (currrenEnemy.GetComponent<Enemy>().currentHealth <= 0)
                {
                    state = BattleState.WON;
                    BattleStateCheck();
                }
                if (currrenEnemy != null)
                {
                    StartCoroutine(EnemyTurn(currentHero.GetComponent<Player>(), currrenEnemy.GetComponent<Enemy>()));
                }
                break;

            case BattleState.WON:
                StartCoroutine(WonBattle(currentHero, currrenEnemy));
                state = BattleState.START;
                break;

            case BattleState.LOST:
                break;

            default:
                break;
        }
    }

    public void StartBattler(GameObject playerObject, GameObject enemyObject, string heroName, string enemyName)
    {
        currentHero = playerObject;
        currrenEnemy = enemyObject;

        StartCoroutine(StartBattle(playerObject, enemyObject, heroName, enemyName));
    }

    IEnumerator StartBattle(GameObject playerObject, GameObject enemyObject, string heroName, string enemyName)
    {
        mainCamera.SetActive(false);

        heroOGMapPoint = playerObject.transform.position;
        enemyOGMapPoint = enemyObject.transform.position;

        yield return new WaitForSeconds(1f);

        playerObject.GetComponent<Player>().currentMoveSpeed = 0;
        playerObject.transform.position = heroSpawnPoint.position;
        enemyObject.transform.position = enemySpawnPoint.position;

        txtHeroName.text = heroName;
        txtEnemyName.text = enemyName;

        PlayerAttackButtonMethods(playerObject, enemyObject);
        PlayerItemButtonMethods(playerObject, enemyObject);

        //Set Max Health for all Characters
        sldHero.SetMaxHealth(playerObject.GetComponent<Player>().maxHealth);
        sldEnemy.SetMaxHealth(enemyObject.GetComponent<Enemy>().maxHealth);

        yield return new WaitForSeconds(1f);
        state = BattleState.PLAYERTURN;
        battleCamera.SetActive(true);
        BattleStateCheck();
    }

    bool runAttackThrough = false;
    private void PlayerAttackButtonMethods(GameObject playerObject, GameObject enemyObject)
    {
        Player playerScript = playerObject.GetComponent<Player>();

        if (!runAttackThrough)
        {
            foreach (PlayerAttacks item in playerScript.playerAttacks)
            {
                if (item.learned)
                {
                    GameObject newGo = Instantiate(btnToCreate, toParentAttackButtonsToo.transform);
                    newGo.GetComponentInChildren<Text>().text = item.item.attackName;
                    //newGo.GetComponent<Button>().onClick.AddListener(() => { item.item.attackTarget(enemyObject, sldEnemy); });
                    newGo.GetComponent<ToolTipScript>().As = item.item;
                    btnHeroButtons.Add(newGo);
                }
            }
        }
        runAttackThrough = true;
    }


    bool runItemsThrough = false;
    private void PlayerItemButtonMethods(GameObject playerObject, GameObject enemyObject)
    {
        Player playerScript = playerObject.GetComponent<Player>();

        if (!runItemsThrough)
        {
            foreach (PlayerItems item in playerScript.playerItems)
            {
                GameObject newGo = Instantiate(btnToCreate, toParentItemButtonsToo.transform);
                newGo.GetComponentInChildren<Text>().text = item.item.itemName;
                //newGo.GetComponent<Button>().onClick.AddListener(() => { item.item.ItemPlayerActivate(playerObject.GetComponent<Player>()); });
                newGo.GetComponent<ToolTipScript>().Is = item.item;
                btnHeroButtons.Add(newGo);
            }
        }
        runItemsThrough = true;
    }

    IEnumerator EnemyTurn(Player player, Enemy enemy)
    {
        btnHero.interactable = false;
        //enemy.EnemyAttack(player, sldHero);
        yield return new WaitForSeconds(2f);
        state = BattleState.PLAYERTURN;
        BattleStateCheck();
    }

    IEnumerator WonBattle(GameObject playerObject, GameObject enemyObject)
    {
        battleCamera.SetActive(false);
        playerObject.transform.position = heroOGMapPoint;
        Debug.Log("mooved");
        enemyObject.transform.position = enemyOGMapPoint;
        Destroy(enemyObject.gameObject);   //change in future to work with text blurb after death (dialogue system)
        yield return new WaitForSeconds(1f);
        playerObject.GetComponent<Player>().currentMoveSpeed = playerObject.GetComponent<Player>().startingMoveSpeed;
        mainCamera.SetActive(true);

        runAttackThrough = false;

        PressMenuBackButtons();
        DestroyAttackAndItemButtons();
    }

    private void PressMenuBackButtons()
    {
        for (int i = 0; i < btnBackButtons.Count; i++)
        {
            if (btnBackButtons[i].gameObject.activeSelf == true)
            {
                btnBackButtons[i].onClick.Invoke();
            }
        }
    }

    private void DestroyAttackAndItemButtons()
    {
        foreach (GameObject item in btnHeroButtons)
        {
            Destroy(item);
        }

        btnHeroButtons = new List<GameObject>();
    }
}
