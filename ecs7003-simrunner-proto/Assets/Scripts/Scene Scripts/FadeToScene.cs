using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeToScene : MonoBehaviour
{
    public Camera camera;
    private int sceneIndex; 
    // Start is called before the first frame update
    public void startFadeOut(int index)
    {
        sceneIndex = index;
        Debug.Log("started");
        StartCoroutine("FadeOut");
    }
    IEnumerator FadeOut()
    {
        Debug.Log("fadeout started");
        Fade fade = camera.GetComponent<Fade>();
        float fadeTime = fade.FadeOut();
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene(sceneIndex);
    }
    // Update is called once per frame
    
}
