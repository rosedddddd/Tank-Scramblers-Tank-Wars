using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ST_Tank_Attack : ST_BaseTankState
{
    public float t;
    public bool circlingClockwise;

    // setting up to enter the state
    public override Type EnterState()
    {
        return null;
    }

    // anything that needs to be reset or changed once the player leaves the stage
    public override Type LeaveState()
    {
        return null;
    }

    // logic that runs every physics update inside of the controller
    public override Type StateLogic()
    {
        if (tank.VisibleEnemyTanks.Count > 0) //when tanks in sight
        {
            float dist = Vector3.Distance(tank.transform.position, tank.VisibleEnemyTanks.Keys.First().transform.position);
            Vector3 normalized = (tank.transform.position - tank.VisibleEnemyTanks.Keys.First().transform.position).normalized;
            float circlingAngle = 90f;
            float circlingAngleBeforeReadyingUpToShoot= 40f;


            //use rays to check if there's an obstacle
            RaycastHit hit;
            if (
                Physics.Raycast(
                    new Ray(tank.transform.position, tank.calcTransform.position - tank.transform.position),
                    out hit,
                    Vector3.Distance(tank.transform.position, tank.calcTransform.position),
                    tank.raycastLayers
                    )
                )
            {
                circlingClockwise = !circlingClockwise;
                Debug.DrawLine(tank.transform.position, hit.point, Color.red, 10);
            }

            circlingAngle *= circlingClockwise ? -1 : 1;
            circlingAngleBeforeReadyingUpToShoot *= circlingClockwise ? -1 : 1;

            if (t > 5) //at 5 seconds round out
            {
                normalized = Quaternion.AngleAxis(circlingAngleBeforeReadyingUpToShoot, Vector3.up) * normalized;
                tank.calcTransform.position = tank.VisibleEnemyTanks.Keys.First().transform.position + normalized * 20;
            }
            else // any other time close in
            {
                normalized = Quaternion.AngleAxis(circlingAngle, Vector3.up) * normalized;
                tank.calcTransform.position = tank.VisibleEnemyTanks.Keys.First().transform.position + normalized * 5;
            }

            if (t > 7) //when at 7 secs restart t and attack
            {
                t = 0;
                tank.TurretFaceWorldPoint(tank.enemyLastSeen.gameObject);
                tank.TurretFireAtPoint(tank.VisibleEnemyTanks.Keys.First());
            }
            else if (dist < 25F) { t += Time.deltaTime; } //keep counting time if within distance
            tank.FollowPathToWorldPoint(tank.calcTransform.gameObject, 0.5f, tank.heuristicMode);
            tank.TurretFaceWorldPoint(tank.enemyLastSeen.gameObject);
        }

        //if low health/ammo/fuel, retreat
        if (tank.lowHealth || tank.lowAmmo || tank.lowFuel) return typeof(ST_Tank_Retreat);

        //if enemy tank out of FOV, switch to search state
        if (tank.VisibleEnemyTanks.Count <= 0 && tank.VisibleEnemyBases.Count <= 0)
        {
            return typeof(ST_Tank_Search);
        }

        //if enemy tank out of firing range, switch to chase state
        if (tank.VisibleEnemyTanks.Count >= 1)
        {
            float enemmyDist = Vector3.Distance(tank.transform.position, tank.enemyLastSeen.transform.position);
            
            if (enemmyDist > 35f) {return typeof(ST_Tank_Chase); } //if too far but still in sight away switch to chase
            
            else { return typeof(ST_Tank_Attack); } //else keep attacking
        }

        //if there are no tanks in FOV and at least 1 enemy base in FOV
        if (tank.VisibleEnemyBases.Count > 0 && tank.VisibleEnemyTanks.Count <= 0)
        {
            float closestDistance = 999f;
            GameObject closestBase = null;

            //chose closest base
            foreach (GameObject item in tank.VisibleEnemyBases.Keys) { 

                if (Vector3.Distance(item.transform.position, tank.transform.position) < closestDistance)
                {
                    closestDistance = Vector3.Distance(item.transform.position, tank.transform.position);
                    closestBase = item;
                    
                }
            }

            //get distance between tank and closest base
            float baseDist = Vector3.Distance(tank.transform.position, closestBase.transform.position);

            //if outside fire range, get close
            if (baseDist > 35f)
            {
                tank.FollowPathToWorldPoint(closestBase, 1f, tank.heuristicMode); 
            }
            //if within fire range, shoot
            else if (baseDist < 35f)
            {
                tank.TurretFireAtPoint(closestBase);
            }
        }
        return null;
    }
}