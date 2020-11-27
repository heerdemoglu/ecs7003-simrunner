using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSelectedScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }
    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
