using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chased : MonoBehaviour
{
    public GameObject gameLight;
    private Light glight;
    private float originalIntensity;
    //zone light
    public GameObject zoneLight;
    //private Color originalColor;
    //public Color zoneLightColor;
    public float highestIntensity = 4f;
    private float lowestIntensity = 0f;

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

            if (distanceToZone < deathTier)
            {
                Debug.Log("made contact");
                zone.GetComponent<Zone>().pauseChase();
                gameController.gameOver();
            }
            else if (distanceToZone < nearTier)
            {
                Debug.Log("Within nearTier");
            }
            else if (distanceToZone < midTier)
            {
                Debug.Log("Within midTier");
            }
            else
            {
                Debug.Log("Within farTier");
            }
        }
        else
        {
            inRange = false;
            zlight.intensity = Mathf.Lerp(zlight.intensity,0f,1); ;
            glight.intensity = Mathf.Lerp(glight.intensity, originalIntensity, 1);
        }

        Debug.DrawLine(transform.position, zone.transform.position, Color.red);
    }

    private void OnDrawGizmos()
    {
        GameObject zon = GameObject.FindGameObjectWithTag("Zone");
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, zon.transform.position);
    }
    // Update is called once per frame
    //void FixedUpdate()
    //{
    //    int layerMask = 9;//9 is zone layer
    //    RaycastHit hit;
    //    if (Physics.Raycast(transform.position, Vector3.back, out hit, Mathf.Infinity, layerMask))
    //    {
    //        Debug.DrawRay(transform.position, Vector3.back, Color.red);

    //        distanceToZone = transform.position.z - chasingZone.transform.position.z;

    //        if (distanceToZone < deathTier)
    //        {
    //            Debug.Log("made contact");

    //            gameController.gameOver();
    //        }
    //        else if (distanceToZone < nearTier)
    //        {
    //            Debug.Log("Within nearTier");
    //        }
    //        else if (distanceToZone < midTier)
    //        {
    //            Debug.Log("Within midTier");
    //        }
    //        else if (distanceToZone < farTier)
    //        {
    //            Debug.Log("Within farTier");
    //        }


    //    }
    //}
}
