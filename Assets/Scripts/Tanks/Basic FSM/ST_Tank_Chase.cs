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
        return null;
        Debug.Log("in chase mode");
    }

    // Called when leaving the state
    public override Type LeaveState()
    {
        targetEnemy = null;
        return null;
        Debug.Log("exit chase mode");
    }

    
// Runs every physics update inside the controller
public override Type StateLogic()
{
    // Get the dictionary of visible enemy tanks.
    var visibleEnemies = tank.VisibleEnemyTanks;

    if (visibleEnemies.Count > 0)
    {
        // Move towards the enemy.
        tank.FollowPathToWorldPoint(visibleEnemies.Keys.First(), normalizedSpeed: 1.0f);

        
        return typeof(ST_Tank_Chase);
    }
<<<<<<< Updated upstream
    else
    {
=======
    
    if (timer > 0){
    timer -= Time.deltaTime;
        tank.FollowPathToWorldPoint(tank.enemyLastSeen.gameObject, normalizedSpeed: 1.0f);
        return null;
>>>>>>> Stashed changes
        
    }

    if (timer <= 0)
        {
            return typeof(ST_Tank_Attack);
        }
        
    tank.GenerateNewRandomWorldPoint();

        
    return typeof(ST_Tank_Search); 
    
}


}
