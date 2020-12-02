using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class fader : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator transition;
    public float timeForTransition=1.5f;


    // Update is called once per frame
    void Update()
    {
        
            //LoadGameScene();
        
    }
    public void LoadGameScene()
    {
        StartCoroutine(LoadGame(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator LoadGame(int index)
    {
        transition.SetTrigger("BeginFadeTransition");
        yield return new WaitForSeconds(timeForTransition);
        SceneManager.LoadScene(index);
        //play animation
        //wait for animation to stop
        //load scene

    //fade out, load scene with canvas black
    }
}
