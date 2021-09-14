using System;
using System.Collections.Generic;
using UnityEngine;

public class Situation : MonoBehaviour
{
    private int priority;
    public Action<TileObject> OnEntityLeave;
    public Action<TileObject> OnEntityJoin;
    private List<TileObject> entities;
    private HashSet<Action<TileObject>> interactions;

    public static void DontCare(TileObject recipient, Action<TileObject> interaction)
    {
        // interaction -=   
    }

    public void NotifyEntityAction(TileObject sender)
    {
        foreach (TileObject tileObject in entities)
        {
            
        }
        /*
         * Hash each interaction with unique id?
         * Statically handle every action an entity can do?
         */
    }
    
    void NotifyEntityLeftSituation(TileObject leaver)
    {
        OnEntityLeave(leaver);
    }

    void NotifyEntityJoinedSituation(TileObject joiner)
    {
        OnEntityJoin(joiner);
    }

    void JoinSituation(TileObject joiner)
    {
        // Add own interactions
        /*
        ...
        */
        
        // Subscribe to interactions
        /*
        // Is it enough if they're Action<TileObject> ?
        ...
        */
        
        NotifyEntityJoinedSituation(joiner);
        
    }

    void LeaveSituation(TileObject leaver)
    {
        // Unsubscribe from interactions
        /*
        ...
        */
        
        // Remove interactions
        /*
        ...
        */

        NotifyEntityLeftSituation(leaver);
        
    }
}
