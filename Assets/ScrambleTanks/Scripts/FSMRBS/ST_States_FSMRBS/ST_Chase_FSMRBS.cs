using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class ST_Chase_FSMRBS : ST_Base_FSMRBS
{
    private ST_SmartTankFSMRBS smartTank;
    float time;

    public ST_Chase_FSMRBS(ST_SmartTankFSMRBS smartTank)
    {
        this.smartTank = smartTank;
    }

    // setting up to enter the state
    public override Type EnterState()
    {
        smartTank.stats["chaseState_FSMRBS"] = true;
        return null;
    }

    // anything that needs to be reset or changed once the player leaves the stage
    public override Type LeaveState()
    {
        smartTank.stats["chaseState_FSMRBS"] = false;
        return null;
    }

    // logic that runs every physics update inside of the controller
    public override Type StateLogic()
    {

        var visibleEnemies = smartTank.VisibleEnemyTanks;
        float enemyDist = Vector3.Distance(smartTank.transform.position, smartTank.VisibleEnemyTanks.Keys.First().transform.position);


        if (time > 1f)
        {
            if (smartTank.stats["lowHealth_FSMRBS"] == true) { return typeof(ST_Retreat_FSMRBS); } //when low health switch toretreat
            if (smartTank.stats["lowFuel_FSMRBS"] == true) { return typeof(ST_Retreat_FSMRBS); } // when low fuel chage retreat
            if (smartTank.stats["lowAmmo_FSMRBS"] == true) { return typeof(ST_Retreat_FSMRBS); }// when low ammo no point attacj retreat
            if (smartTank.stats["targetReached"] == true) { return typeof(ST_Attack_FSMRBS); } //when reached target, attack
            else { return typeof(ST_Search_FSMRBS); } //if none of those, search
        }


        if ((visibleEnemies.Count > 0 && enemyDist > 35f) || smartTank.VisibleConsumables.Count > 0)
        {
            if (enemyDist > 35f)
            {
                smartTank.FollowPathToWorldPoint(visibleEnemies.Keys.First(), normalizedSpeed: 1.0f);//moving towards the visible enemy
                smartTank.TurretFaceWorldPoint(smartTank.VisibleEnemyTanks.Keys.First());//pointing turret at enemy
                Debug.Log("chasing enemy");
                return typeof(ST_Chase_FSMRBS);//stay in chase

            }
            else if (smartTank.VisibleConsumables.Count > 0)
            {
                smartTank.FollowPathToWorldPoint(smartTank.VisibleConsumables.Keys.First(), normalizedSpeed: 1.0f);//moving towards the visible consumable prioritising
                return typeof(ST_Chase_FSMRBS);
            }

        }
        else
        {
            Debug.Log("close enough switching to attack");
            return typeof(ST_Attack_FSMRBS);//when timer runs out tank attacks

        }
        return null;
    }
}
