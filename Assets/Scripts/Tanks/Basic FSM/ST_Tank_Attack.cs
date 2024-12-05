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

            //Debug.Log(t);
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

        //if enemy tank out of FOV, switch to search state
        if (tank.VisibleEnemyTanks.Count <= 0 && tank.VisibleEnemyBases.Count <= 0)
        {
            Debug.Log("switching to search mode");
            //tank.ST_Tank_Search();
            return typeof(ST_Tank_Search);
        }

        //if enemy tank out of firing range, switch to chase state
        if (tank.VisibleEnemyTanks.Count >= 1 && Vector3.Distance(tank.transform.position, tank.VisibleEnemyTanks.Keys.First().transform.position) > 35f)
        {
            Debug.Log("switching to Chase state");
            //tank.ST_Tank_Chase();
            return typeof(ST_Tank_Chase);

        }
        //if enemy within fire range, switch to attack state
        else if (tank.VisibleEnemyTanks.Count >= 1 && Vector3.Distance(tank.transform.position, tank.VisibleEnemyTanks.Keys.First().transform.position) <= 35f)
        {
            Debug.Log("Switching to Attack State");
            //tank.ST_Tank_Attack();
            return typeof(ST_Tank_Attack);
        }

        //if there are no tanks in FOV and at least 1 enemy base in FOV
        if (tank.VisibleEnemyTanks.Count <= 0 && tank.VisibleEnemyBases.Count > 0)
        {
            GameObject enemyBase = tank.VisibleEnemyBases.Keys.First();
            float baseDist = Vector3.Distance(tank.transform.position, enemyBase.transform.position);

            //if outside fire range, get close
            if (tank.VisibleEnemyBases.Count > 0 && baseDist > 35f)
            {
                Debug.Log("getting close to enemy base");
                tank.FollowPathToWorldPoint(enemyBase, 1f, tank.heuristicMode);
            }
            //if within fire range, shoot
            else if (baseDist < 35f)
            {
                Debug.Log("Firing at enemy base");
                tank.TurretFireAtPoint(tank.VisibleEnemyBases.Keys.First());
            }
            //tank.TurretFaceWorldPoint(tank.VisibleEnemyBases.Keys.First());

        }

        return null;
    }
}
