using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Merchant : MonoBehaviour
{
    public string merchantName;
    public Dialogue dialogueIntro;
    public Dialogue dialogueOther;
    public bool hasTalkedTo;
    [HideInInspector] public int currentMoneyTotal;
    public List<MerchantItems> ItemsForSale;
    public GameObject button;
    public GameObject parentButtonsTo;
    private Player player;
    public TextMeshProUGUI txtPlayerMoney;
    public GameObject fImage;
    public GameObject nameText;
    
    private RectTransform fImageRect;
    private RectTransform nameTextRect;
    private float fImageScaleX;
    private float newImageScaleX;

    // Start is called before the first frame update
    void Start()
    {
        hasTalkedTo = false;
        player = FindObjectOfType<Player>();
        dialogueIntro.name = merchantName;
        dialogueOther.name = merchantName;
        txtPlayerMoney.text = "Money: " + player.currentMoneyAmnt.ToString();
        CreateButtons();
        nameText.GetComponent<Text>().text = string.Format("<color=#22A600>" + merchantName + "</color>");
        button.SetActive(false);
        
        fImage.SetActive(false);
        nameText.gameObject.SetActive(true);
        
        fImageRect = fImage.GetComponent<RectTransform>();
        nameTextRect = nameText.GetComponent<RectTransform>();

        fImageScaleX = fImageRect.localScale.x;
        newImageScaleX = nameTextRect.localScale.x;
    }

    private void Update()
    {
        txtPlayerMoney.text = "Money: " + player.currentMoneyAmnt.ToString();
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

    public Dialogue checkDialogue()
    {
        if (hasTalkedTo)
        {
            return dialogueOther;
        }
        else
        {
            hasTalkedTo = true;
            return dialogueIntro;
        }
    }

    private void CreateButtons()
    {
        foreach (MerchantItems item in ItemsForSale)
        {
            GameObject newGo = Instantiate(button, parentButtonsTo.transform);
            newGo.SetActive(true);
            newGo.GetComponent<Button>().onClick.AddListener(() => Buy(item));
            newGo.GetComponent<Button>().GetComponentInChildren<Text>().text = item.item.itemName;
            newGo.GetComponent<ToolTipScript>().Is = item.item;
            newGo.GetComponent<ToolTipScript>().useExtraDscrp = true;
            newGo.GetComponent<ToolTipScript>().extraDescription = "Price: " + item.price;
        }
    }

    private void Buy(MerchantItems merchantItem)
    {
        //checks if the player has enough money
        if (player.currentMoneyAmnt >= merchantItem.price)
        {
            int num = 0;
            //Run through the players item inventory to find the item they want to buy
            for (int i = 0; i < player.playerItems.Count; i++)
            {
                //Adds to player item inventory, decrements money
                if (player.playerItems[i].item.itemName == merchantItem.item.itemName)
                {
                    player.playerItems[i].itemAmount++;
                    player.currentMoneyAmnt -= merchantItem.price;
                    player.playerItems[i].updatebtnAmount();
                    player.MoneyTextUpdate(player.currentMoneyAmnt, 1);
                    txtPlayerMoney.text = player.currentMoneyAmnt.ToString();
                    break;
                }

                num++;
            }

            if (num == player.playerItems.Count)
            {
                FindObjectOfType<NotificationManager>().StartNotification("You haven't unlocked that item yet");
            }
        }
        else
        {
            FindObjectOfType<NotificationManager>().StartNotification("Merchant: You don't have enough money!");
            StartCoroutine(ChangeMoneyTextToRed(txtPlayerMoney, Color.red));
        }
    }

    IEnumerator ChangeMoneyTextToRed(TextMeshProUGUI currentMoney, Color desieredColor)
    {
        Color pastColor;
        pastColor = currentMoney.color;
        currentMoney.color = desieredColor;
        yield return new WaitForSeconds(1f);
        currentMoney.color = pastColor;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            fImage.SetActive(true);
            nameText.gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            nameText.gameObject.SetActive(true);
            fImage.SetActive(false);
        }
    }
}

[System.Serializable]
public class MerchantItems
{
    public ItemScritpable item;
    public int price;
}
