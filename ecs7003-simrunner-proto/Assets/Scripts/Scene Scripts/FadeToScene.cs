using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeToScene : MonoBehaviour
{
    public Camera cam;
    private int sceneIndex;
    private int gameIndex =1;
    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine("fadeIn");

        //startTime = Time.time;
        Fade fade = cam.GetComponent<Fade>();
        float fadeTime = fade.FadeIn();
    }

    //IEnumerator fadeIn()
    //{
    //    Fade fade = cam.GetComponent<Fade>();
    //    float fadeTime = fade.FadeIn();
    //    yield return new WaitForSeconds(fadeTime);
        
    //    if (SceneManager.GetActiveScene().buildIndex == gameIndex)
    //    {   GameObject gc = GameObject.FindGameObjectWithTag("GameController");
    //        GameController gamec = gc.GetComponent<GameController>();
            
    //        gamec.StartTime();
    //    }
    //}

    public void startFadeOut(int index)
    {
        sceneIndex = index;
        Debug.Log("started");
        StartCoroutine("FadeOut");
    }
    IEnumerator FadeOut()
    {
        Debug.Log("fadeout started");
        Fade fade = cam.GetComponent<Fade>();
        
        float fadeTime = fade.FadeOut();
        Debug.Log("called fadeout" + fadeTime);
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene(sceneIndex);
        Debug.Log("sceneloaded");
    }
    // Update is called once per frame
    
}
