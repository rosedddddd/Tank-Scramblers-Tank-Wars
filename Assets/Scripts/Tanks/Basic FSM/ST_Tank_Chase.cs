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
    }

    // Runs every physics update inside the controller
public override Type StateLogic()
{
    if (tank.VisibleEnemyTanks.Count == 0)
    {
        // No enemies visible, switch to search state
        return typeof(ST_Tank_Search);
    }

    // Identify the closest enemy if no target is locked
    if (targetEnemy == null || !tank.VisibleEnemyTanks.ContainsKey(targetEnemy.gameObject))
    {
        GameObject closestEnemyObject = tank.VisibleEnemyTanks
            .OrderBy(enemy => Vector3.Distance(transform.position, enemy.Key.transform.position))
            .First().Key;

        targetEnemy = closestEnemyObject.transform; // Set the target enemy's Transform
        Debug.Log("finding enemy");
    }

    if (targetEnemy != null)
        {
            tank.FollowPathToWorldPoint(targetEnemy.gameObject, 1, tank.heuristicMode);

            // Rotate the turret to face the target enemy
            tank.TurretFaceWorldPoint(targetEnemy.gameObject);
            Debug.Log("moving turret");
        }

    
    return null;
}


}
