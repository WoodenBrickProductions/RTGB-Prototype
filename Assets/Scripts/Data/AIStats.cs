using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AIStats
{
    [Header("AI stats")]
    public float wanderCooldown = 1;
    public int agroRange = 1;
    public int talkRange = 1;
    public int chasingRange = 100;
    public float chasingCooldown = 0.25f;
    public bool usePseudoRandom = false;

    [Header("Debuging")]
    public float wanderTime = 0;
    public float chasingTime = 0;
    public bool chasing = false;

    public AIStats(AIStats aiStats)
    {
        wanderCooldown = aiStats.wanderCooldown;
        agroRange = aiStats.agroRange;
        chasingRange = aiStats.chasingRange;
        chasingCooldown = aiStats.chasingCooldown;
        usePseudoRandom = aiStats.usePseudoRandom;
        wanderTime = aiStats.wanderTime;
        chasingTime = aiStats.chasingTime;
        chasing = aiStats.chasing;
    }
}
