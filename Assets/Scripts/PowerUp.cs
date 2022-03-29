using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Powerup types
public enum PowerUpType { None, Pushback, Rockets, Smash}

public class PowerUp : MonoBehaviour
{
    // For the inspector window
    public PowerUpType powerUpType;
}
