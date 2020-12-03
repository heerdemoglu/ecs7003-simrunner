using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
    // Start is called before the first frame update
    public float fadeTime = 1.5f;
    public Texture2D fadeTexture;
    private int textureDepth = -1000;

    private float start, end, time;

    public static int TRANSPARENT = 0;
    public static int OPAQUE = 1;

    private void OnGUI()
    {
        //Debug.Log("onGUI");
        time += Time.deltaTime;
        float alpha = Mathf.Lerp(start, end, time / fadeTime);//gives fraction between start and end
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);

        GUI.depth = textureDepth;
        Rect dimension = new Rect(0, 0, Screen.width, Screen.height);

        GUI.DrawTexture(dimension, fadeTexture);

    }

    // Update is called once per frame
    public float StartFade(float s, float e)
    {
        start = s;
        end = e;
        time = 0f;
        return fadeTime;

    }
    public float FadeOut()
    {
        return StartFade(TRANSPARENT, OPAQUE);
    }
    public float FadeIn()
    {
        return StartFade( OPAQUE, TRANSPARENT);
    }
}
