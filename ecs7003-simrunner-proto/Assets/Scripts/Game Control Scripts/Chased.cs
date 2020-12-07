using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;
public class Chased : MonoBehaviour
{
    public GameObject gameLight;
    private Light glight;
    private float originalIntensity;
    //zone light
    public GameObject zoneLight;

    public float highestIntensity = 4f;

    private Light zlight;


    private GameController gameController;
    private GameObject zone;

    public float distanceToZone;
    private bool inRange;

    //tier ranges
    private float farTier;//max range of the zone
    private float midTier;
    private float nearTier;
    public float deathTier;
    public float tierRange;
    //private float multiplier;

    // Start is called before the first frame update
    void Start()
    {

        zlight = zoneLight.GetComponent<Light>();
        glight = gameLight.GetComponent<Light>();
        zlight.intensity = 0f;
        originalIntensity = glight.intensity;

        GameObject gameC = GameObject.FindGameObjectWithTag("GameController");
        zone = GameObject.FindGameObjectWithTag("Zone");
        gameController = gameC.GetComponent<GameController>();

        nearTier = deathTier + tierRange;
        midTier = nearTier + tierRange;
        farTier = midTier + tierRange;
    }
    void Update()
    {

        float zoneZ = zone.transform.position.z;
        distanceToZone = Mathf.Abs(transform.position.z - zoneZ);
        
        if (distanceToZone <= farTier)
        { 
            inRange = true;
            zlight.intensity = Mathf.Lerp(0, highestIntensity, 1f - (distanceToZone / farTier));
            glight.intensity = Mathf.Lerp(originalIntensity, 0f, 1f - (distanceToZone / farTier));

            if(transform.position.y < -0.05f)
            {
                zone.GetComponent<Zone>().pauseChase();
                gameController.gameOver();
            }

            if (distanceToZone < deathTier)
            {

                CameraShaker.Instance.ShakeOnce(7f, 8f , 8f, 5f);
                zone.GetComponent<Zone>().pauseChase();
                gameController.gameOver();
            }
            else if (distanceToZone < nearTier)
            {
                CameraShaker.Instance.ShakeOnce(3f, 2f, 3f, 1f);
            }
            else if (distanceToZone < midTier)
            {
                CameraShaker.Instance.ShakeOnce(3f, 2f, 3f, 1f);
            }
            else
            {
                CameraShaker.Instance.ShakeOnce(2f, 1f, 2f, 1f);
            }
        }
        else
        {
            inRange = false;
            zlight.intensity = Mathf.Lerp(zlight.intensity,0f,1); ;
            glight.intensity = Mathf.Lerp(glight.intensity, originalIntensity, 1);
        }
    }


    
}
