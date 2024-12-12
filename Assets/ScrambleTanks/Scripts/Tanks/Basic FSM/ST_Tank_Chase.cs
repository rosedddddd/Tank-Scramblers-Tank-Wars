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
    }

    // Called when leaving the state
    public override Type LeaveState()
    {
        targetEnemy = null;
        return null;
    }
        
    // Runs every physics update inside the controller
    public override Type StateLogic()
    {
        // Get the dictionary of visible enemy tanks.
        var visibleEnemies = tank.VisibleEnemyTanks;
        //creating a variable for enemy distance to use in the if statements
        float enemyDist = Vector3.Distance(tank.transform.position, tank.enemyLastSeen.transform.position);
        
        //if the tank can see an enemy and its over 35 away or it can see a consumable
        if ((visibleEnemies.Count > 0 && enemyDist > 35f ) || tank.VisibleConsumables.Count > 0)
        {   

            //get closer to the enemy if its 35 away
            if (enemyDist > 35f)
            {
                tank.FollowPathToWorldPoint(visibleEnemies.Keys.First(), normalizedSpeed: 1.0f);//moving towards the visible enemy
                tank.TurretFaceWorldPoint(tank.enemyLastSeen.gameObject);//pointing turret at enemy
                Debug.Log("chasing enemy");
                return typeof(ST_Tank_Chase);//stay in chase

            }
            //if the tank sees a consumable go fast towards them to get consumable advantage then continue to chase enemy
            else if (tank.VisibleConsumables.Count > 0)
            {
                tank.FollowPathToWorldPoint(tank.VisibleConsumables.Keys.First(), normalizedSpeed: 1.0f);//moving towards the visible consumable prioritising
                return typeof(ST_Tank_Chase);
                

            }

        }
        else
        {
            //if none of the if statements aare true switch to attack it can deal with needing to go to search
            Debug.Log("close enough switching to attack");
            return typeof(ST_Tank_Attack);//when timer runs out tank attacks
                
        }
        return null;
        
    }
    
        
}


