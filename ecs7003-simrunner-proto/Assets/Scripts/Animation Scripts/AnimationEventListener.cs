using System.Collections;
using UnityEngine;

/**
* A listener component that handles animation events
*/
public class AnimationEventListener : MonoBehaviour {

    //Sets the locked-state in the character controller
    public void SetMovementLocked(int interruptFlag)//as boolean is not recognised in editor
    {
        GetComponent<PlayerController>().SetMovementLocked(interruptFlag != 0);
    }
}

