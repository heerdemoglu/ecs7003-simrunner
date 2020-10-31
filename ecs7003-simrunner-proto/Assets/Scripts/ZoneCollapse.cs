using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* uses some of Transforms lecture as prototype zone collapse requires it.
Final version will have dynamic collapse; it will always follow the user; if the user cannot 
progress over a predefined distance it will get closer to the player.  Zone will be a semi-transparent 
cube; to see the render of both sides.

If user is able to run faster; it will get further away up to a maximum distance, from that it will 
cruise according to the player.

These are to be implemented in the final game. */
public class ZoneCollapse : MonoBehaviour
{

    private Vector3 from;
    private float elapsed;

    public Transform target;
    public float duration;

    // Start is called before the first frame update
    void Start()
    {
        from = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float frac = elapsed / duration;
        transform.position = Vector3.Lerp(from, target.position, frac);
        elapsed += Time.deltaTime;
    }
}
