using System;
using System.Linq;
using UnityEngine;

public class ST_Tank_Chase : ST_BaseTankState
{
    private Transform targetEnemy; // The current target enemy

    // Called when entering the state
    public override Type EnterState()
    {
        targetEnemy = null;
        Debug.Log("in chase mode");
        return null;
        
    }

    // Called when leaving the state
    public override Type LeaveState()
    {
        targetEnemy = null;
        Debug.Log("exit chase mode");
        return null;
        
    }

    
// Runs every physics update inside the controller
float timer;

public override Type StateLogic()
{
    // Get the dictionary of visible enemy tanks.
    var visibleEnemies = tank.VisibleEnemyTanks;

    if (visibleEnemies.Count > 0)
    {
        timer = 5;
        // Move towards the enemy.
        tank.enemyLastSeen.position = tank.VisibleEnemyTanks.Keys.First().transform.position;
        tank.FollowPathToWorldPoint(tank.enemyLastSeen.gameObject, normalizedSpeed: 1.0f);
        

        
        return typeof(ST_Tank_Chase);
    }
    else if (timer > 0){
        timer -= Time.deltaTime;
            tank.FollowPathToWorldPoint(tank.enemyLastSeen.gameObject, normalizedSpeed: 1.0f);
            return null;
        }
    else
    {
        
        tank.GenerateNewRandomWorldPoint();

        
        return typeof(ST_Tank_Search); 
    }
}


}
