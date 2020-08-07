using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class BattleTransitionScript : MonoBehaviour
{
    private BattleManagerV2 battleManagerV2;
    private Camera mainCam;
    private Camera battleCam;
    public Light2D globalLight;
    
    // Start is called before the first frame update
    void Start()
    {
        battleManagerV2 = FindObjectOfType<BattleManagerV2>();
    }

    public void Battler()
    {
        battleManagerV2.ExecuteBattler();
    }

    public void GlobalLightOn()
    {
        globalLight.gameObject.SetActive(true);
    }

    public void GlobalLightOff()
    {
        globalLight.gameObject.SetActive(false);
    }
    
}
