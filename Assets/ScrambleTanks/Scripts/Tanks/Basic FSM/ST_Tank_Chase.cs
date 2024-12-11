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
        float enemyDist = Vector3.Distance(tank.transform.position, tank.enemyLastSeen.transform.position);//looking at distance before switching to attack
        

        if (visibleEnemies.Count > 0 && enemyDist > 35 || tank.VisibleConsumables.Count > 0)
        {
            if (enemyDist < 35)
            {
                tank.FollowPathToWorldPoint(visibleEnemies.Keys.First(), normalizedSpeed: 1.0f);//moving towards the visible enemy
                tank.TurretFaceWorldPoint(tank.enemyLastSeen.gameObject);//pointing turret at enemy
                Debug.Log("chasing enemy");
                return typeof(ST_Tank_Chase);//stay in chase

            }
            else if (tank.VisibleConsumables.Count > 0)
            {
                tank.FollowPathToWorldPoint(tank.VisibleConsumables.Keys.First(), normalizedSpeed: 1.0f);//moving towards the visible consumable prioritising
                return typeof(ST_Tank_Chase);
                

            }

        }
        else
        {
            Debug.Log("close enough switching to attack");
            return typeof(ST_Tank_Attack);//when timer runs out tank attacks
                
        }
        return null;
        
    }
    
        
}


