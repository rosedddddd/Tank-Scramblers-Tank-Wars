using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ST_Tank_Attack : ST_BaseTankState
{
    public float t;
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
        if (tank.VisibleEnemyTanks.Count > 0)
        {
            float dist = Vector3.Distance(tank.transform.position, tank.VisibleEnemyTanks.Keys.First().transform.position);
            Vector3 normalized = (tank.transform.position - tank.VisibleEnemyTanks.Keys.First().transform.position).normalized;
            float circlingAngle = 90f;
            float circlingAngleBeforeReadyingUpToShoot= 40f;

            Debug.Log(t);
            if (t > 5)
            {
                normalized = Quaternion.AngleAxis(circlingAngleBeforeReadyingUpToShoot, Vector3.up) * normalized;
                tank.calcTransform.position = tank.VisibleEnemyTanks.Keys.First().transform.position + normalized * 20;
            }
            else
            {
                normalized = Quaternion.AngleAxis(circlingAngle, Vector3.up) * normalized;
                tank.calcTransform.position = tank.VisibleEnemyTanks.Keys.First().transform.position + normalized * 5;
            }

            if (t > 7)
            {
                t = 0;
                tank.TurretFaceWorldPoint(tank.enemyLastSeen.gameObject);
                tank.TurretFireAtPoint(tank.VisibleEnemyTanks.Keys.First());
            }
            else if (dist < 25F) { t += Time.deltaTime; }
        }
        tank.FollowPathToWorldPoint(tank.calcTransform.gameObject, 0.5f, tank.heuristicMode);
        tank.TurretFaceWorldPoint(tank.enemyLastSeen.gameObject);

        if (tank.VisibleEnemyTanks.Count <= 0 && tank.VisibleEnemyBases.Count > 0)
        {
            GameObject enemyBase = tank.VisibleEnemyBases.Keys.First();
            float baseDist = Vector3.Distance(tank.transform.position, enemyBase.transform.position);

            //if outside fire range, get close
            if (tank.VisibleEnemyBases.Count > 0 && baseDist > 35f)
            {
                Debug.Log("I not brave");
                tank.FollowPathToWorldPoint(enemyBase, 1f, tank.heuristicMode);
            }
            //if within fire range, shoot
            if (baseDist < 35f)
            {
                tank.TurretFireAtPoint(tank.VisibleEnemyBases.Keys.First());
            }
            //tank.TurretFaceWorldPoint(tank.VisibleEnemyBases.Keys.First());

        }

        return null;
    }
}
