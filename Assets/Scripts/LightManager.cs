using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class LightManager : MonoBehaviour
{
    private GameObject[] worldOutsideLights;
    private GameObject[] worldInsideLights;
    private bool isOutside = true;

    private void Start()
    {
        worldOutsideLights = GameObject.FindGameObjectsWithTag("OutsideLight");
        worldInsideLights = GameObject.FindGameObjectsWithTag("InsideLight");
        WorldLightFade();
    }

    public void WorldLightFade()
    {
        for (int i = 0; i < worldOutsideLights.Length; i++)
        {
            worldOutsideLights[i].SetActive(isOutside);
        }
        for (int i = 0; i < worldInsideLights.Length; i++)
        {
            worldInsideLights[i].SetActive(!isOutside);
        }

        isOutside = !isOutside;
    }

    IEnumerator WorldLightsFade(float duration)
    {
        float minLuminosity = 0; // min intensity
        float maxLuminosity = 1; // max intensity

        float counter = 0f;

        //set values dependign on if fadeIn or fadeOut
        float a, b;

        if (isOutside)
        {
            a = minLuminosity;
            b = maxLuminosity;
        }
        else
        {
            a = maxLuminosity;
            b = minLuminosity;
        }

        //float currentIntensity = lightToFade.intensity;
        for (int i = 0; i < worldOutsideLights.Length; i++)
        {
            while (counter < duration)
            {
                counter += Time.deltaTime;

                worldOutsideLights[i].GetComponent<Light2D>().intensity = Mathf.Lerp(a, b, counter / duration);

                yield return null;
            }
        }

        for (int i = 0; i < worldInsideLights.Length; i++)
        {
            while (counter < duration)
            {
                counter += Time.deltaTime;

                worldInsideLights[i].GetComponent<Light2D>().intensity = Mathf.Lerp(b, a, counter / duration);

                yield return null;
            }
        }

    }

    private void OutsideLightsControl(float duration , float a, float b)
    {
        float counter = 0f;

        for (int i = 0; i < worldOutsideLights.Length; i++)
        {
            while (counter < duration)
            {
                counter += Time.deltaTime;

                worldOutsideLights[i].GetComponent<Light2D>().intensity = Mathf.Lerp(a, b, counter / duration);
            }
        }
    }

    private void InsideLightControl(float duration, float a, float b)
    {
        float counter = 0f;

        for (int i = 0; i < worldInsideLights.Length; i++)
        {
            while (counter < duration)
            {
                counter += Time.deltaTime;

                worldInsideLights[i].GetComponent<Light2D>().intensity = Mathf.Lerp(a, b, counter / duration);
            }
        }
    }
}
