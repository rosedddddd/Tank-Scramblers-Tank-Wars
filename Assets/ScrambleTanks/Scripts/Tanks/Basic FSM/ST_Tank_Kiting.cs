using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ST_Tank_Kiting : ST_BaseTankState
{
    public float t;
    bool circlingClockwise = true;
    public float kitCount;

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
        Debug.Log("currently in Kiting mode");

        

        if (tank.lastSeenTimer < 15)
        {
            tank.kitingDistance = 20f;
            float dist = Vector3.Distance(tank.transform.position, tank.enemyLastSeen.position);
            Vector3 normalized = (tank.transform.position - tank.enemyLastSeen.position).normalized;
            //float cirAngle = 90f;
            float cirAngleClose = 40f;
            cirAngleClose *= circlingClockwise ? -1 : 1;
            normalized = Quaternion.AngleAxis(cirAngleClose, Vector3.up) * normalized;
            tank.calcTransform.position = tank.enemyLastSeen.position + normalized * tank.kitingDistance;

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
                Debug.DrawLine(tank.transform.position, hit.point, Color.red,10);
            }
            else
            {
                Debug.DrawLine(tank.transform.position, tank.transform.position + tank.calcTransform.position - tank.transform.position);
            }

            tank.GeneratePathToWorldPoint(tank.calcTransform.gameObject);
            tank.FollowPathToWorldPoint(tank.calcTransform.gameObject, 0.5f, tank.heuristicMode);

            if (tank.TankCurrentHealth <= 75) { return typeof(ST_Tank_Retreat); }
            if(t >= 15) { return typeof(ST_Tank_Attack);}

            if (dist < 25F) { t += Time.deltaTime; }
            
            tank.TurretFaceWorldPoint(tank.enemyLastSeen.gameObject);
        }


        return null;
    }
}
