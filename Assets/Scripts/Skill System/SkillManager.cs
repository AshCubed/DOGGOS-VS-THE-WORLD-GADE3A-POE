using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;
    
    public Image skillImage;
    public Text skillNameText;
    public Text skillDesText;
    public Text skillLevelReq;
    public Text skillPointReq;
    public Button skillUpgradeBtn;
    public GameObject skillDescPanel;
    public List<Skill> skillz;

    [SerializeField] private Skill activateSkill;
    private RawImage activeUpgradeBtnImage;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        for (int i = 0; i < skillz.Count; i++)
        {
            skillz[i].isUpgrade = false;
        }
    }
    
    public void UpgradeButton(RawImage rawImage)
    {
        activeUpgradeBtnImage = rawImage;
    }
    
    public void ShowSkillInfo(Skill skill)
    {
        skillDescPanel.SetActive(true);
        activateSkill = skill;
        skillImage.sprite = activateSkill.skillSprite;
        skillNameText.text = activateSkill.skillName;
        skillDesText.text = activateSkill.skillDes;
        skillLevelReq.text = "Required Level: " + activateSkill.requiredLevel.ToString();
        skillPointReq.text = "Required Upgrade Points: " +activateSkill.requiredUpgradeAmnt.ToString();
        skillUpgradeBtn.onClick.AddListener(() => skill.ActivateSkill(activeUpgradeBtnImage));
    }

    
}
