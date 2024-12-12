using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ST_Attack_FSMRBS : ST_Base_FSMRBS
{
    private ST_SmartTankFSMRBS smartTank;
    float time;

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
            if (smartTank.stats["lowHealth_FSMRBS"] == true) { return typeof(ST_Retreat_FSMRBS);} //when low health, retreat
            if (smartTank.stats["lowFuel_FSMRBS"] == true) { return typeof(ST_Retreat_FSMRBS); } // when low fuel, retreat
            if (smartTank.stats["lowAmmo_FSMRBS"] == true) { return typeof(ST_Retreat_FSMRBS); }// when low ammo, retreat
            if (smartTank.stats["targetReached"] == true) { return typeof(ST_Attack_FSMRBS);} //when reached target, attack
            else { return typeof(ST_Search_FSMRBS); } //if none of those, search
        }

        return null;
    }
}

// OwO
