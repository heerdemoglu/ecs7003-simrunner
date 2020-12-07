using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeToScene : MonoBehaviour
{
    public Camera cam;
    private int sceneIndex;
    private int gameIndex =1;
    private Fade fade;
    // Start is called before the first frame update
    void Start()
    {
        fade = cam.GetComponent<Fade>();
        StartCoroutine("fadeIn");

    }

    IEnumerator fadeIn()
    {
        
        float fadeTime = fade.FadeIn();
        yield return new WaitForSeconds(fadeTime);

        if (SceneManager.GetActiveScene().buildIndex == gameIndex)
        {
            GameObject gc = GameObject.FindGameObjectWithTag("GameController");
            GameController gamec = gc.GetComponent<GameController>();
            gamec.startGame();
            //gamec.StartTime();
        }
    }

    public void startFadeOut(int index)
    {
        sceneIndex = index;
        StartCoroutine("FadeOut");
    }
    IEnumerator FadeOut()
    {
        float fadeTime = fade.FadeOut();
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene(sceneIndex);
    }
    
}
