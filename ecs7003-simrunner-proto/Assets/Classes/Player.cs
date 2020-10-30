using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * A class to realize a player. This is not a Monobehavior!
 * 
 */
public class Player { 

    // Variables:
    private int health { get; set; }
    private float speed { get; set; }
    private Boolean hasDoubleJump { get; set; }

    // Constructor
    public Player(int health, float speed)
    {
        this.health = health;
        this.speed = speed;
    }

}
