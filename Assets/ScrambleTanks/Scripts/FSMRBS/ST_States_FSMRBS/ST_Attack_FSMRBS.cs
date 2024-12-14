using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ST_Attack_FSMRBS : ST_Base_FSMRBS
{
    private ST_SmartTankFSMRBS smartTank;
    float time;
    public float t;
    public bool circlingClockwise;

    public ST_Attack_FSMRBS(ST_SmartTankFSMRBS smartTank)
    {
        this.smartTank = smartTank;
    }

    // setting up to enter the state
    public override Type EnterState()
    {
        smartTank.stats["attackState_FSMRBS"] = true;
        return null;
    }

    // anything that needs to be reset or changed once the player leaves the stage
    public override Type LeaveState()
    {
        smartTank.stats["attackState_FSMRBS"] = false;
        return null;
    }

    // logic that runs every physics update inside of the controller
    public override Type StateLogic()
    {
        time += Time.deltaTime;

        //state swap conditions
        if (time > 1f)
        {
            if (smartTank.stats["lowHealth_FSMRBS"] == true) { return typeof(ST_Retreat_FSMRBS); } //when low health, retreat
            if (smartTank.stats["lowFuel_FSMRBS"] == true) { return typeof(ST_Retreat_FSMRBS); } // when low fuel, retreat
            if (smartTank.stats["lowAmmo_FSMRBS"] == true) { return typeof(ST_Retreat_FSMRBS); }// when low ammo, retreat
            if (smartTank.stats["targetReached"] == true) { return typeof(ST_Attack_FSMRBS); } //when reached target, attack
            else { return typeof(ST_Search_FSMRBS); } //if none of those, search
        }

        if (smartTank.VisibleEnemyTanks.Count > 0) //when tanks in sight
        {
            Debug.LogError("I can a tank");
            float dist = Vector3.Distance(smartTank.transform.position, smartTank.VisibleEnemyTanks.Keys.First().transform.position);
            Vector3 normalized = (smartTank.transform.position - smartTank.VisibleEnemyTanks.Keys.First().transform.position).normalized;
            float circlingAngle = 90f;
            float circlingAngleBeforeReadyingUpToShoot = 40f;


            //use rays to check if there's an obstacle
            RaycastHit hit;
            if (
                Physics.Raycast(
                    new Ray(smartTank.transform.position, smartTank.calcTransform.position - smartTank.transform.position),
                    out hit,
                    Vector3.Distance(smartTank.transform.position, smartTank.calcTransform.position),
                    smartTank.raycastLayers
                    )
                )
            {
                circlingClockwise = !circlingClockwise;
                Debug.DrawLine(smartTank.transform.position, hit.point, Color.red, 10);
            }

            circlingAngle *= circlingClockwise ? -1 : 1;
            circlingAngleBeforeReadyingUpToShoot *= circlingClockwise ? -1 : 1;

            if (t > 5) //at 5 seconds round out
            {
                normalized = Quaternion.AngleAxis(circlingAngleBeforeReadyingUpToShoot, Vector3.up) * normalized;
                smartTank.calcTransform.position = smartTank.VisibleEnemyTanks.Keys.First().transform.position + normalized * 20;
            }
            else // any other time close in
            {
                normalized = Quaternion.AngleAxis(circlingAngle, Vector3.up) * normalized;
                smartTank.calcTransform.position = smartTank.VisibleEnemyTanks.Keys.First().transform.position + normalized * 5;
            }

            if (t > 7) //when at 7 secs restart t and attack
            {
                t = 0;
                smartTank.TurretFaceWorldPoint(smartTank.enemyLastSeen.gameObject);
                smartTank.TurretFireAtPoint(smartTank.VisibleEnemyTanks.Keys.First());
            }
            else if (dist < 25F) { t += Time.deltaTime; } //keep counting time if within distance
            smartTank.FollowPathToWorldPoint(smartTank.calcTransform.gameObject, 0.5f, smartTank.heuristicMode);
            smartTank.TurretFaceWorldPoint(smartTank.enemyLastSeen.gameObject);
        }

        //if there are no tanks in FOV and at least 1 enemy base in FOV
        if (smartTank.VisibleEnemyBases.Count > 0 && smartTank.VisibleEnemyTanks.Count <= 0)
        {
            float closestDistance = 999f;
            GameObject closestBase = null;

            //chose closest base
            foreach (GameObject item in smartTank.VisibleEnemyBases.Keys)
            {

                if (Vector3.Distance(item.transform.position, smartTank.transform.position) < closestDistance)
                {
                    closestDistance = Vector3.Distance(item.transform.position, smartTank.transform.position);
                    closestBase = item;

                }
            }

            //get distance between tank and closest base
            float baseDist = Vector3.Distance(smartTank.transform.position, closestBase.transform.position);

            //if outside fire range, get close
            if (baseDist > 35f)
            {
                smartTank.FollowPathToWorldPoint(closestBase, 1f, smartTank.heuristicMode);
            }
            //if within fire range, shoot
            else if (baseDist < 35f)
            {
                smartTank.TurretFireAtPoint(closestBase);
            }
        }
        return null;
    }
}

// OwO
